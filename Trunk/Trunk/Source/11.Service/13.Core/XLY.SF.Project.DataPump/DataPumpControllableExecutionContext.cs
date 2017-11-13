using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// 进度可控的数据泵的执行上下文。
    /// </summary>
    public class DataPumpControllableExecutionContext : DataPumpExecutionContext
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.DataPumpControllableExecutionContext 实例。
        /// </summary>
        /// <param name="metadata">对任务进行描述的元数据。</param>
        /// <param name="source">数据源。如果不需要数据源则设置为null。</param>
        internal DataPumpControllableExecutionContext(Pump metadata, SourceFileItem source)
            : base(metadata, source)
        {
        }


        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.DataPumpControllableExecutionContext 实例。
        /// 为了保持兼容提供此方法，强烈建议不要使用此构造器，因为它的Source属性不安全。
        /// </summary>
        /// <param name="metadata">对任务进行描述的元数据。</param>
        [Obsolete("在以后的版本中会移除该方法")]
        internal DataPumpControllableExecutionContext(Pump metadata)
            : base(metadata)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 用于异步通知的报告器。
        /// </summary>
        public MultiTaskReporterBase Reporter { get; set; }
        
        /// <summary>
        /// 执行取消操作所等待的最大时间间隔。默认为null表示一直等待。
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 与上下文关联的任务的当前状态。
        /// </summary>
        public TaskState Status => Reporter?.State ?? TaskState.Idle;

        #region CancellationTokenSource

        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 用于通知任务取消的 CancellationToken 对象。
        /// </summary>
        internal CancellationTokenSource CancellationTokenSource
        {
            get
            {
                if (_cancellationTokenSource == null)
                {
                    if (Timeout == null)
                    {
                        _cancellationTokenSource = new CancellationTokenSource();
                    }
                    else
                    {
                        _cancellationTokenSource = new CancellationTokenSource(Timeout.Value);
                    }
                }
                return _cancellationTokenSource;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal

        /// <summary>
        /// 将上下文恢复到初始状态。
        /// </summary>
        internal void Reset()
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            Reporter.Reset();
        }

        #endregion

        #endregion
    }
}
