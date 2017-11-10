using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.VModel.DevHomePage;

namespace XLY.SF.Project.ViewModels.Main.DeviceMain
{
    [Export(ExportKeys.DeviceMainViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceMainViewModel: ViewModelBase
    {
        #region Properties

        /// <summary>
        /// 当前设备信息
        /// </summary>
        public DeviceModel CurDevModel { get; private set; }

        #endregion

        protected override void LoadCore(object parameters)
        {
            if (parameters != null)
            {
                var _curDevice = parameters as IDevice;
                if (_curDevice == null)
                    throw new NullReferenceException(string.Format("当前设备为NULL"));
                GetDeviceByType(_curDevice);
            }
        }

        #region Tools

        /// <summary>
        /// 根据类型获取对应的设备信息
        /// </summary>
        /// <param name="idev"></param>
        private void GetDeviceByType(IDevice idev)
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
                    break;
                case EnumDeviceType.SDCard:
                    break;
                case EnumDeviceType.SIM:
                    break;
            }
        }

        private DeviceModel GetPhoneDevice(IDevice idev)
        {
            var tmpDev = idev as XLY.SF.Project.Domains.Device;
            DeviceModel targetDev = new DeviceModel()
            {
                DevModel = tmpDev.Model,
                DevName = tmpDev.Name,
                HasRoot = true,
                IMEI = tmpDev.IMEI,
                IsRoot = tmpDev.IsRoot,
                SerialNumber = tmpDev.SerialNumber,
                System = string.Format("{0}{1}", tmpDev.OSType, tmpDev.OSVersion),
                IDevSource = idev
            };
            return targetDev;
        }

        #endregion
    }
}
