using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 激活器错误事件参数。
    /// </summary>
    public class ActivatorErrorEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 ActivatorErrorEventArgs 实例。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        public ActivatorErrorEventArgs(Exception ex)
        {
            Exception = ex;
        }

        /// <summary>
        /// 初始化类型 ActivatorErrorEventArgs 实例。
        /// </summary>
        /// <param name="errorMessage">错误消息。</param>
        public ActivatorErrorEventArgs(String errorMessage)
            :this(new Exception(errorMessage))
        {
        }

        /// <summary>
        /// 初始化类型 ActivatorErrorEventArgs 实例。
        /// </summary>
        private ActivatorErrorEventArgs()
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
