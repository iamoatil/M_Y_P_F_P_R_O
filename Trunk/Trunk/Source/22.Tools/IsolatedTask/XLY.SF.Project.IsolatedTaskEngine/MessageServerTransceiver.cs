using System;
using System.IO.Pipes;
using XLY.SF.Project.IsolatedTaskEngine.Common;

namespace XLY.SF.Project.IsolatedTaskEngine
{
    /// <summary>
    ///  服务端消息收发器。
    /// </summary>
#if DEBUG
    public
#else
        internal
#endif
     class MessageServerTransceiver : MessageTransceiver
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 MessageServerTransceiver 实例。
        /// </summary>
        /// <param name="name">消息收发器名称。客户进程使用相同名称的消息收发器与之通信。</param>
        /// <param name="maxParallel">最大并行任务数。</param>
        public MessageServerTransceiver(String name, Int32 maxParallel)
            : base(new NamedPipeServerStream(name, PipeDirection.InOut, maxParallel, PipeTransmissionMode.Message, PipeOptions.None))
        {

        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 启动消息收发器。
        /// </summary>
        public override void Launch()
        {
            ((NamedPipeServerStream)Pipe).WaitForConnection();
        }

        #endregion

        #endregion
    }
}
