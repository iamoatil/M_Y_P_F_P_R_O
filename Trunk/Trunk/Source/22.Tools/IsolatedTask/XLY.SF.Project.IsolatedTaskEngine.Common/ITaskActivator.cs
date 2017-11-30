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
        #region Properites

        /// <summary>
        /// 激活器唯一标识。
        /// </summary>
        Guid ActivatorToken { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 启动业务。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        Boolean Launch();

        #endregion
    }
}
