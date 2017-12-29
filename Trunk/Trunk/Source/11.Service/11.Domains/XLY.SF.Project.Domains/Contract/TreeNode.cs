using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains.Contract;
using XLY.SF.Project.Domains.Contract.DataItemContract;

/* ==============================================================================
* Description：TreeNode  
* Author     ：Fhjun
* Create Date：2017/3/17 16:49:26
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 树节点
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public class TreeNode : NotifyPropertyBase, IDataState, ICheckedItem, IDecoration
    {
        public TreeNode()
        {
            TreeNodes = new List<TreeNode>();
            DataState = EnumDataState.Normal;
            Key = Guid.NewGuid();
        }

        /// <summary>
        /// 数据状态
        /// </summary>
        public EnumDataState DataState { get; set; }

        public Guid Key { get; set; }

        /// <summary>
        /// 树节点Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 显示的内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeNode> TreeNodes { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public object Parent { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(DataItemJsonConverter))]
        public IDataItems Items { get; set; }

        private Type _type = null;
        /// <summary>
        /// Items的数据类型，脚本中须配置
        /// </summary>
        public Type Type
        {
            get
            {
                if (_type == null && Items != null)
                {
                    if (Items.GetType().IsGenericType)
                    {
                        _type = Items.GetType().GetGenericArguments()[0];
                    }
                    else
                    {
                        _type = Items.GetType();
                    }
                }
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// 是否隐藏子节点（在数据展示页面有效），为true则不显示子节点，此时子节点显示由数据展示插件控制
        /// </summary>
        public bool IsHideChildren { get; set; }

        /// <summary>
        /// 统计总数时是否累计子节点
        /// 默认为true
        /// </summary>
        public bool IsIncludeChildrenInTotal { get; set; } = true;

        private int _total = -1;

        /// <summary>
        /// 总数
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public int Total
        {
            get
            {
                if (_total == -1)
                {
                    Filter<dynamic>();
                }
                return _total;
            }
            protected set
            {
                _total = value;
            }
        }

        private int _deletetotal = -1;

        /// <summary>
        /// 删除总数
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public int DeleteTotal
        {
            get
            {
                if (_deletetotal == -1)
                {
                    Filter<dynamic>();
                }
                return _deletetotal;
            }
            protected set
            {
                _deletetotal = value;
            }
        }

        #region CheckState

        private bool? _isChecked = false;
        /// <summary>
        /// 当前数据是否被勾选
        /// </summary>
        public new bool? IsChecked
        {
            get => _isChecked;
            set
            {
                this.SetCheckedState(value, () => { _isChecked = value; OnPropertyChanged(); });
                //if(value != null)
                //{
                //    if (Items != null)
                //    {
                //        int state = value == null ? -1 : value == true ? 1 : 0;
                //        Items.UpdateRange("IsChecked", state);
                //    }
                //}
            }
        }

        #region 是否可见
        private bool? _IsVisible = true;

        /// <summary>
        /// 是否可见
        /// </summary>	
        public bool? IsVisible
        {
            get { return _IsVisible; }
            set
            {
                _IsVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private string _sourcePath = null;
        public string SourcePath { get => _sourcePath ?? (Parent as ICheckedItem)?.SourcePath; set => _sourcePath = value; }

        ICheckedItem ICheckedItem.Parent { get => (Parent as ICheckedItem); set => Parent = value; }

        public IEnumerable<ICheckedItem> GetChildren()
        {
            //IEnumerable<ICheckedItem> ls = new List<ICheckedItem>();
            //if (Items != null)
            //{
            //    ls = ls.Concat(Items.GetView() as IEnumerable<ICheckedItem>);
            //}
            //if (TreeNodes != null)
            //{
            //    ls = ls.Concat(TreeNodes);
            //}
            //return ls;
            return TreeNodes;
        }
        #endregion

        public void Commit()
        {
            if (null != Items)
            {
                Items.Commit();
            }
        }

        public void BuildParent()
        {
            Commit();
            if (Items != null)
                Items.Parent = this;
            if (TreeNodes.Any())
            {
                TreeNodes.ForEach((n) =>
                {
                    n.Commit();
                    n.Parent = this;
                    n.BuildParent();
                });
            }
        }

        public override string ToString()
        {
            return Text;
        }

        #region 数据查询

        public void Filter<T>(params FilterArgs[] args)
        {
            Int32 subNodeTotal = 0;
            Int32 subNodeDeleteTotal = 0;
            if (TreeNodes.Any())
            {
                foreach (var node in TreeNodes)
                {
                    node.Filter<T>(args);
                    subNodeTotal += node.Total;
                    subNodeDeleteTotal += node.DeleteTotal;
                }
            }
            if (Items != null)
            {
                Items.Filter(args);
                if (IsIncludeChildrenInTotal)
                {
                    _total = Items.Count + subNodeTotal;
                    _deletetotal = Items.DeleteCount + subNodeDeleteTotal;
                }
                else
                {
                    _total = Items.Count;
                    _deletetotal = Items.DeleteCount;
                }
            }
            else
            {
                if (IsIncludeChildrenInTotal)
                {
                    _total = subNodeTotal;
                    _deletetotal = subNodeDeleteTotal;
                }
                else
                {
                    _total = 0;
                    _deletetotal = 0;
                }
            }
        }

        #endregion

        /// <summary>
        /// 重新打开数据后，设置当前的数据文件路径
        /// </summary>
        /// <param name="path"></param>
        public void SetCurrentPath(string path)
        {
            if (Items != null)
            {
                Items.DbFilePath = System.IO.Path.Combine(path, "data.db");
                Items.ResetTableName();
            }
            foreach (var item in TreeNodes)
            {
                item.SetCurrentPath(path);
            }
        }

        public object GetMetaData(DecorationProperty dp)
        {
            return SourcePath;
        }

        public string GetKey(DecorationProperty dp)
        {
            return Key.ToString();
        }
    }
}
