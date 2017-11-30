using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewDomain.VModel.DevHomePage
{

    /// <summary>
    /// 手机设备
    /// </summary>
    public class PhoneDevModel : DeviceModel
    {
        public PhoneDevModel()
        {
            HasDeviceCSize = true;
            HasSdCSize = true;
            HasRoot = true;
        }

        #region Icon

        private string _devIcon;
        /// <summary>
        /// 图标
        /// </summary>
        public new string DevIcon
        {
            get
            {
                _devIcon = IsAndroid ? "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/DeviceChoose_Android.png" :
                    "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/DeviceChoose_IOS.png";
                return this._devIcon;
            }

            private set
            {
                this._devIcon = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 是否为Android，否为IOS

        private bool _isAndroid;
        /// <summary>
        /// 是否为安卓，否者为IOS【只为界面绑定显示用】
        /// </summary>
        public bool IsAndroid
        {
            get
            {
                return this._isAndroid;
            }

            set
            {
                this._isAndroid = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region ROOT

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


        private string rootDesc;
        /// <summary>
        /// 权限显示信息【Android是否Root，IOS是否越狱】
        /// </summary>
        public string RootDesc
        {
            get
            {
                return this.rootDesc;
            }

            set
            {
                this.rootDesc = value;
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

        #region 容量
        
        #region SD卡容量

        private double _sDCardTotalSize;
        /// <summary>
        /// SD卡容量总数
        /// </summary>
        public double SDCardTotalSize
        {
            get
            {
                return this._sDCardTotalSize;
            }

            set
            {
                this._sDCardTotalSize = value;
                base.OnPropertyChanged();
            }
        }

        private double usedTotalSizeOfSD;
        /// <summary>
        /// 已用SD卡总和
        /// </summary>
        public double UsedTotalSizeOfSD
        {
            get
            {
                return SDCardTotalSize - UnusedTotalSizeOfSD;
            }
            private set
            {
                usedTotalSizeOfSD = value;
            }
        }


        private double _unusedTotalSizeOfSD;
        /// <summary>
        /// 未使用SD卡总和
        /// </summary>
        public double UnusedTotalSizeOfSD
        {
            get
            {
                return this._unusedTotalSizeOfSD;
            }

            set
            {
                this._unusedTotalSizeOfSD = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #endregion
    }
}
