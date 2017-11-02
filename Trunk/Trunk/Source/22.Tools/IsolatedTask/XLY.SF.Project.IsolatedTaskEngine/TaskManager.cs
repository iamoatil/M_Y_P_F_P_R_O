using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Project.IsolatedTaskEngine.Common;

namespace XLY.SF.Project.IsolatedTaskEngine
{
    internal class TaskManager
    {
        #region Fields

        private readonly List<TaskHandler> _tasks;

        private readonly Semaphore _semaphore;

        private readonly EngineSetup _setup;

        private Boolean _requestStop;

        #endregion

        #region Constructors

        public TaskManager(EngineSetup setup)
        {
            _setup = setup;
            _tasks = new List<TaskHandler>(setup.MaxParallelTask);
            _semaphore = new Semaphore(setup.MaxParallelTask, setup.MaxParallelTask);
        }

        #endregion

        #region Properties

        public Int32 TaskCount => _tasks.Count;

        public Boolean IsRuning { get; private set; }

        #endregion

        #region Methods

        #region Public

        public void Start()
        {
            if (IsRuning) return;
            IsRuning = true;
            Task.Factory.StartNew(Dipatch, TaskCreationOptions.LongRunning);
        }

        public void RequestStop()
        {
            if (_requestStop) return;
            _requestStop = true;
            _semaphore.Dispose();
            lock (_tasks)
            {
                _tasks.ForEach(x => x.Close());
            }
        }

        #endregion

        #region Private

        private void Dipatch()
        {
            while (!_requestStop)
            {
                _semaphore.WaitOne();
                if (_requestStop) break;
                TaskHandler newHandler = new TaskHandler(_setup, TaskOverCallback);
                lock (_tasks)
                {
                    if (_requestStop) break;
                    _tasks.Add(newHandler);
                }
                newHandler.Launch();
            }
            IsRuning = false;
        }

        private void TaskOverCallback(TaskHandler sender, TaskOverEventArgs e)
        {
            if (_requestStop) return;
            lock (_tasks)
            {
                _tasks.Remove(sender);
            }
            _semaphore.Release();
        }

        #endregion

        #endregion
    }
}
