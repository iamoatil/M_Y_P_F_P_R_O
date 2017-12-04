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
    class KeyWordWarningPlugin : AbstractEarlyWarningPlugin
    {
        public KeyWordWarningPlugin()
        {
            var p = new EarlyWarningPluginInfo()
            {
                Guid = "{617C7AC4-4711-4C21-A7C3-D18696EE1C42}",
                Name = "KeyWordWarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning,
                DataSourceTypes = new List<Type> { typeof(MMSDataSource) ,typeof(SmsDataSource) }
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
