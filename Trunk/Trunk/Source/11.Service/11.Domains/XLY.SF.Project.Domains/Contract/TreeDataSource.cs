using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Project.Domains.Contract;

/* ==============================================================================
* Description：TreeDataSource  
* Author     ：Fhjun
* Create Date：2017/3/17 16:47:18
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 复杂数据节点
    /// </summary>
    [Serializable]
    public class TreeDataSource : AbstractDataSource
    {
        /// <summary>
        /// 导航树
        /// </summary>
        public List<TreeNode> TreeNodes { get; set; }

        public TreeDataSource()
        {
            TreeNodes = new List<TreeNode>();
        }

        public override void BuildParent()
        {
            if (this.TreeNodes.Any())
            {
                this.TreeNodes.ForEach((n) =>
                {
                    n.Commit();
                    n.Parent = this;
                    n.BuildParent();
                });
            }

            base.BuildParent();
        }

        public override void Filter<T>(params FilterArgs[] args)
        {
            base.Filter<T>(args);
            if (this.TreeNodes.Any())
            {
                foreach (var node in TreeNodes)
                {
                    node.Filter<T>(args);
                    Total += node.Total;
                    DeleteTotal += node.DeleteTotal;
                }
            }

            if (null != PluginInfo && !String.IsNullOrEmpty(PluginInfo.SaveDbPath))
            {
                DataFilter.Providers.SQLiteFilterDataProvider.ClearSQLiteConnectionCatch(PluginInfo.SaveDbPath);
            }
        }

        public override void SetCurrentPath(string path)
        {
            base.SetCurrentPath(path);
            if(TreeNodes != null)
            {
                foreach (var item in TreeNodes)
                {
                    item.SetCurrentPath(path);
                    //Items?.ResetTableName();
                }
            }
        }

        public override IEnumerable<ICheckedItem> GetChildren()
        {
            return TreeNodes;
            //if (TreeNodes != null)
            //{
            //    return base.GetChildren().Concat(TreeNodes);
            //}
            //else
            //{
            //    return base.GetChildren();
            //}
        }

    }
}
