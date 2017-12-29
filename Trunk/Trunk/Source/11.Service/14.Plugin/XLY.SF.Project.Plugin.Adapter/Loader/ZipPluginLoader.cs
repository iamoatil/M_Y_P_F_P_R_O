// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter
// Author:Songbing
// Created:2017-04-11 14:04:39
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Contract.DataItemContract;

namespace XLY.SF.Project.Plugin.Adapter
{
    /// <summary>
    /// 脚本插件加载器
    /// </summary>
    internal class ZipPluginLoader : AbstractPluginLoader
    {
        public const string DebugScriptExtension = ".zip";          //未加密的脚本文件后缀
        public const string ReleaseScriptExtension = ".xlyzip";         //已加密的脚本文件后缀
        public const string PluginFileExtension = ".js";       //插件文件名后缀
        public const string PluginConfigFileName = "plugin.config";       //配置文件名
        private const string RarPassword = @"#soif!@1751fsd,84&^%23@())wer32''fsd!!**32199.sfd";   //密码
        private const string DesPassword = @"84@#U*;[FSD848afs@f,lSW";   //密码
        private static readonly string DefaultUnrarPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());  //默认的临时解压缩路径
        private object _lockobj = new object();

        /* 脚本文件结构:
         
         --Android_QQ_5.1.0.zip
            |--plugin.config          (主配置文件)
            |--main.js/main.py        (主插件，名称固定)
            |--icon.png               (图标)
            |--chalib                 (数据恢复特征库文件夹)
            |--其它文件及文件夹     
         
        */

