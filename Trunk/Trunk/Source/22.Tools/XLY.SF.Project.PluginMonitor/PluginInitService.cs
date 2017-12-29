using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;

/* ==============================================================================
* Description：XLY.SF.Project.PluginMonitor.PluginInitService  
* Author     ：Fhjun
* Create Date：2017/12/27 13:47:30
* ==============================================================================*/

namespace XLY.SF.Project.PluginMonitor
{
    /// <summary>
    /// PluginInitService
    /// </summary>
    public class PluginInitService
    {
        #region Constructor
        public static PluginInitService Instance => SingleWrapperHelper<PluginInitService>.Instance;
        #endregion

        #region Public
        private string _hashFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugin_monitor.bin");
        public void Refresh(bool isForceRefresh = false)
        {
            bool latest = IsLatest();
            if (!isForceRefresh && latest)
            {
                Console.WriteLine("plugin_monitor is latest");
                return;
            }
            CreateDynamicType();
        }

        public bool IsLatest()
        {
            string hash = !File.Exists(_hashFile) ? "" : File.ReadAllText(_hashFile);

            StringBuilder sb = new StringBuilder();
            foreach (var plugin in PluginAdapter.Instance.Plugins.Select(p => p.PluginInfo))
            {
                if (plugin is DataParsePluginInfo pi && pi.ScriptSourceFilePath != null)
                {
                    if(!File.Exists(pi.ScriptSourceFilePath))
                    {
                        continue;
                    }
                    FileInfo fi = new FileInfo(pi.ScriptSourceFilePath);
                    sb.AppendLine($"{fi.FullName},{fi.Length},{fi.LastWriteTime},{fi.CreationTime}");
                }
            }
            var newHash = CryptographyHelper.MD5Encrypt(sb.ToString());
            File.WriteAllText(_hashFile, newHash);
            return newHash == hash;
        }
        #endregion

        #region Private
        private static List<string> _baseColumns = null;        //动态类型的基本列
        private static PropertyInfo[] _displayText = null;        //动态类型的display特性
        private void CreateDynamicType()
        {
            EmitCreator emit = new EmitCreator();
            string dllfile = EmitCreator.DefaultAssemblyName + ".dll";
            foreach (var plugin in PluginAdapter.Instance.Plugins.Select(p => p.PluginInfo))
            {
                if (plugin is DataParsePluginInfo pi && pi.DataView != null)
                {
                    try
                    {
                        Console.WriteLine($"\r\n============={pi.ScriptSourceFilePath}=================\r\n");
                        foreach (var dv in pi.DataView.DistinctX(v=>v.Type))
                        {
                            if (dv == null)
                                throw new Exception("加载脚本时出错！DataView为空");
                            if (string.IsNullOrWhiteSpace(dv.Type))
                            {
                                throw new Exception("加载脚本时出错！数据类型名称为空");
                            }

                            emit.CreateType($"{EmitCreator.DefaultAssemblyName}.Ns{plugin.Guid.RemoveSpecialChar()}.{dv.Type}", EmitCreator.DefaultAssemblyName, typeof(ScriptDataItem), null);

                            if (_baseColumns == null)
                            {
                                _baseColumns = new List<System.Reflection.PropertyInfo>(typeof(ScriptDataItem).GetProperties()).ConvertAll(p => p.Name);
                            }
                            if(_displayText == null)
                            {
                                _displayText = new PropertyInfo[2];
                                _displayText[0] = typeof(DisplayAttribute).GetProperty("Key");
                                _displayText[1] = typeof(DisplayAttribute).GetProperty("Text");
                            }

                            if (dv.Items != null)
                            {
                                foreach (var item in dv.Items)
                                {
                                    if (_baseColumns.Contains(item.Code))       //如果基类中包含了该列，则不需要创建
                                    {
                                        continue;
                                    }
                                    if(string.IsNullOrWhiteSpace(item.Code))
                                    {
                                        continue;
                                    }
                                    var property = emit.CreateProperty(item.Code, GetColumnType(item.Type, item.Format));
                                    emit.SetPropertyAttribute(property, typeof(DisplayAttribute), null, null, _displayText,new object[] { item.Code, string.IsNullOrWhiteSpace(item.Name) ? item.Code : item.Name });
                                }
                            }
                            emit.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("CreateDynamicType Error: " + ex.Message + ex.StackTrace);
                    }
                }
            }
            if (File.Exists(dllfile))
            {
                File.Delete(dllfile);
            }
            emit.SaveAsDll(dllfile);
            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "XLY.SF.Project.Domains.dll", SearchOption.AllDirectories))
            {
                FileInfo fi = new FileInfo(file);
                if(fi.DirectoryName != AppDomain.CurrentDomain.BaseDirectory)
                {
                    try
                    {
                        File.Copy(dllfile, Path.Combine(fi.DirectoryName, dllfile), true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("CreateDynamicType Copy err: " + ex.Message + ex.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// 获取列类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private Type GetColumnType(EnumColumnType type, string format)
        {
            switch (type)
            {
                case EnumColumnType.DateTime:
                    return typeof(DateTime);
                case EnumColumnType.Double:
                    return typeof(double);
                case EnumColumnType.Enum:
                    return type.GetType().Assembly.GetType(string.Format("XLY.SF.Project.Domains.{0}", format));
                case EnumColumnType.Int:
                    return typeof(int);
                case EnumColumnType.List:
                    return typeof(List<string>);
                default:
                    return typeof(string);

            }
        }
        #endregion
    }
}
