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
    public class AppWarningPlugin : AbstractEarlyWarningPlugin
    {
        public AppWarningPlugin()
        {
            var p = new AppEarlyWarningPluginInfo()
            {
                Guid = "{E3422DF4-B69A-4642-8420-B69ABCB413A4}",
                Name = "AppWarningPlugin",
                OrderIndex = 1,
                PluginType = PluginType.SpfEarlyWarning,               
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
                //todo 此处dataNode.SensitiveData.CategoryName != "App"为硬代码
                if (dataNode.SensitiveData.CategoryName != "App")
                {
                    continue;
                }
                string cmd = string.Format("{1} like '%{2}%'", dataSource.Items.DbTableName, SqliteDbFile.JsonColumnName, dataNode.SensitiveData.Value);
                IEnumerable<dynamic> result = dataSource.Items.FilterByCmd<dynamic>(cmd);
                foreach (AbstractDataItem item in result)
                {
                    item.SensitiveId = dataNode.SensitiveData.SensitiveId;
                    
                }
            }
            return null;
        }

        public override void Execute(object arg)
        {
            EarlyWarningPluginArgument ewArg = (EarlyWarningPluginArgument)arg;
            DeviceDataSource ds = ewArg.DeviceDataSource;
            IDataSource dataSource = ds.DataSource;


        }
    }
}
