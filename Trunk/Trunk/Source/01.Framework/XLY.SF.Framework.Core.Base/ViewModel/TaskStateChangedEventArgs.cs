using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 异步任务状态改变事件参数。
    /// </summary>
    public class TaskStateChangedEventArgs : TaskEventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 TaskStateChangedEventArgs 实例。
        /// </summary>
        /// <param name="taskId">任务标识。</param>
        /// <param name="oldState">任务之前的状态。</param>
        /// <param name="newState">任务当前的。</param>
        /// <param name="message">消息。</param>
        public TaskStateChangedEventArgs(String taskId, TaskState oldState, TaskState newState, String message)
            : base(taskId, message)
        {
            OldState = oldState;
            NewState = newState;
        }

        private TaskStateChangedEventArgs() : base("") { }

        #endregion

        #region Properties

        /// <summary>
        /// 任务之前的状态。
        /// </summary>
        public TaskState OldState { get; }

        /// <summary>
        /// 任务当前的。
        /// </summary>
        public TaskState NewState { get; }

        #endregion
    }
}
