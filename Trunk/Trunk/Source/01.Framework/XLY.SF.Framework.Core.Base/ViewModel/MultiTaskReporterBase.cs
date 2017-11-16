using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 提供多任务进度报告的基类。
    /// </summary>
    public abstract class MultiTaskReporterBase
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

        #region Terminate

        /// <summary>
        /// 任务结束事件。
        /// </summary>
        public event EventHandler<TaskTerminateEventArgs> Terminate;

        /// <summary>
        /// Terminate。
        /// </summary>
        /// <param name="args">事件参数。。</param>
        protected void OnTerminate(TaskTerminateEventArgs args)
        {
            Terminate?.Invoke(this, args);
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 MultiProgressReportBase 实例。
        /// </summary>
        protected MultiTaskReporterBase()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// 当前总进度。
        /// </summary>
        public abstract Double Progress { get; }

        /// <summary>
        /// 当前总状态。
        /// </summary>
        public abstract TaskState State { get; }

        /// <summary>
        /// 获取指定任务的状态。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <returns>状态。</returns>
        public abstract TaskState this[String id] { get; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取指定任务的进度。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <returns>进度。</returns>
        public abstract Double GetProgress(String id);

        /// <summary>
        /// 报告开始。
        /// </summary>
        /// <param name="id">任务id。</param>
        ///<param name="message">消息。</param>
        public abstract void Start(String id, String message = null);

        /// <summary>
        /// 报告进度。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <param name="progress">进度。</param>
        /// <param name="message">消息。</param>
        public abstract void ChangeProgress(String id, Double progress, String message = null);

        /// <summary>
        /// 报告完成。
        /// </summary>
        /// <param name="id">任务id。</param>
        public abstract void Finish(String id);

        /// <summary>
        /// 报告停止。
        /// </summary>
        /// <param name="id">任务id。</param>
        public abstract void Stop(String id);

        /// <summary>
        /// 报告所有任务停止。
        /// </summary>
        public abstract void StopAll();

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <param name="ex">异常信息。</param>
        /// <param name="message">消息。</param>
        public abstract void Defeat(String id, Exception ex, String message = null);

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <param name="errorMessage">错误消息。</param>
        /// <param name="message">消息。</param>
        public void Defeat(String id, String errorMessage, String message = null)
        {
            Defeat(id, new Exception(errorMessage), message);
        }

        /// <summary>
        /// 重置。
        /// </summary>
        public abstract void Reset();

        #endregion

        #endregion

    }
}
