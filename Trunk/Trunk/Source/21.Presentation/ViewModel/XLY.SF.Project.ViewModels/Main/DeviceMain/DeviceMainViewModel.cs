using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.VModel.DevHomePage;

namespace XLY.SF.Project.ViewModels.Main.DeviceMain
{
    [Export(ExportKeys.DeviceMainViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceMainViewModel : ViewModelBase
    {
        #region Properties

        #region 设备信息

        private DeviceModel _curDevModel;
        /// <summary>
        /// 当前设备信息
        /// </summary>
        public DeviceModel CurDevModel
        {
            get
            {
                return _curDevModel;
            }
            set
            {
                _curDevModel = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 当前显示的界面

        private UcViewBase _curDeviceView;

        /// <summary>
        /// 当前设备显示的界面
        /// </summary>
        public UcViewBase CurDeviceView
        {
            get
            {
                return _curDeviceView;
            }
            set
            {
                _curDeviceView = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        protected override void LoadCore(object parameters)
        {
            if (parameters != null)
            {
                var _curDevice = parameters as IDevice;
                if (_curDevice == null)
                    throw new NullReferenceException(string.Format("当前设备为NULL"));
                CreateDeviceByType(_curDevice);
            }

            #region 测试代码

            CurDevModel = new PhoneDevModel()
            {
                Name = "测试阿斯蒂芬",
                IsAndroid = true,
                IsRoot = true
            };

            #endregion




            //首次加载使用设备首页
            CurDeviceView = IocManagerSingle.Instance.GetViewPart(ExportKeys.DeviceHomePageView);
            CurDeviceView.DataSource.LoadViewModel(CurDevModel);
        }

        #region Tools

        #region 设备类型转换

        /// <summary>
        /// 根据类型获取对应的设备信息
        /// </summary>
        /// <param name="idev"></param>
        private void CreateDeviceByType(IDevice idev)
        {
            switch (idev.DeviceType)
            {
                case EnumDeviceType.None:
                    break;
                case EnumDeviceType.Phone:
                    CurDevModel = GetPhoneDevice(idev);
                    break;
                case EnumDeviceType.Chip:
                    break;
                case EnumDeviceType.Disk:
                    CurDevModel = GetLocalFileDevice(idev);
                    break;
                case EnumDeviceType.SDCard:
                    CurDevModel = GetMemoryCardDevice(idev);
                    break;
                case EnumDeviceType.SIM:
                    break;
            }
        }

        private DeviceModel GetPhoneDevice(IDevice idev)
        {
            var tmpDev = idev as XLY.SF.Project.Domains.Device;
            DeviceModel targetDev = new PhoneDevModel()
            {
                Name = "测试阿斯蒂芬",
                DevModel = tmpDev.Model,
                IMEI = tmpDev.IMEI,
                IsRoot = tmpDev.IsRoot,
                SerialNumber = tmpDev.SerialNumber,
                System = string.Format("{0}{1}", tmpDev.OSType, tmpDev.OSVersion),
                IsAndroid = tmpDev.OSType == EnumOSType.Android,



                DeviceTotalSize = 50,
                UsedTotalSizeOfDevice = 12,
                UnusedTotalSizeOfDevice = 50 - 12,
                SDCardTotalSize = 100,
                UsedTotalSizeOfSD = 20,
                UnusedTotalSizeOfSD = 100 - 20,






                IDevSource = idev
            };
            return targetDev;
        }

        private DeviceModel GetLocalFileDevice(IDevice idev)
        {
            var lfDev = idev as XLY.SF.Project.Domains.LocalFileDevice;
            DeviceModel targetDev = new LocalFileDevModel()
            {
                Name = "测试阿斯蒂芬",
                FilePath = lfDev.PathName,
                FileTypeName = "默认类型",
                IDevSource = idev
            };
            return targetDev;
        }

        private DeviceModel GetMemoryCardDevice(IDevice idev)
        {
            var sdCDev = idev as XLY.SF.Project.Domains.SDCardDevice;
            DeviceModel targetDev = new MemoryCardDevModel()
            {
                Name = "测试阿斯蒂芬",
                MemoryCardTypeName = "",
                Number = sdCDev.DiskNumber.ToString(),
                IDevSource = idev
            };
            return targetDev;
        }

        #endregion

        #endregion
    }
}
