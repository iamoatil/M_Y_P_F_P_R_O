using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 系统使用的消息码。
    /// </summary>
    public enum SystemMessageCode
    {
        /// <summary>
        /// 完成事件的消息码。
        /// </summary>
        CompletedEvent=-1,
        /// <summary>
        /// 失败事件的消息码。
        /// </summary>
        FailedEvent = -2,
        /// <summary>
        /// 进度改变事件的消息码。
        /// </summary>
        ProgressChangedEvent = -3,
    }
}
