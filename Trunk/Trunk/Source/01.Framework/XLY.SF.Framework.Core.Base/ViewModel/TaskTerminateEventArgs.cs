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
        /// <param name="isCompleted">是否完成。如果为false，表示在取消任务。</param>
        /// <param name="message">消息。</param>
        public TaskTerminateEventArgs(String taskId, Boolean isCompleted, String message)
            : base(taskId, message)
        {
            if (isCompleted)
            {
                IsCompleted = true;
            }
            else
            {
                IsCancelled = true;
            }
        }

        private TaskTerminateEventArgs():base("") { }

        #endregion

        #region Properties

        /// <summary>
        /// 是否完成。
        /// </summary>
        public Boolean IsCompleted { get; private set; }

        /// <summary>
        /// 是否取消。
        /// </summary>
        public Boolean IsCancelled { get; private set; }

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
