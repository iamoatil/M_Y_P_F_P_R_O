using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Framework.Core.Base.MessageBase.Navigation
{
    /// <summary>
    /// 导航记录状态
    /// </summary>
    internal class NavigationLogStatus
    {
        /// <summary>
        /// 创建导航记录状态
        /// </summary>
        /// <param name="showInNewWindow">是否在新窗口显示</param>
        /// <param name="exportKey">导航的界面Key</param>
        internal NavigationLogStatus(bool showInNewWindow, string exportKey)
        {
            ShowInNewWindow = showInNewWindow;
            ExportKey = exportKey;
        }

        /// <summary>
        /// 是否在新窗口中显示
        /// </summary>
        public bool ShowInNewWindow { get; private set; }

        /// <summary>
        /// 导出的Key
        /// </summary>
        public string ExportKey { get; private set; }
    }
}
