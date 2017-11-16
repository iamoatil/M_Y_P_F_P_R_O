using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    /// <summary>
    /// 异步多任务通知。
    /// </summary>
    public interface IAsyncMultiTaskProgress
    {
        /// <summary>
        /// 任务进度改变事件。
        /// </summary>
        event EventHandler<TaskProgressEventArgs> ProgressChanged;

        /// <summary>
        /// 任务结束事件。
        /// </summary>
        event EventHandler<TaskTerminateEventArgs> Terminated;
    }
}
