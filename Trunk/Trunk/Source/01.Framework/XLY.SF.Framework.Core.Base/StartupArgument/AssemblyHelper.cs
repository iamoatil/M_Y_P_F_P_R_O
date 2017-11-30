using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Framework.Core.Base.StartupArgument.AssemblyHelper
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/28 14:50:54
* ==============================================================================*/

namespace XLY.SF.Framework.Core.Base
{
    /// <summary>
    /// 用于程序集的动态加载，比如IOC加载外部的程序集、插件程序集配置等；
    /// 主要用于多个组件间的调用（比如数据展示组件调用数据预览组件）
    /// </summary>
    public class AssemblyHelper
    {
        public AssemblyHelper()
        {
            var asm = ConverterPathConfig(ConfigurationManager.AppSettings[AssemblyPathKey]);
            AssemblyPath = new List<string>();
            foreach (var item in asm)
            {
                if(File.Exists(item))
                {
                    AssemblyPath.Add(item);
                }
                else if(Directory.Exists(item))
                {
                    AssemblyPath.AddRange(Directory.GetFiles(item, "*.dll", SearchOption.AllDirectories));
                    AssemblyPath.AddRange(Directory.GetFiles(item, "*.exe", SearchOption.AllDirectories));
                }
            }
            IocPath = ConverterPathConfig(ConfigurationManager.AppSettings[IocPathKey]);
            PluginPath = ConverterPathConfig(ConfigurationManager.AppSettings[PluginPathKey]);
        }
        #region Public

        private static AssemblyHelper _instance = null;
        public static AssemblyHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AssemblyHelper();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 需要动态引入的程序集路径，通过app.config中&lt;add key="AssemblyPath" value=""/&gt;配置
        /// </summary>
        public List<string> AssemblyPath { get; set; }
        /// <summary>
        /// 需要IOC载入的程序集路径，通过app.config中&lt;add key="IocPath" value=""/&gt;配置
        /// </summary>
        public List<string> IocPath { get; set; }
        /// <summary>
        /// 需要加载插件的程序集路径，通过app.config中&lt;add key="PluginPath" value=""/&gt;配置
        /// </summary>
        public List<string> PluginPath { get; set; }

        /// <summary>
        /// 加载外部引入的程序集
        /// </summary>
        public void Load()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public void Unload()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }
        #endregion

        #region Private
        private const string AssemblyPathKey = "AssemblyPath";
        private const string IocPathKey = "IocPath";
        private const string PluginPathKey = "PluginPath";

        /// <summary>
        /// 程序集解析失败时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var strTempAssmbPath = "";
            string dllName = args.Name.Substring(0, args.Name.IndexOf(","));
            strTempAssmbPath = AssemblyPath.FirstOrDefault(s => Path.GetFileNameWithoutExtension(s).Equals(dllName));
            return string.IsNullOrWhiteSpace(strTempAssmbPath) ? null : Assembly.LoadFrom(strTempAssmbPath);
        }

        /// <summary>
        /// 解析路径配置，转换为路径名称列表
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private static List<string> ConverterPathConfig(string paths)
        {
            List<string> lst = new List<string>();
            if (string.IsNullOrWhiteSpace(paths))
                return lst;
            foreach (var p in paths.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, p);
                if (File.Exists(p))        //只添加存在的文件
                    lst.Add(new FileInfo(p).FullName);
                else if (Directory.Exists(p))        //只添加存在的文件
                    lst.Add(new DirectoryInfo(p).FullName);
            }
            return lst;
        }
        #endregion


    }
}
