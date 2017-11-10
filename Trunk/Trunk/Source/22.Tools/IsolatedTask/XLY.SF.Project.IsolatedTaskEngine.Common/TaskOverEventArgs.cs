using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 任务结束事件参数。
    /// </summary>
    public class TaskOverEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 TaskFailedEventArgs 实例。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        public TaskOverEventArgs(Exception ex)
            : this(ex.Message)
        {
            Exception = ex;
        }

        /// <summary>
        /// 初始化类型 TaskFailedEventArgs 实例。
        /// </summary>
        /// <param name="errorMessage">错误消息。</param>
        public TaskOverEventArgs(String errorMessage)
            :this(false)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 初始化类型 TaskFailedEventArgs 实例。
        /// </summary>
        /// <param name="isCompleted">是否完成。</param>
        public TaskOverEventArgs(Boolean isCompleted)
        {
            IsCompleted = isCompleted;
        }

        /// <summary>
        /// 初始化类型 TaskFailedEventArgs 实例。
        /// </summary>
        private TaskOverEventArgs()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 异常信息。
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 错误消息。
        /// </summary>
        public String ErrorMessage { get; private set; }

        /// <summary>
        /// 任务是否完成。
        /// </summary>
        public Boolean IsCompleted { get; private set; }

        #endregion
    }
}
