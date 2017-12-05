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
    public class PhoneWarningPlugin : AbstractEarlyWarningPlugin
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
            EarlyWarningResult earlyWarningResult = ewArg.EarlyWarningResult;

            List<DataNode> dataNodes = ewArg.DataNodes;
            AbstractDataSource dataSource = ds.DataSource as AbstractDataSource;
            
            if (dataSource == null
                || dataSource.Items == null)
            {
                return null;
            }

            foreach (DataNode dataNode in dataNodes)
            {
                //todo 此处dataNode.SensitiveData.CategoryName != "关键字"为硬代码
                if (dataNode.SensitiveData.CategoryName != "电话")
                {
                    continue;
                }

                string cmd = string.Format("{1} like '%{2}%'", dataSource.Items.DbTableName, SqliteDbFile.JsonColumnName, dataNode.SensitiveData.Value);
                IEnumerable<dynamic> result = dataSource.Items.FilterByCmd<dynamic>(cmd);
                foreach (AbstractDataItem item in result)
                {
                    item.SensitiveId = dataNode.SensitiveData.SensitiveId;
                }
                earlyWarningResult.SqlDb.WriteResult(result, dataSource.Items.DbTableName, (Type)dataSource.Type);
                earlyWarningResult.Serializer.Serialize(dataSource);
            }
            return null;
        }

        public override void Execute(object arg0)
        {
            throw new NotImplementedException();
        }
    }
}
