using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 任务结束事件参数。
    /// </summary>
    public class TaskTerminateEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 TaskFailedEventArgs 实例。
        /// </summary>
        /// <param name="message">消息。</param>
        public TaskTerminateEventArgs(String message = null)
        {
            Message = message;
        }

        /// <summary>
        /// 初始化类型 TaskFailedEventArgs 实例。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        public TaskTerminateEventArgs(Exception ex)
        {
            Exception = ex;
        }

        /// <summary>
        /// 初始化类型 TaskFailedEventArgs 实例。
        /// </summary>
        private TaskTerminateEventArgs()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 消息。
        /// </summary>
        public String Message { get; private set; }

        /// <summary>
        /// 异常信息。
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 是否失败。
        /// </summary>
        public Boolean IsFailed => Exception != null;

        #endregion
    }
}
