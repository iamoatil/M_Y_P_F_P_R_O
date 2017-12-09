﻿/* ==============================================================================
* Description：Md5Warning  
* Author     ：litao
* Create Date：2017/12/2 10:14:57
* ==============================================================================*/

using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    internal class Md5WarningPlugin : AbstractEarlyWarningPlugin
    {
        public Md5WarningPlugin()
        {
            var p = new Md5EarlyWarningPluginInfo()
            {
                Guid = "{D531E61F-544C-44EB-A499-8BBA86069F45}",
                Name = "Md5WarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning,
            };
            p.DetectDirs.Add("path1");
            p.DetectDirs.Add("path2");
            p.DetectDirs.Add("path2");
            PluginInfo = p;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            //FileBrowsingServiceFactory.GetFileBrowsingService();

            return null;
        }

        
    }
}