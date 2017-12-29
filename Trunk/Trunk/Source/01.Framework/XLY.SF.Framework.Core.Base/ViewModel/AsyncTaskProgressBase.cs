using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 异步任务进度报告器基类。
    /// </summary>
    [Serializable]
    public class AsyncTaskProgressBase : IAsyncTaskProgress
    {
        #region Event

        #region ProgressChanged

        /// <summary>
        /// 任务进度改变事件。
        /// </summary>
        public event EventHandler<TaskProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// 触发ProgressChanged事件。
        /// </summary>
        /// <param name="args">事件参数。。</param>
        protected void OnProgressChanged(TaskProgressChangedEventArgs args)
        {
            ProgressChanged?.Invoke(this, args);
        }

        #endregion

        #region TaskStateChanged

        /// <summary>
        /// 任务状态改变事件。
        /// </summary>
        public event EventHandler<TaskStateChangedEventArgs> TaskStateChanged;

        /// <summary>
        /// 触发TaskStateChanged事件。
        /// </summary>
        /// <param name="args">事件参数。</param>
        protected void OnTaskStateChanged(TaskStateChangedEventArgs args)
        {
            TaskStateChanged?.Invoke(this, args);
        }

        #endregion

        #region Terminated

        /// <summary>
        /// 任务结束事件。
        /// </summary>
        public event EventHandler<TaskTerminateEventArgs> Terminated;

        /// <summary>
        /// 触发任务结束事件。
        /// </summary>
        /// <param name="args">事件参数。。</param>
        protected void OnTerminated(TaskTerminateEventArgs args)
        {
            Terminated?.Invoke(this, args);
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 AsyncTaskProgressBase 实例。
        /// </summary>
        protected AsyncTaskProgressBase() { }

        #endregion
    }
}
