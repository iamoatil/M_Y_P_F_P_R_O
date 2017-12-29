using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 提供任务进度报告的基类。
    /// </summary>
    public abstract class TaskReporterBase : AsyncTaskProgressBase, ITaskProgressReporter
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 ProgressReportBase 实例。
        /// </summary>
        /// <param name="taskId">任务Id。</param>
        protected TaskReporterBase(String taskId)
        {
            TaskId = taskId ?? throw new ArgumentNullException(nameof(taskId));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 状态。
        /// </summary>
        public TaskState State { get; protected set; }

        /// <summary>
        /// 进度。
        /// </summary>
        public Double Progress { get; protected set; }

        /// <summary>
        /// Id。
        /// </summary>
        public String TaskId { get; }

        /// <summary>
        /// 任务没有完成也没有处于空闲状态。
        /// </summary>
        public Boolean IsNotStoppedAndIdle
        {
            get
            {
                TaskState state = State;
                if (state == TaskState.Idle) return false;
                return (((Int32)state) & 0xFF00) != 0x0200;
            }
        }

        /// <summary>
        /// 任务是否正在执行，不包括Waiting状态。
        /// </summary>
        public Boolean IsRuning => ((Int32)State & 0xFF00) == 0x0100;

        /// <summary>
        /// 任务是否已经停止。
        /// </summary>
        public Boolean IsStopped => ((Int32)State & 0xFF00) == 0x0200;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 报告正在等待被执行。
        /// </summary>
        ///<param name="message">消息。</param>
        public abstract void Wait(String message);

        /// <summary>
        /// 报告开始。
        /// </summary>
        ///<param name="message">消息。</param>
        public abstract void Start(String message);

        /// <summary>
        /// 报告进度。
        /// </summary>
        /// <param name="progress">进度。</param>
        /// <param name="message">消息。</param>
        public abstract void ChangeProgress(Double progress, String message);

        /// <summary>
        /// 报告完成。
        /// </summary>
        /// <param name="isCompleted">是完成还是取消。</param>
        /// <param name="message">消息。</param>
        public abstract void Finish(Boolean isCompleted, String message);

        /// <summary>
        /// 报告正在取消。
        /// </summary>
        /// <param name="message">消息。</param>
        public abstract void Cancelling(String message);

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        /// <param name="message">消息。</param>
        public abstract void Defeat(Exception ex, String message);

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="errorMessage">错误消息。</param>
        /// <param name="message">消息。</param>
        public void Defeat(String errorMessage, String message)
        {
            Defeat(new Exception(errorMessage), message);
        }

        /// <summary>
        /// 重置。
        /// </summary>
        public abstract void Reset();

        #endregion

        #endregion
    }
}
