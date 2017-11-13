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
    public abstract class SingleTaskReporterBase : IAsyncTaskProgress
    {
        #region Event

        #region ProgressChanged

        /// <summary>
        /// 任务进度改变事件。
        /// </summary>
        public event EventHandler<TaskProgressEventArgs> ProgressChanged;

        /// <summary>
        /// 触发ProgressChanged事件。
        /// </summary>
        /// <param name="args">事件参数。。</param>
        protected void OnProgressChanged(TaskProgressEventArgs args)
        {
            ProgressChanged?.Invoke(this, args);
        }

        #endregion

        #region Ternimate

        /// <summary>
        /// 任务结束事件。
        /// </summary>
        public event EventHandler<TaskTerminateEventArgs> Ternimated;

        /// <summary>
        /// 触发Ternimate事件。
        /// </summary>
        /// <param name="args">事件参数。。</param>
        protected void OnTernimate(TaskTerminateEventArgs args)
        {
            Ternimated?.Invoke(this, args);
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 ProgressReportBase 实例。
        /// </summary>
        protected SingleTaskReporterBase()
        {
            Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 当前状态。
        /// </summary>
        public TaskState State { get; protected set; }

        /// <summary>
        /// 当前进度。
        /// </summary>
        public Double Progress { get; protected set; }

        /// <summary>
        /// 标识。
        /// </summary>
        public String Id { get; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 报告开始。
        /// </summary>
        ///<param name="message">消息。</param>
        public abstract void Start(String message = null);

        /// <summary>
        /// 报告进度。
        /// </summary>
        /// <param name="progress">进度。</param>
        /// <param name="message">消息。</param>
        public abstract void ChangeProgress(Double progress, String message = null);

        /// <summary>
        /// 报告完成。
        /// </summary>
        public abstract void Finish();

        /// <summary>
        /// 报告停止。
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        /// <param name="message">消息。</param>
        public abstract void Defeat(Exception ex, String message = null);

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="errorMessage">错误消息。</param>
        /// <param name="message">消息。</param>
        public void Defeat(String errorMessage, String message = null)
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
