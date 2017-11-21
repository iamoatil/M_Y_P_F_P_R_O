using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.MessageBase.Navigation;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.VModel.DevHomePage;
using XLY.SF.Project.ViewModels.Main.CaseManagement;

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

        #region Commands

        /// <summary>
        /// 数据提取结果
        /// </summary>
        public ProxyRelayCommand ExtractionResultCommand { get; private set; }
        /// <summary>
        /// 跳转到设备首页
        /// </summary>
        public ProxyRelayCommand DeviceHomePageCommand { get; private set; }

        #endregion

        public DeviceMainViewModel()
        {
            base.MessageAggregation.RegisterGeneralMsg<bool>(this, GeneralKeys.ExtractTaskCompleteMsg, TaskCompleteCallback);
            ExtractionResultCommand = new ProxyRelayCommand(ExeucteExtractionResultCommand);
            DeviceHomePageCommand = new ProxyRelayCommand(ExecuteDeviceHomePageCommand);
        }

        protected override void InitLoad(object parameters)
        {
            if (parameters != null)
            {
                var _curDevice = parameters as DeviceExtractionAdorner;
                if (_curDevice == null)
                    throw new NullReferenceException(string.Format("当前设备为NULL"));
                CreateDeviceByType(_curDevice);


                //首次加载使用设备首页
                CurDeviceView = IocManagerSingle.Instance.GetViewPart(ExportKeys.DeviceHomePageView);
                CurDeviceView.DataSource.LoadViewModel(CurDevModel);
            }
        }

        #region ExecuteCommands

        private string ExecuteDeviceHomePageCommand()
        {
            var a = IocManagerSingle.Instance.GetViewPart(ExportKeys.DeviceHomePageView);
            a.DataSource.LoadViewModel(CurDevModel);

            CurDeviceView = a;
            return string.Empty;
        }

        private string ExeucteExtractionResultCommand()
        {
            var a = IocManagerSingle.Instance.GetViewPart(ExportKeys.DataDisplayView);
            var b = CurDevModel.DeviceExtractionAdorner as DeviceExtractionAdorner;
            a.DataSource.LoadViewModel(b.Target.Path);
            CurDeviceView = a;
            return string.Empty;
        }

        #endregion

        #region Tools

        #region 设备类型转换

        /// <summary>
        /// 根据类型获取对应的设备信息
        /// </summary>
        /// <param name="idev"></param>
        private void CreateDeviceByType(DeviceExtractionAdorner idev)
        {
            switch (idev.Device.DeviceType)
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

        private DeviceModel GetPhoneDevice(DeviceExtractionAdorner idev)
        {
            var tmpDev = idev.Device as XLY.SF.Project.Domains.Device;
            DeviceModel targetDev = new PhoneDevModel()
            {
                Name = idev.Name,
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





                DeviceExtractionAdorner = idev,
                IDevSource = idev.Device
            };
            return targetDev;
        }

        private DeviceModel GetLocalFileDevice(DeviceExtractionAdorner idev)
        {
            var lfDev = idev.Device as XLY.SF.Project.Domains.LocalFileDevice;
            DeviceModel targetDev = new LocalFileDevModel()
            {
                Name = idev.Name,
                FilePath = lfDev.PathName,
                FileTypeName = "默认类型",
                DeviceExtractionAdorner = idev,
                IDevSource = idev.Device
            };
            return targetDev;
        }

        private DeviceModel GetMemoryCardDevice(DeviceExtractionAdorner idev)
        {
            var sdCDev = idev.Device as XLY.SF.Project.Domains.SDCardDevice;
            DeviceModel targetDev = new MemoryCardDevModel()
            {
                Name = idev.Name,
                MemoryCardTypeName = "",
                Number = sdCDev.DiskNumber.ToString(),
                DeviceExtractionAdorner = idev,
                IDevSource = idev.Device
            };
            return targetDev;
        }

        #endregion

        #endregion

        #region 任务完成回调

        //自动提取完成
        private void TaskCompleteCallback(GeneralArgs<bool> args)
        {
            if (args.Parameters)
            {
                //跳转
                var a = IocManagerSingle.Instance.GetViewPart(ExportKeys.DataDisplayView);
                var b = CurDevModel.DeviceExtractionAdorner as DeviceExtractionAdorner;
                a.DataSource.LoadViewModel(b.Target.Path);
                CurDeviceView = a;
            }
        }

        #endregion
    }
}
