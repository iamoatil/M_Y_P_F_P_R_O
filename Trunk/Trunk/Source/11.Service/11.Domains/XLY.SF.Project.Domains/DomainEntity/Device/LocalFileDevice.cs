using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.LocalFileDevice
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/20 15:03:35
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 本地文件和文件夹设备
    /// </summary>
    [Serializable]
    public class LocalFileDevice : AbstractDevice
    {
        public LocalFileDevice()
        {
            DeviceType = EnumDeviceType.Disk;
            Status = EnumDeviceStatus.Online;
        }

        public override string ID { get => $"本地文件设备:IsDirectory={IsDirectory}&PathName={PathName}"; set => throw new NotImplementedException(); }

        /// <summary>
        /// 文件夹则为true，文件为false
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// 选择的全路径名
        /// </summary>
        public string PathName { get; set; }

        /// <summary>
        /// 支持的数据类型
        /// </summary>
        public EnumOSType OSType { get; set; }

        /// <summary>
        /// 芯片类型，山寨机数据有效
        /// </summary>
        public FlshType CottageFlshType { get; set; }

        /// <summary>
        /// 设备类型，山寨机数据有效
        /// </summary>
        public DevType CottageDevType { get; set; }

    }
}
