using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.UsbDevice
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/20 15:03:35
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// Usb设备
    /// </summary>
    [Serializable]
    public class UsbDevice : AbstractDevice
    {
        public UsbDevice()
        {
            DeviceType = EnumDeviceType.None;
            Status = EnumDeviceStatus.Identifying;
        }

        public override string ID { get => $"未知Usb设备:vid={VID}&pid={PID}"; set => throw new NotImplementedException(); }

        public uint VID { get; set; }

        public uint PID { get; set; }

        public List<string> DeviceIDArray { get; set; }
    }
}
