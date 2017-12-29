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
        /// 任务结束事件的消息码。
        /// </summary>
        TaskTerminateEvent = -1,
        /// <summary>
        /// 任务引擎错误事件的消息码。
        /// </summary>
        TaskEngineErrorEvent = -2,
    }
}
