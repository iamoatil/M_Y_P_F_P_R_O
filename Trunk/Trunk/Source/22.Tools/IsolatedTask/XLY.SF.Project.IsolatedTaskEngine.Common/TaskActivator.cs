using System;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 任务激活器。根据命令执行特定的业务。
    /// </summary>
    public abstract class TaskActivator : ITaskActivator
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 CommandActivator 实例。
        /// </summary>
        protected TaskActivator()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 请求引擎将消息发送给客户端。
        /// </summary>
        internal Action<Message> RequestSendMessageCallback { get; set; }

        /// <summary>
        /// 请求引擎终止任务并通知客户端。
        /// </summary>
        internal Action RequestTerminateTask { get; set; }

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
        /// 请求引擎将消息发送给客户端。
        /// </summary>
        /// <param name="message">消息。</param>
        protected void OnSend(Message message)
        {
            RequestSendMessageCallback.Invoke(message);
        }

        /// <summary>
        /// 通知引擎所有任务已经完成。这将导致客户端收到任务结束事件，并回收该任务激活器。
        /// </summary>
        /// <param name="args">任务结束事件参数。</param>
        protected void OnTerminateActivator(TaskOverEventArgs args)
        {
            Message message = Message.CreateSystemMessage((Int32)SystemMessageCode.TaskOverEvent, args);
            OnSend(message);
            RequestTerminateTask.Invoke();
        }

        #endregion

        #endregion
    }
}
