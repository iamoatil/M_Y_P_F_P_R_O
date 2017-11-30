using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 多任务进度报告器。
    /// </summary>
    public class DefaultMultiTaskReporter : MultiTaskReporterBase
    {
        #region Fields

        private readonly Dictionary<String, SingleTaskReporterBase> _reporters = new Dictionary<String, SingleTaskReporterBase>();

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 MultiProgressReporter 实例。
        /// </summary>
        public DefaultMultiTaskReporter()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 当前总进度。
        /// </summary>
        public override Double Progress
        {
            get
            {
                return _reporters.Values.Sum(x=>x.Progress) / _reporters.Count;
            }
        }

        /// <summary>
        /// 当前总状态。
        /// </summary>
        public override TaskState State
        {
            get
            {
                if (_reporters.Values.Any(x => ((Int32)x.State & 0xFF00) == 0x0100))
                {
                    return TaskState.Running;
                }
                else if (_reporters.Values.All(x => ((Int32)x.State & 0xFF00) == 0x0200))
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
        /// 获取指定任务的状态。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <returns>状态。</returns>
        public override TaskState this[String id]
        {
            get
            {
                if (_reporters.ContainsKey(id))
                {
                    return _reporters[id].State;
                }
                return TaskState.Idle;
            }
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取指定任务的进度。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <returns>进度。</returns>
        public override Double GetProgress(String id)
        {
            if (_reporters.ContainsKey(id))
            {
                return _reporters[id].Progress;
            }
            return 0;
        }

        /// <summary>
        /// 报告开始。
        /// </summary>
        /// <param name="id">任务id。</param>
        ///<param name="message">消息。</param>
        public override void Start(String id, String message = null)
        {
            if (!_reporters.ContainsKey(id))
            {
                DefaultSingleTaskReporter reporter = new DefaultSingleTaskReporter(id);
                reporter.ProgressChanged += (a, b) =>
                {
                    DefaultSingleTaskReporter r = (DefaultSingleTaskReporter)a;
                    OnProgressChanged(b);
                };
                reporter.Terminated += (a, b) =>
                  {
                      DefaultSingleTaskReporter r = (DefaultSingleTaskReporter)a;
                      OnTerminate(b);
                  };
                _reporters.Add(id, reporter);
                reporter.Start(message);
            }
        }

        /// <summary>
        /// 报告进度。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <param name="progress">进度。</param>
        /// <param name="message">消息。</param>
        public override void ChangeProgress(String id, Double progress, String message = null)
        {
            if (!_reporters.ContainsKey(id)) return;
            _reporters[id].ChangeProgress(progress, message);
        }

        /// <summary>
        /// 报告完成。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <param name="message">消息。</param>
        public override void Finish(String id,String message = null)
        {
            if (!_reporters.ContainsKey(id)) return;
            _reporters[id].Finish(message);
        }

        /// <summary>
        /// 报告停止。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <param name="message">消息。</param>
        public override void Stop(String id, String message = null)
        {
            if (!_reporters.ContainsKey(id)) return;
            _reporters[id].Stop(message);
        }

        /// <summary>
        /// 报告所有任务停止。
        /// </summary>
        public override void StopAll()
        {
            foreach (SingleTaskReporterBase reporter in _reporters.Values)
            {
                reporter.Stop();
            }
        }

        /// <summary>
        /// 报告失败。
        /// </summary>
        /// <param name="id">任务id。</param>
        /// <param name="ex">异常信息。</param>
        /// <param name="message">消息。</param>
        public override void Defeat(String id, Exception ex, String message = null)
        {
            if (!_reporters.ContainsKey(id)) return;
            _reporters[id].Defeat(ex, message);
        }

        /// <summary>
        /// 重置。
        /// </summary>
        public override void Reset()
        {
            if (State == TaskState.Completed)
            {
                foreach (DefaultSingleTaskReporter reporter in _reporters.Values)
                {
                    reporter.Reset();
                }
            }
        }

        #endregion

        #endregion
    }
}
