using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.ViewDomain.VModel.DevHomePage
{
    /// <summary>
    /// 设备首页编辑项
    /// </summary>
    public class DevHomePageEditItemModel: NotifyPropertyBase
    {
        #region 检材编号

        private string _no;
        /// <summary>
        /// 检材编号
        /// </summary>
        public string No
        {
            get
            {
                return this._no;
            }

            set
            {
                this._no = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 持有人

        private string _holder;
        /// <summary>
        /// 持有人
        /// </summary>
        public string Holder
        {
            get
            {
                return this._holder;
            }

            set
            {
                this._holder = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 证件类型

        private string _credentialsType;
        /// <summary>
        /// 证件类型
        /// </summary>
        public string CredentialsType
        {
            get
            {
                return this._credentialsType;
            }

            set
            {
                this._credentialsType = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 持有人证件号

        private string _holderCredentialsNo;
        /// <summary>
        /// 持有人证件号
        /// </summary>
        public string HolderCredentialsNo
        {
            get
            {
                return this._holderCredentialsNo;
            }

            set
            {
                this._holderCredentialsNo = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 送检人

        private string _censorshipPerson;
        /// <summary>
        /// 送检人
        /// </summary>
        public string CensorshipPerson
        {
            get
            {
                return this._censorshipPerson;
            }

            set
            {
                this._censorshipPerson = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 单位名称

        private string _unitName;
        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName
        {
            get
            {
                return this._unitName;
            }

            set
            {
                this._unitName = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 送检人证件号

        private string _censorshipPersonCredentialsNo;
        /// <summary>
        /// 送检人证件号
        /// </summary>
        public string CensorshipPersonCredentialsNo
        {
            get
            {
                return this._censorshipPersonCredentialsNo;
            }

            set
            {
                this._censorshipPersonCredentialsNo = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 采集人

        private string _operator;
        /// <summary>
        /// 采集人
        /// </summary>
        public string Operator
        {
            get
            {
                return this._operator;
            }

            set
            {
                this._operator = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 证件号

        private string _credentialsNo;
        /// <summary>
        /// 证件号
        /// </summary>
        public string CredentialsNo
        {
            get
            {
                return this._credentialsNo;
            }

            set
            {
                this._credentialsNo = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 说明

        private string _desciption;
        /// <summary>
        /// 说明
        /// </summary>
        public string Desciption
        {
            get
            {
                return this._desciption;
            }

            set
            {
                this._desciption = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
    }
}
