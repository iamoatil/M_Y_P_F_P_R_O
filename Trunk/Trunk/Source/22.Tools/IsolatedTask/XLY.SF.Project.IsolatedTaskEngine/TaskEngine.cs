using log4net;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Project.IsolatedTaskEngine.Common;

namespace XLY.SF.Project.IsolatedTaskEngine
{
    /// <summary>
    /// 任务引擎。
    /// </summary>
    public class TaskEngine
    {
        #region Fields

        private TaskManager _taskManager;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 TaskEngine 实例。
        /// </summary>
        /// <param name="setup">配置。</param>
        public TaskEngine(EngineSetup setup)
        {
            Setup = setup ?? throw new ArgumentNullException("setup");
        }

        #endregion

        #region Properties

        #region SetupInfo

        private EngineSetup _setup;
        /// <summary>
        /// 引擎配置。
        /// </summary>
        public EngineSetup Setup
        {
            get => _setup;
            private set
            {
                value.Owner = this;
                _setup = value;
            }
        }

        #endregion

        #region TaskCount

        /// <summary>
        /// 当前运行的任务数量。
        /// </summary>
        public Int32 TaskCount => _taskManager == null ? 0 : _taskManager.TaskCount;

        #endregion

        /// <summary>
        /// 指示引擎是否正在运行中。
        /// </summary>
        public Boolean IsRuning => _taskManager == null ? false : _taskManager.IsRuning;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 日志记录器。
        /// </summary>
        public static ILog Logger { get; } = LogManager.GetLogger("SysLog");

        /// <summary>
        /// 开启。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (IsRuning) return;
            _taskManager = new TaskManager(Setup);
            _taskManager.Start();
        }

        /// <summary>
        /// 停止。
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (IsRuning)
            {
                _taskManager.RequestStop();
                _taskManager = null;
            }
        }

        #endregion

        #region Private

        #endregion

        #endregion
    }
}
