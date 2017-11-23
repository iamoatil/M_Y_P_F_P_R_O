using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ProxyService;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewModels.Main.CaseManagement;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.ViewModels.Device.DeviceSelectViewModel
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/17 14:52:21
* ==============================================================================*/

namespace XLY.SF.Project.ViewModels.Device
{
    /// <summary>
    /// 数据源选择界面VM
    /// </summary>
    [Export(ExportKeys.DeviceSelectViewModel, typeof(ViewModelBase))]
    public class DeviceSelectViewModel : ViewModelBase
    {
        public DeviceSelectViewModel()
        {
            SelectFileCommond = new RelayCommand(DoSelectFileCommond);
            SelectFolderCommond= new RelayCommand(DoSelectFolderCommond);
        }

        #region 属性

        #region 设备列表
        private ObservableCollection<IDevice> _devices = new ObservableCollection<IDevice>();

        /// <summary>
        /// 设备列表
        /// </summary>	
        public ObservableCollection<IDevice> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 当前选择的设备
        private IDevice _selectDevice;

        /// <summary>
        /// 当前选择的设备
        /// </summary>	
        public IDevice SelectDevice
        {
            get { return _selectDevice; }
            set
            {
                _selectDevice = value;
                OnPropertyChanged();

                if(_selectDevice != null)
                {
                    DoSelectDeviceCommond();
                }
            }
        }

        #endregion

        #region 当前是否连接有设备
        private bool _hasDevice;

        /// <summary>
        /// 当前是否连接有设备
        /// </summary>	
        public bool HasDevice
        {
            get { return _hasDevice; }
            set
            {
                _hasDevice = value;
                OnPropertyChanged();
            }
        }
        #endregion


        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        [Import(typeof(IPopupWindowService))]
        private IPopupWindowService _fileDlg
        {
            get;
            set;
        }
        #endregion

        #region Commond

        public RelayCommand SelectFileCommond { get; set; }
        /// <summary>
        /// 选择单个文件
        /// </summary>
        private void DoSelectFileCommond()
        {
            LocalFileDevice file = _fileDlg.ShowDialogWindow(ExportKeys.DeviceSelectFileView, null) as LocalFileDevice;
            if(file != null)
            {
                CreateDevice(file);
            }
        }

        public RelayCommand SelectFolderCommond { get; set; }
        /// <summary>
        /// 选择文件夹
        /// </summary>
        private void DoSelectFolderCommond()
        {
            string path = _fileDlg.OpenFileDialog();
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            LocalFileDevice file = new LocalFileDevice() { IsDirectory = true, PathName = path };
            CreateDevice(file);
        }

        /// <summary>
        /// 选择了某个设备
        /// </summary>
        /// <returns></returns>
        private string DoSelectDeviceCommond()
        {
            CreateDevice(SelectDevice);
            return "创建设备成功";
        }
        #endregion

        #region 事件

        protected override void Closed()
        {
            ProxyFactory.DeviceMonitor.OnDeviceConnected -= DeviceMonitor_OnDeviceConnected;
        }

        protected override void InitLoad(object parameters)
        {
            Devices.Clear();
            Devices.AddRange(ProxyFactory.DeviceMonitor.GetCurConnectedDevices());
            HasDevice = Devices.Count > 0;
            ProxyFactory.DeviceMonitor.OnDeviceConnected += DeviceMonitor_OnDeviceConnected;

        }

        /// <summary>
        /// 设备状态改变事件
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="isOnline"></param>
        private void DeviceMonitor_OnDeviceConnected(Domains.IDevice dev, bool isOnline)
        {
            SystemContext.Instance.AsyncOperation.Post(t =>
            {
                if (isOnline)
                {
                    Devices.Add(dev);
                }
                else
                {
                    Devices.Remove(Devices.FirstOrDefault(d => d.Equals(dev)));
                }
                HasDevice = Devices.Count > 0;
            }, null);
        }
        #endregion

        #region 方法

        /// <summary>
        /// 创建一个设备，如果设备已经存在，则跳转到该设备
        /// </summary>
        /// <param name="device"></param>
        private void CreateDevice(IDevice device)
        {
            if(device.DeviceType == EnumDeviceType.None)
            {
                return;
            }
            DeviceExtractionAdorner dea = new DeviceExtractionAdorner();
            var ca = SystemContext.Instance.CurrentCase;
            var dev = ca.DeviceExtractions.FirstOrDefault(d => d[DeviceExternsion.XLY_IdKey] == device.ID);
            if (dev == null)     //不存在则创建新设备
            {
                dev = SystemContext.Instance.CurrentCase.CreateDeviceExtraction(device.Name, device.DeviceType.ToString());
                dea.Target = dev;
                dea.Device = device;
                dea.Save();
            }
            else
            {
                dea.Target = dev;
            }
            //跳转到设备
            MessageAggregation.SendGeneralMsg(new GeneralArgs<DeviceExtractionAdorner>(ExportKeys.DeviceAddedMsg) { Parameters = dea });
        }

        #endregion
    }
}
