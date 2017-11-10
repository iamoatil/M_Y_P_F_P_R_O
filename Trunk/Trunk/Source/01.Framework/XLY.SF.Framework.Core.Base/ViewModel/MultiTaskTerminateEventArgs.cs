using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 异步多任务结束事件参数。
    /// </summary>
    public class MultiTaskTerminateEventArgs : TaskTerminateEventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 AsyncMultiTaskTerminateEventArgs 实例。
        /// </summary>
        /// <param name="reporter">单个进度报告器。</param>
        /// <param name="exception">异常信息。</param>
        /// <param name="message">消息。</param>
        public MultiTaskTerminateEventArgs(DefaultSingleTaskReporter reporter, Exception exception, String message) 
            : base(exception, message)
        {
            Reporter = reporter ?? throw new ArgumentNullException("reporter");
        }


        /// <summary>
        /// 初始化类型 AsyncMultiTaskTerminateEventArgs 实例。
        /// </summary>
        /// <param name="reporter">单个进度报告器。</param>
        /// <param name="isCompleted">是否完成。如果为false，表示在未完成的情况下停止任务。</param>
        public MultiTaskTerminateEventArgs(DefaultSingleTaskReporter reporter, Boolean isCompleted)
            :base(isCompleted)
        {
            Reporter = reporter ?? throw new ArgumentNullException("reporter");
        }

        #endregion

        #region Properties

        /// <summary>
        /// 单个进度报告器。
        /// </summary>
        public DefaultSingleTaskReporter Reporter { get; }

        #endregion
    }
}
