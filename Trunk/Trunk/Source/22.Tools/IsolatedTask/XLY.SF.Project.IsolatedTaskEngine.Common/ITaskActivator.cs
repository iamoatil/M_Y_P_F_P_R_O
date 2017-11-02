using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 表示任务激活器。根据命令执行特定的业务。
    /// </summary>
    internal interface ITaskActivator : IDisposable
    {
        #region Events

        event EventHandler<TaskOverEventArgs> TaskOver;

        event EventHandler ProgressChanged;

        #endregion

        #region Methods

        /// <summary>
        /// 执行特定业务。
        /// </summary>
        /// <param name="message">消息。</param>
        void Execute(Message message);

        #endregion
    }
}
