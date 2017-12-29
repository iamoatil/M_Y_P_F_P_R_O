using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 任务进度报告器。
    /// </summary>
    public class DefaultTaskReporter : TaskReporterBase
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 ProgressReporter 实例。
        /// </summary>
        /// <param name="taskId">任务Id。</param>
        public DefaultTaskReporter(String taskId)
            : base(taskId)
        {
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 报告正在等待被执行。
        /// </summary>
        ///<param name="message">消息。</param>
        public override void Wait(String message = null)
        {
            if (State != TaskState.Idle) return;
            State = TaskState.Waiting;
            OnTaskStateChanged(new TaskStateChangedEventArgs(TaskId, TaskState.Idle, TaskState.Waiting, message));
        }

        /// <summary>
        /// 报告开始。
        /// </summary>
        ///<param name="message">消息。</param>
        public override void Start(String message = null)
        {
            if (State != TaskState.Waiting) return;
            State = TaskState.Starting;
            OnTaskStateChanged(new TaskStateChangedEventArgs(TaskId, TaskState.Waiting, TaskState.Starting, message));
        }

        /// <summary>
        /// 报告进度。
        /// </summary>
        /// <param name="progress">进度。</param>
        /// <param name="message">消息。</param>
        public override void ChangeProgress(Double progress, String message = null)
        {
            if (IsRuning)
            {
                State = TaskState.Running;
                Progress = progress;
                OnProgressChanged(new TaskProgressChangedEventArgs(TaskId, progress, message));
            }
        }

        /// <summary>
        /// 报告正在取消。
        /// </summary>
        /// <param name="message">消息。</param>
        public override void Cancelling(String message = null)
        {
            if (IsNotStoppedAndIdle)
            {
                TaskState oldState = State;
                State = TaskState.IsCancellationRequest;
                OnTaskStateChanged(new TaskStateChangedEventArgs(TaskId, oldState, TaskState.IsCancellationRequest, message));
            }
        }

        /// <summary>
        /// 报告完成。
        /// </summary>
        /// <param name="isCompleted">是完成还是取消。</param>
        /// <param name="message">消息。</param>
        public override void Finish(Boolean isCompleted, String message = null)
        {
            if (!IsNotStoppedAndIdle) return;
            State = isCompleted ? TaskState.Completed : TaskState.Cancelled;
            Progress = 100;
            OnTerminated(new TaskTerminateEventArgs(TaskId, isCompleted, message));
        }

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        /// <param name="message">消息。</param>
        public override void Defeat(Exception ex, String message = null)
        {
            if (!IsNotStoppedAndIdle) return;
            State = TaskState.Failed;
            OnTerminated(new TaskTerminateEventArgs(TaskId, ex, message));
        }

        /// <summary>
        /// 重置。
        /// </summary>
        public override void Reset()
        {
            //枚举值高字节如果为0x02，表示结束
            if (((Int32)State & 0xFF00) != 0x0200)
            {
                return;
            }
            State = TaskState.Idle;
            Progress = 0;
        }

        #endregion

        #endregion
    }
}
