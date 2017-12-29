using System;
using System.Collections.Generic;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains.Contract;

/* ==============================================================================
* Description：AbstractDataSource  
* Author     ：Fhjun
* Create Date：2017/3/17 16:30:01
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据基类
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public abstract class AbstractDataSource : NotifyPropertyBase, IDataSource, IDecoration
    {
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// 数据提取时使用的任务路径 ，该属性用于修正任务包路径修改后导致的路径不正确的问题
        /// </summary>
        public string DataExtractionTaskPath { get; set; }

        /// <summary>
        /// 当前任务路径
        /// </summary>
        public string CurrentTaskPath { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(DataItemJsonConverter))]
        public IDataItems Items { get; set; }

        /// <summary>
        /// 当前列表数据的类型，如果是C#插件则为Items的Type，如果为脚本插件为自定义类型名称
        /// </summary>
        public Type Type { get; set; }

        public DataParsePluginInfo PluginInfo { get; set; }

        private int _total = -1;

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
                this.SetCheckedState(value, () => { this._isChecked = value; OnPropertyChanged(); });
                //if(Items != null && value != null)
                //{
                //    int state = value == null ? -1 : value == true ? 1 : 0;
                //    Items.UpdateRange("IsChecked", state);
                //}
                if (this is TreeDataSource tree && value != null)
                {
                    if (tree.TreeNodes != null)
                    {
                        foreach (var tn in tree.TreeNodes)
                        {
                            tn.IsChecked = value;
                        }
                    }
                }
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
                this.SetTreeState(value, (item) => item.IsVisible, (item, v) => item.IsVisible = v, () => { this._IsVisible = value; OnPropertyChanged(); });
            }
        }
        #endregion

        private string _sourcePath = null;
        public string SourcePath { get => _sourcePath ?? Parent?.SourcePath; set => _sourcePath = value; }
        public ICheckedItem Parent { get; set; }
        public virtual IEnumerable<ICheckedItem> GetChildren()
        {
            return null;
            //return new ICheckedItem[0];
            //if (Items == null)
            //    return new ICheckedItem[0];
            //return Items.GetView() as IEnumerable<ICheckedItem>;
        }

        #endregion

        public virtual void BuildParent()
        {
            if (null != Items)
            {
                Items.Parent = this;
                Items.Commit();
            }
        }

        #region 读取数据后补齐路径
        /// <summary>
        /// 读取数据后补齐路径
        /// </summary>
        /// <param name="path"></param>
        public virtual void SetCurrentPath(string path)
        {
            CurrentTaskPath = path;
            if (Items != null)
            {
                Items.DbFilePath = System.IO.Path.Combine(path, "data.db");
                Items.ResetTableName();
            }
        }
        #endregion

        #region 数据查询

        public virtual void Filter<T>(params FilterArgs[] args)
        {
            if (Items == null)
            {
                _total = 0;
                _deletetotal = 0;
                return;
            }
            Items.Filter(args);
            _total = Items.Count;
            _deletetotal = Items.DeleteCount;

            if (null != PluginInfo && !String.IsNullOrEmpty(PluginInfo.SaveDbPath))
            {
                DataFilter.Providers.SQLiteFilterDataProvider.ClearSQLiteConnectionCatch(PluginInfo.SaveDbPath);
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

        #endregion
    }
}
