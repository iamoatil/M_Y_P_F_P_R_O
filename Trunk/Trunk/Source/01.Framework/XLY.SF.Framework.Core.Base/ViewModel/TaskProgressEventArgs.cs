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
    public class TaskProgressEventArgs : TaskEventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 AsyncProgressEventArgs 实例。
        /// </summary>
        /// <param name="progress">进度。</param>
        /// <param name="exception">异常信息。</param>
        public TaskProgressEventArgs(Double progress, String message)
            : base(message)
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
