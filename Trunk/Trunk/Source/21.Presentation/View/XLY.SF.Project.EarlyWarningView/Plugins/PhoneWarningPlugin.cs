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
    class PhoneWarningPlugin : AbstractEarlyWarningPlugin
    {
        public PhoneWarningPlugin()
        {
            var p = new EarlyWarningPluginInfo()
            {
                Guid = "{B3A98E69-1F47-454B-B99F-2A090EF05F58}",
                Name = "PhoneWarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning,
                DataSourceTypes = new List<Type> { typeof(CallDataSource),typeof(ContactDataSource) }
            };
            PluginInfo = p;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            EarlyWarningPluginArgument ewArg = (EarlyWarningPluginArgument)arg;
            DeviceDataSource ds = ewArg.DeviceDataSource;
            List<DataNode> dataNodes = ewArg.DataNodes;
            IDataSource dataSource = ds.DataSource;

            foreach (DataNode dataNode in dataNodes)
            {
                string cmd = string.Format("{1} like '%{2}%'", dataSource.Items.DbTableName, SqliteDbFile.JsonColumnName, dataNode.SensitiveData.Value);
                IEnumerable<dynamic> result = dataSource.Items.FilterByCmd<dynamic>(cmd);
                foreach (AbstractDataItem item in result)
                {
                    item.SensitiveId = dataNode.SensitiveData.SensitiveId;
                }
            }
            return null;
        }
    }
}
