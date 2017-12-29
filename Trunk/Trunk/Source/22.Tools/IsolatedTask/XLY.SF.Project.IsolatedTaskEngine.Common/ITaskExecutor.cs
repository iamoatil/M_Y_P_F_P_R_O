using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 表示任务执行器。根据命令执行特定的业务。
    /// </summary>
    internal interface ITaskExecutor
    {
        #region Properites

        /// <summary>
        /// 任务执行器的唯一标识。
        /// </summary>
        Guid ExecurtorId { get; }

        /// <summary>
        /// 日志记录器。
        /// </summary>
        ILog Logger { get; set; }

        /// <summary>
        /// 任务执行器是否已经关闭。
        /// </summary>
        Boolean IsClosed { get; }

        /// <summary>
        /// 请求引擎将消息发送给客户端。
        /// </summary>
        Action<Message> RequestSendMessage { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 启动任务执行器。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        Boolean Launch();

        /// <summary>
        /// 关闭任务执行器。
        /// </summary>
        void Close();

        /// <summary>
        /// 收到客户进程消息时执行该方法。
        /// </summary>
        /// <param name="message">消息。</param>
        void Receive(Message message);

        #endregion
    }
}
