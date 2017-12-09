using System;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 任务引擎错误事件参数。
    /// </summary>
    public class TaskEnginErrorEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 TaskEnginErrorEventArgs 实例。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        public TaskEnginErrorEventArgs(Exception ex)
        {
            Exception = ex;
        }

        /// <summary>
        /// 初始化类型 TaskEnginErrorEventArgs 实例。
        /// </summary>
        /// <param name="errorMessage">错误消息。</param>
        public TaskEnginErrorEventArgs(String errorMessage)
            :this(new Exception(errorMessage))
        {
        }

        /// <summary>
        /// 初始化类型 TaskEnginErrorEventArgs 实例。
        /// </summary>
        private TaskEnginErrorEventArgs()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 异常信息。
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion
    }
}
