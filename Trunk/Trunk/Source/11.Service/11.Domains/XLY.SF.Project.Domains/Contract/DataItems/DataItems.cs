// ***********************************************************************
// Assembly:XLY.SF.Project.Domains
// Author:Songbing
// Created:2017-06-08 13:42:57
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.DataFilter;
using XLY.SF.Project.DataFilter.Providers;
using XLY.SF.Project.DataFilter.Views;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据集
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    [Serializable]
    public class DataItems<T> : NotifyPropertyBase, IDataItems<T>
        where T : AbstractDataItem
    {
        #region Fields

        [NonSerialized]
        private DataAggregationFilterView<T> _filterViewInstance;
        private DataAggregationFilterView<T> _filterView
        {
            get
            {
                if (_filterViewInstance == null)
                {
                    _filterViewInstance = new DataAggregationFilterView<T>(this, Key);
                }
                return _filterViewInstance;
            }
            set
            {
                _filterViewInstance = value;
            }
        }

        private readonly T[] _empty;

        [NonSerialized]
        private IFilterDataProvider _providerInstance;
        private IFilterDataProvider _provider
        {
            get
            {
                if (_providerInstance == null)
                {
                    _providerInstance = new MultiSQLiteFilterDataProvider(DbFilePath, DbTableName) { AttachedDatabase = DecorationExtesion.BookmarkItemProperty.GetDbFilePath(new System.IO.FileInfo(DbFilePath).Directory.Parent.FullName) };
                }
                return _providerInstance;
            }
            set
            {
                _providerInstance = value;
            }
        }

        #endregion

        #region Constructors

        public DataItems(string dbFilePath, bool isCreateNew = true, string tableName = null, string key = null)
        {
            Key = key ?? Guid.NewGuid().ToString();

            DbFilePath = dbFilePath;
            if (tableName != null)
            {
                DbTableName = tableName;
            }
            if (isCreateNew)
            {
                DbTableName = DbInstance.CreateTable<T>(DbTableName);
            }
            else
            {
                DbInstance.SetTableName(typeof(T).FullName, DbTableName);
            }
            _empty = new T[0];
        }

        #endregion

        #region Properties
        public ICheckedItem Parent { get; set; }

        /// <summary>
        /// 数据唯一标示
        /// </summary>
        public string Key { get; set; }

        public string DbTableName { get; set; }

        public void ResetTableName()
        {
            DbInstance.SetTableName(typeof(T).FullName, DbTableName);
        }

        public string DbFilePath { get; set; }

        public string DbBmkFilePath => System.IO.Path.Combine( new System.IO.FileInfo(DbFilePath).Directory.Parent.FullName,BookmarkDecorationProperty.DPDBName);

        public IFilterDataProvider Provider => _provider;

        public SqliteDbFile DbInstance
        {
            get { return SqliteDbFile.GetSqliteDbFile(DbFilePath); }
        }

        #endregion

        #region IPage

        /// <summary>
        /// 数据总数
        /// </summary>
        public Int32 Count => _filterView.Count;

        /// <summary>
        /// 删除数据总数
        /// </summary>
        public Int32 DeleteCount => 0;

        /// <summary>
        /// 获取当前的数据集
        /// </summary>
        public IEnumerable GetView(int cursor = 0, int pageSize = -1)
        {
            var view = _filterView.GetView(cursor, pageSize) as IEnumerable<T>;
            if (view != null)
            {
                foreach (var item in view)
                {
                    item.Parent = Parent;
                }
                DbInstance.OnReadView(view, Key);
            }
            return view;
        }

        public IEnumerator GetEnumerator()
        {
            return _empty.GetEnumerator();
        }

        #endregion

        #region Methods

        #region Public

        public void Commit()
        {
            if (null != DbInstance)
            {
                DbInstance.Commit();
            }
        }

        private void Add(T obj)
        {
            if (null != DbInstance)
            {
                DbInstance.Add(obj, Key);
            }
        }

        public void Add(object obj)
        {
            Add(obj as T);
        }

        public void AddRange(IEnumerable<object> list)
        {
            foreach (var obj in list)
            {
                Add(obj as T);
            }
        }
        
        public void Filter(params FilterArgs[] args)
        {
            _filterView.Reset();
            _filterView.Args = args;
            _filterView.Initialize();
            //_filterView.NextPage();
            OnPropertyChanged("View");
            OnPropertyChanged("Count");
        }

        #endregion

        #endregion
    }
}
