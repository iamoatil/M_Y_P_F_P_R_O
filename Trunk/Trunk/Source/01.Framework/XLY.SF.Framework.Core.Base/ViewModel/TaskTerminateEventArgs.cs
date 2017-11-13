using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 异步任务结束事件参数。
    /// </summary>
    public class TaskTerminateEventArgs : TaskEventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 AsyncTaskTerminateEventArgs 实例。
        /// </summary>
        /// <param name="taskId">任务标识。</param>
        /// <param name="exception">异常信息。</param>
        /// <param name="message">消息。</param>
        public TaskTerminateEventArgs(String taskId, Exception exception, String message)
            : base(taskId, message)
        {
            Exception = exception ?? throw new ArgumentNullException("exception");
        }

        /// <summary>
        /// 初始化类型 AsyncTaskTerminateEventArgs 实例。
        /// </summary>
        /// <param name="taskId">任务标识。</param>
        /// <param name="isCompleted">是否完成。如果为false，表示在未完成的情况下停止任务。</param>
        public TaskTerminateEventArgs(String taskId, Boolean isCompleted)
            : base(taskId)
        {
            if (isCompleted)
            {
                IsCompleted = true;
            }
            else
            {
                IsStopped = true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 是否完成。
        /// </summary>
        public Boolean IsCompleted { get; }

        /// <summary>
        /// 是否停止。
        /// </summary>
        public Boolean IsStopped { get; }

        /// <summary>
        /// 异常信息。
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 是否失败。
        /// </summary>
        public Boolean IsFailed => Exception != null;

        #endregion
    }
}
