using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.ViewDomain.VModel.DevHomePage
{
    public class DeviceModel : NotifyPropertyBase
    {
        #region 设备名 

        private string _devName;
        /// <summary>
        /// 设备名
        /// </summary>
        public string DevName
        {
            get
            {
                return this._devName;
            }

            set
            {
                this._devName = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region ROOT

        private bool _hasRoot;
        /// <summary>
        /// 是否有ROOT状态
        /// </summary>
        public bool HasRoot
        {
            get
            {
                return this._hasRoot;
            }

            set
            {
                this._hasRoot = value;
                base.OnPropertyChanged();
            }
        }

        private bool _isRoot;
        /// <summary>
        /// 是否ROOT
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return this._isRoot;
            }

            set
            {
                this._isRoot = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 型号

        private string _devModel;
        /// <summary>
        /// 型号
        /// </summary>
        public string DevModel
        {
            get
            {
                return this._devModel;
            }

            set
            {
                this._devModel = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 系统

        private string _system;
        /// <summary>
        /// 系统
        /// </summary>
        public string System
        {
            get
            {
                return this._system;
            }

            set
            {
                this._system = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region IMEI    

        private string _imei;

        public string IMEI
        {
            get
            {
                return this._imei;
            }

            set
            {
                this._imei = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 序列号

        private string _serialNumber;
        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNumber
        {
            get
            {
                return this._serialNumber;
            }

            set
            {
                this._serialNumber = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region IDevice

        private IDevice _iDevSource;

        public IDevice IDevSource
        {
            get
            {
                return this._iDevSource;
            }

            set
            {
                this._iDevSource = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
    }
}
