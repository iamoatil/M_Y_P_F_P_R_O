using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.Adapter.PluginContainerAdapter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/17 13:25:00
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.Adapter
{
    /// <summary>
    /// 加载脚本容器
    /// </summary>
    public class PluginContainerAdapter
    {
        public static PluginContainerAdapter Instance => SingleWrapperHelper<PluginContainerAdapter>.Instance;

        public Dictionary<PluginType, Type> Plugins { get; set; }

        private bool _Isloaded = false;
        public void Initialization(IAsyncTaskProgress asyn, params string[] pluginPaths)
        {
            if (!_Isloaded)
            {
                _Isloaded = true;
                Plugins = new Dictionary<PluginType, Type>();
                List<string> existList = new List<string>();

                List<string> dirs = new List<string>();
                dirs.Add(AppDomain.CurrentDomain.BaseDirectory);
                dirs.AddRange(pluginPaths);

                foreach (var dir in dirs)
                {
                    var files = System.IO.Directory.GetFiles(dir, "XLY.SF.Project.*.dll", System.IO.SearchOption.AllDirectories);
                    foreach (var dllFile in files)
                    {
                        if (existList.Contains(System.IO.Path.GetFileName(dllFile)))
                        {
                            continue;
                        }
                        existList.Add(System.IO.Path.GetFileName(dllFile));
                        try
                        {
                            var ass = Assembly.LoadFile(dllFile);
                            foreach (var cla in ass.GetTypes().Where(t => t.GetCustomAttribute<PluginContainerAttribute>() != null && !t.IsAbstract && !t.IsInterface))
                            {
                                Plugins[cla.GetCustomAttribute<PluginContainerAttribute>().PluginType] = cla;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        public T GetPlugin<T>(PluginType type)
        {
            if (!Plugins.ContainsKey(type))
            {
                //throw new Exception("未匹配到合适的插件!");
                return default(T);
            }
            return (T)Activator.CreateInstance(Plugins[type]);
        }
    }
}
