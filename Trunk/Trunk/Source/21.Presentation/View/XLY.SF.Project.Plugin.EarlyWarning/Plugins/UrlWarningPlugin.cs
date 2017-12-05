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
    public class UrlWarningPlugin : AbstractEarlyWarningPlugin
    {
        public UrlWarningPlugin()
        {
            var p = new UrlEarlyWarningPluginInfo()
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
            EarlyWarningResult earlyWarningResult = ewArg.EarlyWarningResult;

            List<DataNode> dataNodes = ewArg.DataNodes;
            IDataSource dataSource = ds.DataSource;            

            TreeDataSource treeDataSource = dataSource as TreeDataSource;
            if(treeDataSource == null
                || treeDataSource.TreeNodes.Count < 1)
            {
                return null;
            }
            foreach (DataNode dataNode in dataNodes)
            {
                //todo 此处dataNode.SensitiveData.CategoryName != "URL"为硬代码
                if (dataNode.SensitiveData.CategoryName != "URL")
                {
                    continue;
                }
                //todo 此处可以直接匹配，书签和历史记录           
                foreach (TreeNode treeNode in treeDataSource.TreeNodes)
                {
                    string cmd = string.Format("{1} like '%{2}%'", treeNode.Items.DbTableName, SqliteDbFile.JsonColumnName, dataNode.SensitiveData.Value);
                    IEnumerable<dynamic> result = treeNode.Items.FilterByCmd<dynamic>(cmd);
                    foreach (AbstractDataItem item in result)
                    {
                        item.SensitiveId = dataNode.SensitiveData.SensitiveId;
                    }
                    earlyWarningResult.SqlDb.WriteResult(result, treeNode.Items.DbTableName, (Type)treeNode.Type);
                    earlyWarningResult.Serializer.Serialize(treeDataSource);
                }
            }
            return null;
        }

        public override void Execute(object arg0)
        {
            throw new NotImplementedException();
        }
    }
}
