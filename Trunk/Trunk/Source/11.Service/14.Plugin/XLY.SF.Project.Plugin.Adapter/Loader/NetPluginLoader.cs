// ***********************************************************************
// Assembly:XLY.SF.Project.Plugin.Adapter
// Author:Songbing
// Created:2017-04-11 14:04:20
// Description:
// ***********************************************************************
// Last Modified By:You Min
// Last Modified On:2017/5/23
// ***********************************************************************

using System.Collections.Generic;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;
using System.Linq;
using System;
using XLY.SF.Framework.Log4NetService;
using System.Reflection;
using XLY.SF.Project.Domains.Plugin;

namespace XLY.SF.Project.Plugin.Adapter
{
    /// <summary>
    /// C#插件加载器
    /// </summary>
    internal class NetPluginLoader : AbstractPluginLoader
    {
        protected override void LoadPlugin(IAsyncTaskProgress asyn, params string[] pluginPaths)
        {
            try
            {
                Plugins = new List<IPlugin>();
                List<string> existList = new List<string>();

                List<string> dirs = new List<string>();
                dirs.Add(AppDomain.CurrentDomain.BaseDirectory);
                dirs.AddRange(pluginPaths);
                foreach (var dir in dirs)
                {
                    var files = System.IO.Directory.GetFiles(dir, "XLY.SF.Project.Plugin.*.dll", System.IO.SearchOption.AllDirectories);
                    foreach (var dllFile in files)
                    {
                        if (existList.Contains(System.IO.Path.GetFileName(dllFile)))
                        {
                            continue;
                        }
                        existList.Add(System.IO.Path.GetFileName(dllFile));
                        var ass = Assembly.LoadFile(dllFile);
                        foreach (var cla in ass.GetTypes().Where(t => t.GetCustomAttribute<PluginAttribute>() != null && !t.IsAbstract && !t.IsInterface))
                        {
                            var plu = ass.CreateInstance(cla.FullName) as IPlugin;
                            if (null != plu && null != plu.PluginInfo)
                            {
                                if (Plugins.Any(p => p.PluginInfo.Guid == plu.PluginInfo.Guid))
                                {
                                    LoggerManagerSingle.Instance.Error($"C#插件加载出错！重复的Guid：{plu.PluginInfo.Guid}");
                                }
                                else
                                {
                                    Plugins.Add(plu);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "C#插件加载出错！");
            }
        }
    }
}
