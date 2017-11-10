using System;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 任务激活器。根据命令执行特定的业务。
    /// </summary>
    public abstract class TaskActivator : ITaskActivator
    {
        #region Events

        #region TaskOver

        /// <summary>
        /// 任务结束事件。
        /// </summary>
        public event EventHandler<TaskOverEventArgs> TaskOver;

        /// <summary>
        /// 触发任务结束事件。
        /// </summary>
        protected void OnTaskOver(Boolean isCompleted)
        {
            TaskOver?.Invoke(this, new TaskOverEventArgs(isCompleted));
        }

        /// <summary>
        /// 触发任务结束事件。。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        protected void OnTaskOver(Exception ex)
        {
            TaskOver?.Invoke(this, new TaskOverEventArgs(ex));
        }

        /// <summary>
        /// 触发任务结束事件。
        /// </summary>
        /// <param name="errorMessage">错误消息。</param>
        protected void OnTaskOver(String errorMessage)
        {
            TaskOver?.Invoke(this, new TaskOverEventArgs(errorMessage));
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 CommandActivator 实例。
        /// </summary>
        protected TaskActivator()
        {
        }

        #endregion

        #region Properties

        internal Action<Message> RequestSendMessageCallback { get; set; }

        /// <summary>
        /// 对象占用的资源是否已经被释放。
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 启动。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        public abstract Boolean Launch();

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Protected

        /// <summary>
        /// 清理资源。
        /// </summary>
        /// <param name="isDisposing">如果为true表示显示清理，否则系统自动清理。</param>
        protected virtual void Dispose(Boolean isDisposing)
        {
            if (IsDisposed) return;
            IsDisposed = true;
        }

        /// <summary>
        /// 客户进程收到消息时执行该方法。
        /// </summary>
        /// <param name="message">消息。</param>
        internal protected virtual void OnReceive(Message message)
        {
        }

        /// <summary>
        /// 客户进程发送消息时执行该方法。
        /// </summary>
        /// <param name="message">消息。</param>
        protected void OnSend(Message message)
        {
            RequestSendMessageCallback?.Invoke(message);
        }

        #endregion

        #endregion
    }
}
