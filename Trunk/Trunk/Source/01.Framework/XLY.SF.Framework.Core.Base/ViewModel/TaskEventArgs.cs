using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    /// <summary>
    /// 异步任务事件。
    /// </summary>
    public class TaskEventArgs : EventArgs
    {
        #region Cosntructors

        /// <summary>
        /// 初始化类型 AsyncTaskEventArgs 实例。
        /// </summary>
        public TaskEventArgs()
        {

        }

        /// <summary>
        /// 初始化类型 AsyncTaskEventArgs 实例。
        /// </summary>
        /// <param name="message">事件消息。</param>
        public TaskEventArgs(String message)
        {
            Message = message;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 消息。
        /// </summary>
        public String Message { get; }

        #endregion
    }
}
