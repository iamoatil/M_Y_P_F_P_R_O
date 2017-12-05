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
    public class KeyWordWarningPlugin : AbstractEarlyWarningPlugin
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
            EarlyWarningResult earlyWarningResult = ewArg.EarlyWarningResult;

            List<DataNode> dataNodes = ewArg.DataNodes;
            AbstractDataSource dataSource = ds.DataSource as AbstractDataSource;

            TreeDataSource treeDataSource = dataSource as TreeDataSource;
            if (dataSource != null
                && dataSource.Items != null)
            {
                foreach (DataNode dataNode in dataNodes)
                {
                    //todo 此处dataNode.SensitiveData.CategoryName != "关键字"为硬代码
                    if (dataNode.SensitiveData.CategoryName != "关键字")
                    {
                        continue;
                    }

                    if (dataSource.Items != null)
                    {
                        string cmd = string.Format("{1} like '%{2}%'", dataSource.Items.DbTableName, SqliteDbFile.JsonColumnName, dataNode.SensitiveData.Value);
                        IEnumerable<dynamic> result = dataSource.Items.FilterByCmd<dynamic>(cmd);
                        foreach (AbstractDataItem item in result)
                        {
                            item.SensitiveId = dataNode.SensitiveData.SensitiveId;
                        }
                        earlyWarningResult.SqlDb.WriteResult(result, dataSource.Items.DbTableName, (Type)dataSource.Type);
                        earlyWarningResult.Serializer.Serialize(dataSource);
                    }
                    //todo TreeDataSource

                }
            }
            else if (treeDataSource != null
                && treeDataSource.TreeNodes.Count > 1)
            {
                foreach (DataNode dataNode in dataNodes)
                {
                    //todo 此处dataNode.SensitiveData.CategoryName != "URL"为硬代码
                    if (dataNode.SensitiveData.CategoryName != "关键字")
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
            }

            return null;
        }

        public override void Execute(object arg0)
        {
            throw new NotImplementedException();
        }
    }
}
