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

        //[NonSerialized]
        //private readonly DataAggregationFilterView<T> _filterView;
        [NonSerialized]
        private DataAggregationFilterView<T> _filterViewInstance;
        private DataAggregationFilterView<T> _filterView
        {
            get
            {
                if(_filterViewInstance == null)
                {
                    _filterViewInstance = new DataAggregationFilterView<T>(this, Key);
                    _filterViewInstance.OnAssociatedBookmark += _filterView_OnAssociatedBookmark;
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
                    _providerInstance = new SQLiteFilterDataProvider(DbFilePath, DbTableName);
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
            if(tableName != null)
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
            //_provider = new SQLiteFilterDataProvider(dbFilePath, DbTableName);
            //_filterView = new DataAggregationFilterView<T>(this, Key);
            //_filterView.OnAssociatedBookmark += _filterView_OnAssociatedBookmark;
            _empty = new T[0];
        }

     

        #endregion

        #region Properties

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

        public IFilterDataProvider Provider => _provider;

        public SqliteDbFile DbInstance
        {
            get { return SqliteDbFile.GetSqliteDbFile(DbFilePath); }
        }

        public IEnumerable<T> View => _filterView.View ?? _empty;

        IEnumerable IDataItems.View => View;

        public Int32 Count => _filterView.Count;

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
            this.Add(obj as T);
        }

        public void Filter(params FilterArgs[] args)
        {
            _filterView.Args = args;
            _filterView.Filter();
            OnPropertyChanged("View");
            OnPropertyChanged("Count");
        }


        #endregion

        #region Private
        /// <summary>
        /// 关联数据的书签状态
        /// </summary>
        private void _filterView_OnAssociatedBookmark()
        {
            DbInstance.GetDataItemsBookmark(View);
        }
        #endregion
        #endregion
    }
}
