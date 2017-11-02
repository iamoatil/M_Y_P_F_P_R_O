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
            TaskActivator activator = (TaskActivator)System.Activator.CreateInstance(setup.EntryType);
            activator.TaskOver += (a, b) => TerminateTask(b);
            _activator = activator;
        }

        #endregion

        #region Properties

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
            _transceiver.Launch();
            _isRuning = true;
            Task.Factory.StartNew(Handle, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 清理资源。
        /// </summary>
        public void Dispose()
        {
            TerminateTask(new TaskOverEventArgs(false));
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
                _activator.Execute(message);
            }
            _activator.Dispose();
        }

        private void TerminateTask(TaskOverEventArgs e)
        {
            if (IsDisposed) return;
            _isRuning = false;
            _taskOverCallback(this, e);
            Int32 code = e.IsCompleted ? (Int32)SystemMessageCode.CompletedEvent : (Int32)SystemMessageCode.FailedEvent;
            _transceiver.Send(new Message(code, e));
            _transceiver.Close();
            IsDisposed = true;
        }

        #endregion

        #endregion
    }
}
