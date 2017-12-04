/* ==============================================================================
* Description：AppWarning  
* Author     ：litao
* Create Date：2017/12/2 10:15:58
* ==============================================================================*/


using System;
using System.Collections.Generic;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class AppWarningPlugin : AbstractEarlyWarningPlugin
    {
        public AppWarningPlugin()
        {
            var p = new EarlyWarningPluginInfo()
            {
                Guid = "{E3422DF4-B69A-4642-8420-B69ABCB413A4}",
                Name = "AppWarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning,
                DataSourceTypes = new List<Type>{ typeof(int) }
            };
            PluginInfo = p;
        }        

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            return null;
        }
    }
}
