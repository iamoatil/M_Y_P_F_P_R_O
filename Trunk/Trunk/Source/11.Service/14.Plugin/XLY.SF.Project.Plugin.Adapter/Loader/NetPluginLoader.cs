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
        protected override void LoadPlugin(IAsyncTaskProgress asyn)
        {
            try
            {
                Plugins = new List<IPlugin>();

                var files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "XLY.SF.Project.Plugin.*.dll");
                foreach (var dllFile in files)
                {
                    var ass = Assembly.LoadFile(dllFile);
                    foreach (var cla in ass.GetTypes().Where(t => t.GetCustomAttribute<PluginAttribute>() != null))
                    {
                        var plu = ass.CreateInstance(cla.FullName) as IPlugin;
                        if (null != plu)
                        {
                            Plugins.Add(plu);
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
