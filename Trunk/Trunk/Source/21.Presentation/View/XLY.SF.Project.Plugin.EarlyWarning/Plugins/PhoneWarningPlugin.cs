/* ==============================================================================
* Description：PhoneWarning  
* Author     ：litao
* Create Date：2017/12/2 10:15:48
* ==============================================================================*/

using System;
using System.Collections.Generic;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    internal class PhoneWarningPlugin : AbstractEarlyWarningPlugin
    {
        public PhoneWarningPlugin()
        {
            var p = new PhoneEarlyWarningPluginInfo()
            {
                Guid = "{B3A98E69-1F47-454B-B99F-2A090EF05F58}",
                Name = "PhoneWarningPlugin",
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
            string keyColumn = ConstDefinition.XLYJson ;
            ColumnUpdate(dataSource.Items, configDbManager, keyColumn);
            return null;
        }
        
    }
}
