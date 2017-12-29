using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    /// <summary>
    /// 任务进度报告器。
    /// </summary>
    public interface ITaskProgressReporter : IAsyncTaskProgress
    {
        #region Properties

        /// <summary>
        /// 状态。
        /// </summary>
        TaskState State { get; }

        /// <summary>
        /// 标志任务当前等待被执行或在执行中。
        /// </summary>
        Boolean IsNotStoppedAndIdle { get; }

        /// <summary>
        /// 标志任务当前正在执行。
        /// </summary>
        Boolean IsRuning { get; }

        /// <summary>
        /// 标志任务当前已经停止。
        /// </summary>
        Boolean IsStopped { get; }

        /// <summary>
        /// 进度。
        /// </summary>
        Double Progress { get; }

        /// <summary>
        /// Id。
        /// </summary>
        String TaskId { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 报告正在等待被执行。
        /// </summary>
        ///<param name="message">消息。</param>
        void Wait(String message = null);

        /// <summary>
        /// 报告开始。
        /// </summary>
        ///<param name="message">消息。</param>
        void Start(String message = null);

        /// <summary>
        /// 报告进度。
        /// </summary>
        /// <param name="progress">进度。</param>
        /// <param name="message">消息。</param>
        void ChangeProgress(Double progress, String message = null);

        /// <summary>
        /// 报告进度。
        /// </summary>
        /// <param name="isCompleted">是完成还是取消。</param>
        /// <param name="message">消息。</param>
        void Finish(Boolean isCompleted, String message = null);

        /// <summary>
        /// 报告停止。
        /// </summary>
        /// <param name="message">消息。</param>
        void Cancelling(String message = null);

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        /// <param name="message">消息。</param>
        void Defeat(Exception ex, String message = null);

        /// <summary>
        /// 重置。
        /// </summary>
        void Reset();

        #endregion
    }
}
