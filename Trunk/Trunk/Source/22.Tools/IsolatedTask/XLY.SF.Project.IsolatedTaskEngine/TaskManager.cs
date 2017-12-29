using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        public EngineSetup Setup => _setup;

        #endregion

        #region Methods

        #region Public

        public void Start()
        {
            if (IsRuning) return;
            IsRuning = true;
            Task task = Task.Factory.StartNew(Dipatch, TaskCreationOptions.LongRunning);
            TaskEngine.Logger.Info("Task manager started");
            task.Wait();
        }

        public void RequestStop()
        {
            if (_requestStop) return;
            _requestStop = true;
            _semaphore.Dispose();
            _tasks.ForEach(x => x.Close());
            TaskEngine.Logger.Info("Task manager is stopping...");
        }

        #endregion

        #region Private

        private void Dipatch()
        {
            TaskHandler handler = null;
            while (!_requestStop)
            {
                _semaphore.WaitOne();
                if (_requestStop) break;
                handler = NewHandler();
                if (handler.IsLaunched)
                {
                    if (_requestStop)
                    {
                        handler.Close();
                        break;
                    }
                }
                else
                {
                    handler.Close();
                }
            }
            IsRuning = false;
            TaskEngine.Logger.Info("Task manager stopped.");
        }

        private TaskHandler NewHandler()
        {
            TaskHandler newHandler = new TaskHandler(this);
            newHandler.Terminate += NewHandler_Terminate;
            lock (_tasks)
            {
                _tasks.Add(newHandler);
            }
            newHandler.Launch();
            return newHandler;
        }

        private void NewHandler_Terminate(object sender, EventArgs e)
        {
            if (_requestStop) return;
            lock (_tasks)
            {
                _tasks.Remove((TaskHandler)sender);
            }
            _semaphore.Release();
            TaskEngine.Logger.Info($"Task manager recycle one handler:{((TaskHandler)sender).HandlerId}");
        }

        #endregion

        #endregion
    }
}
