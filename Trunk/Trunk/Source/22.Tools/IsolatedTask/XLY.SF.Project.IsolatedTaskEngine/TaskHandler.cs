using System;
using System.Threading.Tasks;
using XLY.SF.Project.IsolatedTaskEngine.Common;

namespace XLY.SF.Project.IsolatedTaskEngine
{
    /// <summary>
    /// 任务处理器。
    /// </summary>
    internal class TaskHandler : IDisposable
    {
        #region Fields

        private Boolean _isRuning;

        private readonly TaskActivator _activator;

        private readonly MessageServerTransceiver _transceiver;

        #endregion

        #region Cosntructors

        public TaskHandler(TaskManager owner)
        {
            Owner = owner;
            _transceiver = new MessageServerTransceiver(owner.Setup.TransceiverName, owner.Setup.MaxParallelTask);
            _transceiver.Disconnect += (a, b) => TerminateTask();
            TaskActivator activator = (TaskActivator)Activator.CreateInstance(owner.Setup.EntryType);
            activator.Logger = TaskEngine.Logger;
            activator.RequestSendMessageCallback = (m) => _transceiver.Send(m);
            activator.RequestTerminateTask = () => TerminateTask();
            _activator = activator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 是否启动成功。
        /// </summary>
        public Boolean IsLaunched => _isRuning;

        /// <summary>
        /// 是否已被清理。
        /// </summary>
        public Boolean IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// 拥有者。
        /// </summary>
        public TaskManager Owner { get; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 启动任务。
        /// </summary>
        public void Launch()
        {
            if (_isRuning || IsDisposed) return;
            if (_transceiver.Launch())
            {
                try
                {
                    if (_activator.Launch())
                    {
                        _isRuning = true;
                        Task.Factory.StartNew(Handle, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
                        TaskEngine.Logger.Info("Task handler launched");
                    }
                    else
                    {
                        OnActivatorError(new Exception("Launch task failed"));
                    }
                }
                catch (Exception ex)
                {
                    OnActivatorError(ex);
                }
            }
            else
            {
                TerminateTask();
            }
        }

        /// <summary>
        /// 清理资源。
        /// </summary>
        public void Dispose()
        {
            TerminateTask();
        }

        /// <summary>
        /// 关闭任务。
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        #endregion

        #region Private

        private void Handle()
        {
            Message message = null;
            while (_isRuning)
            {
                message = _transceiver.Receive();
                TaskEngine.Logger.Debug($"Received message:[Code]{message?.Code},[Token]{message?.Token}");
                if (message == null) continue;
                try
                {
                    _activator.OnReceive(message);
                }
                catch (Exception ex)
                {
                    OnActivatorError(ex);
                    break;
                }
            }
        }

        /// <summary>
        /// 终止任务。
        /// </summary>
        private void TerminateTask()
        {
            if (IsDisposed) return;
            _isRuning = false;
            IsDisposed = true;
            _activator.Dispose();
            _transceiver.Close();
            TaskEngine.Logger.Info("Task handler terminated");
            Owner.ReleaseTask(this);
        }

        /// <summary>
        /// 触发任务激活器错误事件。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        private void OnActivatorError(Exception ex)
        {
            TaskEngine.Logger.Error("Activator Error", ex);
            Message message = Message.CreateSystemMessage((Int32)SystemMessageCode.ActivatorErrorEvent, new ActivatorErrorEventArgs(ex));
            _transceiver.Send(message);
        }

        #endregion

        #endregion
    }
}
