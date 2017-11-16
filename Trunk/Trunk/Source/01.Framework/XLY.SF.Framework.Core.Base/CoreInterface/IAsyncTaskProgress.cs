using System;
using System.Runtime.CompilerServices;
using XLY.SF.Framework.Core.Base.ViewModel;

/* ==============================================================================
* Description：IAsyncResult  
* Author     ：Fhjun
* Create Date：2017/4/11 11:16:14
* ==============================================================================*/

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    /// <summary>
    /// 异步任务通知。
    /// </summary>
    public interface IAsyncTaskProgress
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
