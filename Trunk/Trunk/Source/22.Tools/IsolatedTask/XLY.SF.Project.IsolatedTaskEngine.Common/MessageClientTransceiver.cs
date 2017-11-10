using System;
using System.IO.Pipes;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 客户进程的消息收发器。
    /// </summary>
    public class MessageClientTransceiver : MessageTransceiver
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 MessageClientTransceiver 实例。
        /// </summary>
        /// <param name="name">消息收发器名称。该名称必须与服务进程的消息收发器名称一致。</param>
        /// <param name="timeout">超时时间。如果小于或等于0,则具有无限超时值的等待服务进程响应。</param>
        /// <param name="serverName">要连接的远程计算机的名称。默认为本地计算机。</param>
        public MessageClientTransceiver(String name, String serverName = ".", Int32 timeout = 0)
            : base(new NamedPipeClientStream(serverName, name, PipeDirection.InOut, PipeOptions.Asynchronous))
        {
            Timeout = timeout;
        }

        #endregion

        #region Properties

        public Int32 Timeout { get; }

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 启动消息收发器。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean LaunchCore()
        {
            if (Timeout <= 0)
            {
                ((NamedPipeClientStream)Pipe).Connect();
            }
            else
            {
                ((NamedPipeClientStream)Pipe).Connect(Timeout);
            }
            return Pipe.IsConnected;
        }

        #endregion

        #endregion
    }
}
