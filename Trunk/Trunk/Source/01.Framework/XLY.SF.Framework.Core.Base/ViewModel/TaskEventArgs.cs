using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 异步任务事件。
    /// </summary>
    public class TaskEventArgs : EventArgs
    {
        #region Cosntructors

        /// <summary>
        /// 初始化类型 AsyncTaskEventArgs 实例。
        /// </summary>
        /// <param name="taskId">任务标识。</param>
        public TaskEventArgs(String taskId)
        {
            TaskId = taskId ?? throw new ArgumentNullException("taskId");
        }

        /// <summary>
        /// 初始化类型 AsyncTaskEventArgs 实例。
        /// </summary>
        /// <param name="taskId">任务标识。</param>
        /// <param name="message">事件消息。</param>
        public TaskEventArgs(String taskId, String message)
            : this(taskId)
        {
            Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 任务标识。
        /// </summary>
        public String TaskId { get; private set; }

        /// <summary>
        /// 消息。
        /// </summary>
        public String Message { get; private set; }

        #endregion
    }
}
