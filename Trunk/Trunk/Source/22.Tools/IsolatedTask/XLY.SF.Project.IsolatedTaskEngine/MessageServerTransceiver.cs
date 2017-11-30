using System;
using System.IO.Pipes;
using XLY.SF.Project.IsolatedTaskEngine.Common;

namespace XLY.SF.Project.IsolatedTaskEngine
{
    /// <summary>
    ///  服务端消息收发器。
    /// </summary>
    internal class MessageServerTransceiver : MessageTransceiver
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 MessageServerTransceiver 实例。
        /// </summary>
        /// <param name="name">消息收发器名称。客户进程使用相同名称的消息收发器与之通信。</param>
        /// <param name="maxParallel">最大并行任务数。</param>
        public MessageServerTransceiver(String name, Int32 maxParallel)
            : base(new NamedPipeServerStream(name, PipeDirection.InOut, maxParallel, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 1024, 256))
        {

        }

        #endregion

        #region Methods

        #region protected

        /// <summary>
        /// 启动消息收发器。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean LaunchCore()
        {
            ((NamedPipeServerStream)Pipe).WaitForConnection();
            return Pipe.IsConnected;
        }

        #endregion

        #endregion
    }
}