        protected override void LoadPlugin(IAsyncTaskProgress asyn, params string[] pluginPaths)
        {
            List<IPlugin> plugins = new List<IPlugin>();
            List<FileInfo> pluginFiles = GetPluginFileList(new[] { DebugScriptExtension, ReleaseScriptExtension }, pluginPaths);
            System.Threading.Tasks.Parallel.ForEach(pluginFiles, (file) =>
            //pluginFiles.ForEach(file=>
            {
                try
                {
                    bool isPassword = file.Extension.Equals(ReleaseScriptExtension, StringComparison.OrdinalIgnoreCase);

                    //解压缩文件到临时目录
                    string tmpDir = UnRarFile(file, isPassword);

                    //读取配置文件
                    AbstractZipPluginInfo pluginInfo = ReadPluginInfo(Path.Combine(tmpDir, PluginConfigFileName));

                    //读取脚本文件内容
                    pluginInfo.ZipTempDirectory = tmpDir;
                    pluginInfo.ScriptSourceFilePath = file.FullName;
                    GetScriptLanguage(pluginInfo);
                    ReadScriptContent(pluginInfo, isPassword);

                    //生成插件实例
                    IPlugin plugin = GetPlugin(pluginInfo);
                    if (null != plugin)
                    {
                        plugin.PluginInfo = pluginInfo;
                        lock (_lockobj)
                        {
                            plugins.Add(plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Warn(ex, string.Format("解析脚本发生异常！脚本文件：{0}", file.FullName));
                }
                System.Threading.Thread.Sleep(20);
            });

            Plugins = plugins;
            StartDynamicProgram();
        }

        /// <summary>
        /// 执行动态类库生成
        /// </summary>
        private void StartDynamicProgram()
        {
            var p = System.Diagnostics.Process.GetProcessesByName("XLY.SF.Project.PluginMonitor");
            if(p.Length == 0)
            {
                if (!File.Exists("XLY.SF.Project.PluginMonitor.exe"))
                    return;
                System.Diagnostics.Process pro = new System.Diagnostics.Process();
                pro.StartInfo.FileName = "XLY.SF.Project.PluginMonitor.exe";
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.CreateNoWindow = true;

                pro.Start();
            }
        }

        /// <summary>
        /// 解压缩文件到临时目录
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="isPassword"></param>
        /// <returns></returns>
        private string UnRarFile(FileInfo fi, bool isPassword)
        {
            string path = Path.Combine(DefaultUnrarPath, fi.Name);
            if (isPassword)
            {//TODO：加密压缩包解压

            }
            else
            {
                ZipFile.ExtractToDirectory(fi.FullName, path);
            }
            return path;
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        private AbstractZipPluginInfo ReadPluginInfo(string configFile)
        {
            if (!FileHelper.IsValid(configFile))
            {
                throw new Exception("脚本配置文件不存在!");
            }
            Type pluginType = GetPluginTypeByConfigFile(configFile);
            var plugin = Serializer.DeSerializeFromXML(configFile, pluginType) as AbstractZipPluginInfo;
            if (plugin == null)
            {
                throw new Exception("脚本配置文件格式不正确，反序列化失败！");
            }

            //部分参数需要计算
            plugin.AfterReadConfigure();
            //if (plugin is DataParsePluginInfo dp)
            //{
            //    //动态创建数据类型
            //    if (dp.DataView != null)
            //    {
            //        dp.DataView.ForEach(dv => CreateDynamicType(dp, dv));
            //    }
            //}

            return plugin;
        }

        private static Dictionary<PluginLanguage, string> _dicLanguage = null;
        private static Dictionary<PluginLanguage, string> DicLanguage
        {
            get
            {
                if(_dicLanguage == null)
                {
                    _dicLanguage = new Dictionary<PluginLanguage, string>() {
                        { PluginLanguage.Html, "index.html" },
                        { PluginLanguage.JavaScript, "main.js" },
                        { PluginLanguage.Python36, "main.py" },
                        { PluginLanguage.Csharp, "Program.cs" },
                    };
                }
                return _dicLanguage;
            }
        }

        /// <summary>
        /// 获取脚本文件的语言类型
        /// </summary>
        /// <param name="plugin"></param>
        private void GetScriptLanguage(AbstractZipPluginInfo plugin)
        {
            foreach (var item in DicLanguage)
            {
                if (File.Exists(Path.Combine(plugin.ZipTempDirectory, item.Value)))
                {
                    plugin.Language = item.Key;
                    plugin.ScriptFile = Path.Combine(plugin.ZipTempDirectory, item.Value);
                    return;
                }
            }
            throw new Exception($"无法获取到插件的语言类型！{plugin.ZipTempDirectory}");
        }

        /// <summary>
        /// 读取脚本文件内容
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="isPassword"></param>
        /// <returns></returns>
        private void ReadScriptContent(AbstractZipPluginInfo plugin, bool isPassword)
        {
            switch(plugin.Language)
            {
                case PluginLanguage.JavaScript:
                    plugin.ScriptObject = FileHelper.FileToUTF8String(plugin.ScriptFile);
                    break;
                case PluginLanguage.Html:
                    plugin.ScriptObject = plugin.ScriptFile;
                    break;
                case PluginLanguage.Python36:
                    plugin.ScriptObject = plugin.ScriptFile;
                    break;
                case PluginLanguage.Csharp:
                    plugin.ScriptObject = null;
                    break;
            }
        }

        private static List<string> _baseColumns = null;        //动态类型的基本列
        ///// <summary>
        ///// 动态创建插件的类型
        ///// </summary>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private void CreateDynamicType(DataParsePluginInfo plugin, DataView dv)
        //{
        //    if (dv == null)
        //    {
        //        throw new Exception("加载脚本时出错！DataView为空");
        //    }
        //    if (string.IsNullOrWhiteSpace(dv.Type))
        //    {
        //        throw new Exception("加载脚本时出错！数据类型名称为空");
        //    }
            
        //    EmitCreator emit = new EmitCreator();
        //    emit.CreateType(dv.Type, $"{EmitCreator.DefaultAssemblyName}.Ns{plugin.Guid.RemoveSpecialChar()}", typeof(ScriptDataItem), GetInterfacesTypes(dv.Contract));

        //    if(_baseColumns == null)
        //    {
        //        _baseColumns = new List<System.Reflection.PropertyInfo>(typeof(ScriptDataItem).GetProperties()).ConvertAll(p=>p.Name);
        //    }

        //    if (dv.Items != null)
        //    {
        //        foreach (var item in dv.Items)
        //        {
        //            if (_baseColumns.Contains(item.Code))       //如果基类中包含了该列，则不需要创建
        //            {
        //                continue;
        //            }
        //            var property = emit.CreateProperty(item.Code, GetColumnType(item.Type, item.Format));
        //            emit.SetPropertyAttribute(property, typeof(DisplayAttribute), null, null);
        //        }
        //    }
        //    dv.DynamicType = emit.Save();
        //}

        ///// <summary>
        ///// 协议类型转换
        ///// </summary>
        ///// <param name="contract"></param>
        ///// <returns></returns>
        //private Type[] GetInterfacesTypes(string contract)
        //{
        //    return null;
        //    //if (string.IsNullOrWhiteSpace(contract))
        //    //{
        //    //    return null;
        //    //}
        //    //List<Type> lst = new List<Type>();
        //    //foreach (var c in contract.Split(','))
        //    //{
        //    //    Type t;
        //    //    switch (c.ToLower().Trim())
        //    //    {
        //    //        case "conversion":
        //    //            t = typeof(IConversion);
        //    //            break;
        //    //        //case "datastate":
        //    //        //    t = typeof(IDataState);
        //    //        //    break;
        //    //        case "file":
        //    //            t = typeof(IFile);
        //    //            break;
        //    //        case "mail":
        //    //            t = typeof(IMail);
        //    //            break;
        //    //        case "map":
        //    //            t = typeof(IMap);
        //    //            break;
        //    //        case "thumbnail":
        //    //            t = typeof(IThumbnail);
        //    //            break;
        //    //        default:
        //    //            t = null;
        //    //            break;
        //    //    }
        //    //    if (t != null && !lst.Contains(t))
        //    //    {
        //    //        lst.Add(t);
        //    //    }
        //    //}
        //    //return lst.ToArray();
        //}

        ///// <summary>
        ///// 获取列类型
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="format"></param>
        ///// <returns></returns>
        //private Type GetColumnType(EnumColumnType type, string format)
        //{
        //    switch (type)
        //    {
        //        case EnumColumnType.DateTime:
        //            return typeof(DateTime);
        //        case EnumColumnType.Double:
        //            return typeof(double);
        //        case EnumColumnType.Enum:
        //            return type.GetType().Assembly.GetType(string.Format("XLY.SF.Project.Domains.{0}", format));
        //        case EnumColumnType.Int:
        //            return typeof(int);
        //        case EnumColumnType.List:
        //            return typeof(List<string>);
        //        default:
        //            return typeof(string);

        //    }
        //}

        private Type GetPluginTypeByConfigFile(string configFile)
        {
            XDocument doc = XDocument.Load(configFile);
            string pluginType = doc.Element("plugin")?.Attribute("type")?.Value;
            if (string.IsNullOrWhiteSpace(pluginType))
            {
                throw new Exception("配置文件格式错误，未定义插件类型PluginType");
            }
            PluginType pt = (PluginType)Enum.Parse(typeof(PluginType), pluginType, true);
            switch (pt)
            {
                case PluginType.SpfDataParse:
                    return typeof(DataParsePluginInfo);
                case PluginType.SpfDataView:
                    return typeof(DataViewPluginInfo);
                case PluginType.SpfDataPreview:
                    return typeof(DataPreviewPluginInfo);
                case PluginType.SpfReport:
                    return typeof(DataReportPluginInfo);
                case PluginType.SpfReportModule:
                    return typeof(DataReportModulePluginInfo);
                default:
                    return null;
            }
        }

        private IPlugin GetPlugin(AbstractPluginInfo pluginInfo)
        {
            //if (pluginInfo is DataParsePluginInfo dp && dp.Language == PluginLanguage.JavaScript)
            //{
            //    return new DataJSScriptPlugin() { PluginInfo = pluginInfo };
            //}

            return PluginContainerAdapter.Instance.GetPlugin<IPlugin>(pluginInfo.PluginType);
        }
    }
}
