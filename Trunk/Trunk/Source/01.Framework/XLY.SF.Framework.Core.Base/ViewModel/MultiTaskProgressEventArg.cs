using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 异步多任务进度改变事件参数。
    /// </summary>
    public class MultiTaskProgressEventArg : TaskProgressEventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 AsyncMultiTaskProgressEventtArg 实例。
        /// </summary>
        /// <param name="reporter">单个进度报告器。</param>
        /// <param name="message">消息。</param>
        public MultiTaskProgressEventArg(DefaultSingleTaskReporter reporter, String message = null)
            : base(reporter.Progress, message)
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
