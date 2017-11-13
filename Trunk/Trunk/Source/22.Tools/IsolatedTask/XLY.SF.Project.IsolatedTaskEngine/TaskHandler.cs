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

        private readonly Action<TaskHandler, TaskOverEventArgs> _taskOverCallback;

        #endregion

        #region Cosntructors

        public TaskHandler(EngineSetup setup, Action<TaskHandler, TaskOverEventArgs> taskOverCallback)
        {
            _taskOverCallback = taskOverCallback;
            _transceiver = new MessageServerTransceiver(setup.TransceiverName, setup.MaxParallelTask);
            _transceiver.Disconnect += (a, b) => TerminateTask(null);
            TaskActivator activator = (TaskActivator)Activator.CreateInstance(setup.EntryType);
            activator.RequestSendMessageCallback = (m) => _transceiver.Send(m);
            activator.TaskOver += (a, b) => TerminateTask(b);
            activator.ActivatorError += (a, b) => OnActivatorError(b);
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
                        Task.Factory.StartNew(Handle, TaskCreationOptions.LongRunning);
                    }
                    else
                    {
                        OnActivatorError(new ActivatorErrorEventArgs("Launch task failed"));
                    }
                }
                catch (Exception ex)
                {
                    OnActivatorError(new ActivatorErrorEventArgs(ex));
                }
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// 清理资源。
        /// </summary>
        public void Dispose()
        {
            TerminateTask(null);
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
                if (message == null) continue;
                try
                {
                    _activator.OnReceive(message);
                }
                catch (Exception ex)
                {
                    OnActivatorError(new ActivatorErrorEventArgs(ex));
                }
            }
        }

        /// <summary>
        /// 终止任务。
        /// </summary>
        /// <param name="e">如果e为null，表示客户端断开连接而导致的任务终止。否则，由注入的业务触发。</param>
        private void TerminateTask(TaskOverEventArgs e)
        {
            if (IsDisposed) return;
            _isRuning = false;
            if (e != null)
            {
                Message message = Message.CreateSystemMessage((Int32)SystemMessageCode.TaskOverEvent, e);
                _transceiver.Send(message);
            }
            _activator.Dispose();
            _transceiver.Close();
            IsDisposed = true;
            _taskOverCallback(this, e);
        }

        /// <summary>
        /// 给代理发送激活器错误事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        private void OnActivatorError(ActivatorErrorEventArgs e)
        {
            Message message = Message.CreateSystemMessage((Int32)SystemMessageCode.ActivatorErrorEvent, e);
            _transceiver.Send(message);
        }

        #endregion

        #endregion
    }
}
