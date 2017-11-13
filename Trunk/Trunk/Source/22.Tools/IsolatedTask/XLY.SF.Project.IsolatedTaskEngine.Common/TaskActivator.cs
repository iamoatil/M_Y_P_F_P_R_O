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
        /// <param name="taskId">任务标识。</param>
        /// <param name="isCompleted">是否取消。如果为true表示正常完成，否表示取消。</param>
        protected void OnTaskOver(String taskId, Boolean isCompleted)
        {
            TaskOver?.Invoke(this, new TaskOverEventArgs(taskId, isCompleted));
        }

        /// <summary>
        /// 触发任务结束事件。。
        /// </summary>
        /// <param name="taskId">任务标识。</param>
        /// <param name="ex">异常信息。</param>
        protected void OnTaskOver(String taskId, Exception ex)
        {
            TaskOver?.Invoke(this, new TaskOverEventArgs(taskId, ex));
        }

        /// <summary>
        /// 触发任务结束事件。
        /// </summary>
        /// <param name="taskId">任务标识。</param>
        /// <param name="errorMessage">错误消息。</param>
        protected void OnTaskOver(String taskId, String errorMessage)
        {
            TaskOver?.Invoke(this, new TaskOverEventArgs(taskId, errorMessage));
        }

        #endregion

        #region ActivatorError

        /// <summary>
        /// 任务激活错误事件。
        /// </summary>
        public event EventHandler<ActivatorErrorEventArgs> ActivatorError;

        /// <summary>
        /// 触发任务激活器错误事件。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        protected void OnActivatorError(Exception ex)
        {
            ActivatorError?.Invoke(this, new ActivatorErrorEventArgs(ex));
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
