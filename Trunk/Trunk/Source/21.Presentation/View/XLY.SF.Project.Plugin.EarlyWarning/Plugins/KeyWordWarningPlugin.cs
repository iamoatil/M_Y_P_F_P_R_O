/* ==============================================================================
* Description：KeyWordWarning  
* Author     ：litao
* Create Date：2017/12/2 10:15:09
* ==============================================================================*/

using System;
using System.Collections.Generic;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    internal class KeyWordWarningPlugin : AbstractEarlyWarningPlugin
    {
        public KeyWordWarningPlugin()
        {
            var p = new KeyWordEarlyWarningPluginInfo()
            {
                Guid = "{617C7AC4-4711-4C21-A7C3-D18696EE1C42}",
                Name = "KeyWordWarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning,

            };
            PluginInfo = p;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            EarlyWarningPluginArgument ewArg = (EarlyWarningPluginArgument)arg;
            DeviceDataSource ds = ewArg.DeviceDataSource;
            DbFromConfigData configDbManager = ewArg.ConfigDbManager;

            AbstractDataSource dataSource = ds.DataSource as AbstractDataSource;

            if (dataSource == null
                || dataSource.Items == null)
            {
                return null;
            }
            KeyWordColumnUpdate(dataSource.Items, configDbManager);
            return null;
        }
    }
}
