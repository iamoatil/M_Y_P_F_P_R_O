using System;
using XLY.SF.Framework.Core.Base.ViewModel;

/* ==============================================================================
* Description：IAsyncResult  
* Author     ：Fhjun
* Create Date：2017/4/11 11:16:14
* ==============================================================================*/

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    /// <summary>
    /// 异步任务通知。
    /// </summary>
    public interface IAsyncTaskProgress
    {
        #region Events

        /// <summary>
        /// 任务进度改变事件。
        /// </summary>
        event EventHandler<TaskProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// 任务状态改变事件。
        /// </summary>
        event EventHandler<TaskStateChangedEventArgs> TaskStateChanged;

        /// <summary>
        /// 任务结束事件。
        /// </summary>
        event EventHandler<TaskTerminateEventArgs> Terminated;

        #endregion
    }

    /// <summary>
    /// 默认实现的异步通知类
    /// </summary>
    [Serializable]
    public class DefaultAsyncTaskProgress : AsyncTaskProgressBase
    {
        /// <summary>
        /// 通知进度
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="progress">进度</param>
        /// <param name="msg">消息</param>
        public void OnProgress(string taskid, double progress, string msg = null)
        {
            OnProgressChanged(new TaskProgressChangedEventArgs(taskid, progress, msg));
        }

        /// <summary>
        /// 任务结束
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="isCompleted"></param>
        /// <param name="message"></param>
        public void OnTaskStateChanged(string taskid, TaskState oldState, TaskState newState, String message = null)
        {
            OnTaskStateChanged(new TaskStateChangedEventArgs(taskid, oldState, newState, message));
        }

        /// <summary>
        /// 任务结束
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="isCompleted"></param>
        /// <param name="message"></param>
        public void OnTerminated(string taskid, Boolean isCompleted, String message = null)
        {
            OnTerminated(new TaskTerminateEventArgs(taskid, isCompleted, message));
        }

    }
}
