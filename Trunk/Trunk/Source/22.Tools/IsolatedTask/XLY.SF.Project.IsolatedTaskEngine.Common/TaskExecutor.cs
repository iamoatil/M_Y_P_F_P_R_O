using log4net;
using System;
using System.IO;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 任务执行器。根据命令执行特定的业务。
    /// </summary>
    public abstract class TaskExecutor : ITaskExecutor
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 CommandActivator 实例。
        /// </summary>
        protected TaskExecutor()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 请求引擎将消息发送给客户端。
        /// </summary>
        Action<Message> ITaskExecutor.RequestSendMessage { get; set; }

        /// <summary>
        /// 日志记录器。
        /// </summary>
        public ILog Logger { get; set; }

        /// <summary>
        /// 激活器唯一标识。
        /// </summary>
        public Guid ExecurtorId { get; } = Guid.NewGuid();

        /// <summary>
        /// 任务执行器是否已经关闭。
        /// </summary>
        public Boolean IsClosed { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 启动。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        public abstract Boolean Launch();

        /// <summary>
        /// 关闭任务执行器并释放资源。
        /// </summary>
        public void Close()
        {
            if (IsClosed) return;
            CloseCore();
            IsClosed = true;
        }

        /// <summary>
        /// 收到客户进程消息时执行该方法。
        /// </summary>
        /// <param name="message">消息。</param>
        void ITaskExecutor.Receive(Message message)
        {
            OnReceive(message);
        }

        #endregion

        #region Protected

        /// <summary>
        /// 关闭任务执行器并释放资源。
        /// </summary>
        protected abstract void CloseCore();

        /// <summary>
        /// 收到客户进程消息时执行该方法。
        /// </summary>
        /// <param name="message">消息。</param>
        protected abstract void OnReceive(Message message);

        /// <summary>
        /// 将消息发送给客户端。
        /// </summary>
        /// <param name="message">消息。</param>
        protected void Send(Message message)
        {
            ((ITaskExecutor)this).RequestSendMessage.Invoke(message);
        }

        /// <summary>
        /// 通知客户端任务已经结束。
        /// </summary>
        /// <param name="args">任务结束事件参数。</param>
        protected void OnExecutorTerminate(TaskTerminateEventArgs args)
        {
            Logger.Info($"Task terminate:{ExecurtorId}");
            Message message = Message.CreateSystemMessage((Int32)SystemMessageCode.TaskTerminateEvent, args);
            Send(message);
        }

        #endregion

        #endregion
    }
}
