using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 多任务进度报告器。
    /// </summary>
    public class TaskReporterAggregation : AsyncTaskProgressBase
    {
        #region Fields

        private readonly Dictionary<String, ITaskProgressReporter> _reporters = new Dictionary<String, ITaskProgressReporter>();
       
        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 MultiProgressReporter 实例。
        /// </summary>
        public TaskReporterAggregation()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 当前总进度。
        /// </summary>
        public Double Progress
        {
            get
            {
                return _reporters.Values.Sum(x=>x.Progress) / _reporters.Count;
            }
        }

        /// <summary>
        /// 当前总状态。
        /// </summary>
        public TaskState State
        {
            get
            {
                IEnumerable<ITaskProgressReporter> reporters = _reporters.Values;
                if (reporters.Any(x => x.IsNotStoppedAndIdle))
                {
                    return TaskState.Running;
                }
                else if (reporters.All(x => x.IsStopped))
                {
                    return TaskState.Completed;
                }
                else
                {
                    return TaskState.Idle;
                }
            }
        }

        /// <summary>
        /// 获取指定任务的进度报告器。
        /// </summary>
        /// <param name="taskId">任务id。</param>
        /// <returns>进度报告器。</returns>
        public ITaskProgressReporter this[String taskId]
        {
            get
            {
                if (_reporters.ContainsKey(taskId))
                {
                    return _reporters[taskId];
                }
                return null;
            }
        }

        /// <summary>
        /// 所有报告器。
        /// </summary>
        public IEnumerable<ITaskProgressReporter> Reporters => _reporters.Values;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取指定任务的进度。
        /// </summary>
        /// <param name="taskId">任务id。</param>
        /// <returns>进度。</returns>
        public Double GetProgress(String taskId)
        {
            if (_reporters.ContainsKey(taskId))
            {
                return _reporters[taskId].Progress;
            }
            return 0;
        }

        /// <summary>
        /// 报告正在等待被执行。
        /// </summary>
        /// <param name="taskId">任务id。</param>
        ///<param name="message">消息。</param>
        public void Wait(String taskId, String message = null)
        {
            if (!_reporters.ContainsKey(taskId))
            {
                DefaultTaskReporter reporter = new DefaultTaskReporter(taskId);
                reporter.ProgressChanged += (a, b) =>
                {
                    DefaultTaskReporter r = (DefaultTaskReporter)a;
                    OnProgressChanged(b);
                };
                reporter.Terminated += (a, b) =>
                {
                    DefaultTaskReporter r = (DefaultTaskReporter)a;
                    OnTerminated(b);
                };
                reporter.TaskStateChanged += (a, b) =>
                  {
                      DefaultTaskReporter r = (DefaultTaskReporter)a;
                      OnTaskStateChanged(b);
                  };
                _reporters.Add(taskId, reporter);
            }
            _reporters[taskId].Wait(message);
        }

        /// <summary>
        /// 报告开始。
        /// </summary>
        /// <param name="taskId">任务id。</param>
        ///<param name="message">消息。</param>
        public void Start(String taskId, String message = null)
        {
            if (!_reporters.ContainsKey(taskId)) return;
            _reporters[taskId].Start(message);
        }

        /// <summary>
        /// 报告进度。
        /// </summary>
        /// <param name="taskId">任务id。</param>
        /// <param name="progress">进度。</param>
        /// <param name="message">消息。</param>
        public void ChangeProgress(String taskId, Double progress, String message = null)
        {
            if (!_reporters.ContainsKey(taskId)) return;
            _reporters[taskId].ChangeProgress(progress, message);
        }

        /// <summary>
        /// 报告完成。
        /// </summary>
        /// <param name="taskId">任务id。</param>
        /// <param name="isCompleted">是完成还是取消。</param>
        /// <param name="message">消息。</param>
        public void Finish(String taskId, Boolean isCompleted, String message = null)
        {
            if (!_reporters.ContainsKey(taskId)) return;
            _reporters[taskId].Finish(isCompleted, message);
        }

        /// <summary>
        /// 重置进度报告器状态。
        /// </summary>
        /// <param name="taskId">任务Id。</param>
        public void Reset(String taskId)
        {
            if (!_reporters.ContainsKey(taskId)) return;
            _reporters[taskId].Reset();
        }

        /// <summary>
        /// 报告取消。
        /// </summary>
        /// <param name="taskId">任务id。</param>
        /// <param name="message">消息。</param>
        public void Cancelling(String taskId, String message = null)
        {
            if (!_reporters.ContainsKey(taskId)) return;
            _reporters[taskId].Cancelling(message);
        }

        /// <summary>
        /// 报告所有任务正在等待被执行。
        /// </summary>
        /// <param name="message">消息。</param>
        public void WaitAll(String message = null)
        {
            foreach (String taskId in _reporters.Keys)
            {
                Wait(taskId, message);
            }
        }

        /// <summary>
        /// 报告所有任务已开始。
        /// </summary>
        /// <param name="message">消息。</param>
        public void StartAll(String message = null)
        {
            foreach (String taskId in _reporters.Keys)
            {
                Start(taskId, message);
            }
        }

        /// <summary>
        /// 报告所有任务已停止。
        /// </summary>
        /// <param name="message">消息。</param>
        public void CancellingAll(String message = null)
        {
            foreach (TaskReporterBase reporter in Reporters)
            {
                reporter.Cancelling(message);
            }
        }

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="taskId">任务id。</param>
        /// <param name="ex">异常信息。</param>
        /// <param name="message">消息。</param>
        public void Defeat(String taskId, Exception ex, String message = null)
        {
            if (!_reporters.ContainsKey(taskId)) return;
            _reporters[taskId].Defeat(ex, message);
        }

        /// <summary>
        /// 重置。
        /// </summary>
        public void ResetAll()
        {
            if (State == TaskState.Completed)
            {
                foreach (ITaskProgressReporter reporter in Reporters)
                {
                    reporter.Reset();
                }
            }
        }

        #endregion

        #endregion
    }
}
