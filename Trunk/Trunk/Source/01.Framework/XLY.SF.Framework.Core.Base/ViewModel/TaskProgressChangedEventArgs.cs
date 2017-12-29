using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 异步任务进度改变事件参数。
    /// </summary>
    public class TaskProgressChangedEventArgs : TaskEventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 TaskProgressChangedEventArgs 实例。
        /// </summary>
        /// <param name="taskId">任务标识。</param>
        /// <param name="progress">进度。</param>
        /// <param name="exception">异常信息。</param>
        public TaskProgressChangedEventArgs(String taskId, Double progress, String message)
            : base(taskId, message)
        {
            Progress = progress;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 当前进度
        /// </summary>
        public Double Progress { get; }

        #endregion
    }
}
