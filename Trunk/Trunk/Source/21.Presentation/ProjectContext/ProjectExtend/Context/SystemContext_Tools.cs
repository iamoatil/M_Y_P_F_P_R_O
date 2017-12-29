using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.ViewDomain.Model;

namespace ProjectExtend.Context
{
    public partial class SystemContext
    {
        #region 创建

        /// <summary>
        /// 获取操作截图保存的文件名称
        /// </summary>
        /// <returns></returns>
        private string GetOperationImageSaveName()
        {
            return string.Format("Op_{0:yyyyMMdd HHmmss}.png", DateTime.Now);
        }

        #endregion

        #region 系统相关

        /// <summary>
        /// 加载当前DPI
        /// </summary>
        private void LoadCurrentScreenDPI()
        {
            using (ManagementClass mc = new ManagementClass("Win32_DesktopMonitor"))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    int PixelsPerXLogicalInch = 0; // dpi for x
                    int PixelsPerYLogicalInch = 0; // dpi for y

                    foreach (ManagementObject each in moc)
                    {
                        PixelsPerXLogicalInch = int.Parse((each.Properties["PixelsPerXLogicalInch"].Value.ToString()));
                        PixelsPerYLogicalInch = int.Parse((each.Properties["PixelsPerYLogicalInch"].Value.ToString()));
                    }

                    //设置当前DPI
                    this.DpiX = PixelsPerXLogicalInch;
                    this.DpiY = PixelsPerYLogicalInch;
                }
            }
        }

        #endregion

        #region 从配置文件读取对应信息

        /// <summary>
        /// 从配置文件读取对应信息
        /// </summary>
        private bool LoadConfig()
        {
            return true;
        }

        #endregion        
    }
}
