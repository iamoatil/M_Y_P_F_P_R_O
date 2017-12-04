/* ==============================================================================
* Description：UrlWarning  
* Author     ：litao
* Create Date：2017/12/2 10:15:26
* ==============================================================================*/

using System;
using System.Collections.Generic;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class UrlWarningPlugin : AbstractEarlyWarningPlugin
    {
        public UrlWarningPlugin()
        {
            var p = new EarlyWarningPluginInfo()
            {
                Guid = "{F8EB7422-6C4E-43A1-9C3B-D5FF04371268}",
                Name = "UrlWarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning
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
