/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/12/15 10:54:03 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 文件提取结果数据集
    /// </summary>
    [Serializable]
    public class FileTreeDataSource : TreeDataSource
    {
        public override void Filter<T>(params FilterArgs[] args)
        {
            base.Filter<T>(args);
            if (TreeNodes.Any())
            {
                foreach (var node in TreeNodes)
                {
                    node.Filter<T>(args);
                }

                Total = TreeNodes.First().Total;//文件提取结果只统计第一个节点
                DeleteTotal = TreeNodes.First().DeleteTotal;
            }

            if (null != PluginInfo && !String.IsNullOrEmpty(PluginInfo.SaveDbPath))
            {
                DataFilter.Providers.SQLiteFilterDataProvider.ClearSQLiteConnectionCatch(PluginInfo.SaveDbPath);
            }
        }
    }
}
