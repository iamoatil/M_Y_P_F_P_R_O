using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains.Contract;
using XLY.SF.Project.Domains.Contract.DataItemContract;

/* ==============================================================================
* Description：AbstractDataItem  
* Author     ：Fhjun
* Create Date：2017/3/17 16:58:36
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 单行数据的基类
    /// </summary>
    [Serializable]
    public abstract class AbstractDataItem : NotifyPropertyBase, IDataState, IAOPPropertyChangedMonitor
    {
        protected AbstractDataItem()
        {
        }

        #region Base Property
        /// <summary>
        /// 数据状态，正常还是删除
        /// </summary>
        [Display(ColumnIndex = -1)]
        public EnumDataState DataState { get; set; } = EnumDataState.Normal;

        private string _md5 = null;

        /// <summary>
        /// 单行数据的MD5值
        /// </summary>
        [Display(ColumnIndex = 99)]
        public string MD5
        {
            get
            {
                if (_md5 == null)
                {
                    _md5 = BuildMd5();
                }
                return _md5;
            }
            set { _md5 = value; }
        }

        private int _bookMarkId = -1;
        /// <summary>
        /// 加入书签的编号，小于0则未加入书签
        /// </summary>
        [Display(Visibility = EnumDisplayVisibility.ShowInDatabase)]
        public int BookMarkId
        {
            get => _bookMarkId;
            set
            {
                _bookMarkId = value;
                OnPropertyChanged();
            }
        }

        private int _sensitiveId = -1;
        /// <summary>
        /// 敏感数据序号，比如包含了“涉黄、涉毒”等信息
        /// </summary>
        public int SensitiveId { get => _sensitiveId; set { _sensitiveId = value; OnPropertyChanged(); } }

        /// <summary>
        /// 属性改变时的事件
        /// </summary>
        public event Action<object, string, object> OnPropertyValueChangedEvent;
        #endregion

        #region MD5
        /// <summary>
        /// 实现MD5值的生成算法
        /// </summary>
        /// <returns></returns>
        public virtual string BuildMd5()
        {
            return ExpressionHelper.Md5(this, typeof(DisplayAttribute));
        }
        #endregion

        #region 属性监听

        /// <summary>
        /// 使用AOP实现监听属性的变化，在变化时将数据更新到数据库中
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <param name="propertyName"></param>
        public virtual void OnPropertyValueChanged(object propertyValue, [CallerMemberName] string propertyName = null)
        {
            OnPropertyValueChangedEvent?.Invoke(this, propertyName, propertyValue);
        }

        /// <summary>
        /// 重新计算数据，如MD5值
        /// </summary>
        public virtual void Recalculate()
        {
            _md5 = BuildMd5();
        }
        #endregion
    }
}
