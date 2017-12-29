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
        #region 设备信息

        #region 名称 

        private string _name;
        /// <summary>
        /// 设备名
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                this._name = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region Icon

        private string _devIcon;
        /// <summary>
        /// 图标
        /// </summary>
        public string DevIcon
        {
            get
            {
                switch (IDevSource.DeviceType)
                {
                    case EnumDeviceType.Chip:
                        _devIcon = "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/DeviceChoose_Sim.png";
                        break;
                    case EnumDeviceType.LocalFile:
                        break;
                    case EnumDeviceType.SDCard:
                        _devIcon = "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/DeviceChoose_SDCard.png";
                        break;
                    case EnumDeviceType.SIM:
                        _devIcon = "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/DeviceChoose_Sim.png";
                        break;
                    default:
                        _devIcon = "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/DeviceChoose_unknown.png";
                        break;
                }
                return this._devIcon;
            }

            private set
            {
                this._devIcon = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 是否有ROOT状态

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

            protected set
            {
                this._hasRoot = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 容量标识

        private bool _hasDeviceCSize;
        /// <summary>
        /// 是否有设备容量
        /// </summary>
        public bool HasDeviceCSize
        {
            get
            {
                return this._hasDeviceCSize;
            }

            protected set
            {
                this._hasDeviceCSize = value;
                base.OnPropertyChanged();
            }
        }

        private bool _hasSdCSize;
        /// <summary>
        /// 是否有SD卡容量
        /// </summary>
        public bool HasSdCSize
        {
            get
            {
                return this._hasSdCSize;
            }

            protected set
            {
                this._hasSdCSize = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 设备容量

        private double _deviceTotalSize;
        /// <summary>
        /// 设备容量总数
        /// </summary>
        public double DeviceTotalSize
        {
            get
            {
                return this._deviceTotalSize;
            }

            set
            {
                this._deviceTotalSize = value;
                base.OnPropertyChanged();
            }
        }

        private double _usedTotalSizeOfDevice;
        /// <summary>
        /// 已用总和
        /// </summary>
        public double UsedTotalSizeOfDevice
        {
            get
            {
                return DeviceTotalSize - UnusedTotalSizeOfDevice;
            }

            set
            {
                _usedTotalSizeOfDevice = value;
            }
        }

        private double _unusedTotalSizeOfDevice;
        /// <summary>
        /// 未使用总和
        /// </summary>
        public double UnusedTotalSizeOfDevice
        {
            get
            {
                return this._unusedTotalSizeOfDevice;
            }

            set
            {
                this._unusedTotalSizeOfDevice = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 是否在线

        private bool _onlineStatus;
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool OnlineStatus
        {
            get
            {
                return this._onlineStatus;
            }

            set
            {
                this._onlineStatus = value;
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

        #region DeviceExtractionAdorner

        /// <summary>
        /// DeviceExtractionAdorner
        /// </summary>
        public object DeviceExtractionAdorner { get; set; }

        #endregion

        #endregion

        #region 录入信息-

        private DevHomePageEditItemModel _devHomePageEditItems;
        /// <summary>
        /// 设备首页编辑项信息
        /// </summary>
        public DevHomePageEditItemModel DevHomePageEditItemInfo
        {
            get
            {
                return this._devHomePageEditItems;
            }

            set
            {
                this._devHomePageEditItems = value;
                base.OnPropertyChanged();
            }
        }

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

        private bool _desciption;
        /// <summary>
        /// 说明
        /// </summary>
        public bool Desciption
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

        #endregion
    }
}
