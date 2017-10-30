/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/14 16:10:38 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// SIM卡
    /// </summary>
    [Serializable]
    public class SIMCardDevice : AbstractDevice
    {
        public SIMCardDevice()
        {
            DeviceType = EnumDeviceType.SIM;
            Status = EnumDeviceStatus.None;
        }

        #region ID
        /// <summary>
        /// 设备ID 唯一标识符
        /// </summary>
        public override string ID { get => $"{DeviceType}:{ComNumStr}"; set => throw new NotImplementedException(); }
        #endregion

        /// <summary>
        /// COM口编号
        /// </summary>
        public string ComNumStr { get; set; }
    }
}
