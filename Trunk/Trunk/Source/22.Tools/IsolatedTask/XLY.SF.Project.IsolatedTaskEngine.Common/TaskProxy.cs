using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 任务客户进程代理。
    /// </summary>
    public class TaskProxy
    {
        #region Events

        public event EventHandler Disconnected;

        #region TaskOver

        /// <summary>
        /// 任务结束事件。
        /// </summary>
        public event EventHandler<TaskTerminateEventArgs> TaskTerminated;

        /// <summary>
        /// 触发任务结束事件。
        /// </summary>
        /// <param name="args">事件参数。</param>
        private void OnTaskTerminate(TaskTerminateEventArgs args)
        {
            TaskTerminated?.Invoke(this, args);
        }

        #endregion

        #region MessageArrived

        public event EventHandler<Message> Received;

        /// <summary>
        /// 收到消息事件。
        /// </summary>
        /// <param name="message">消息。</param>
        private void OnReceived(Message message)
        {
            Received?.Invoke(this, message);
        }

        #endregion

        #region ActivatorError

        /// <summary>
        /// 任务引擎错误事件。
        /// </summary>
        public event EventHandler<TaskEnginErrorEventArgs> TaskEngineError;

        /// <summary>
        /// 触发任务引擎错误事件。
        /// </summary>
        /// <param name="args">事件参数。</param>
        private void OnTaskEngineError(TaskEnginErrorEventArgs args)
        {
            TaskEngineError?.Invoke(this, args);
        }

        #endregion

        #endregion

        #region Fields

        private NamedPipeClientStream _pipe;

        private MessageTransceiver _ss;

        private Boolean _requestStop;

        private readonly String _name;

        private readonly String _serverName;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 TaskProxy 实例。
        /// </summary>
        /// <param name="name">消息收发器名称。该名称必须与服务进程的消息收发器名称一致。</param>
        /// <param name="serverName">要连接的远程计算机的名称。默认为本地计算机。</param>
        public TaskProxy(String name, String serverName = ".")
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _serverName = serverName ?? throw new ArgumentNullException(nameof(serverName));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 代理是否已经连接。
        /// </summary>
        public Boolean IsConnected
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set;
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 连接。
        /// </summary>
        /// <param name="timeout">在连接超时之前等待响应的毫秒数。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        public void Connect(Int32 timeout = 2000)
        {
            if (IsConnected) return;
            NamedPipeClientStream pipe = null;
            try
            {
                _requestStop = false;
                pipe = new NamedPipeClientStream(_serverName, _name, PipeDirection.InOut, PipeOptions.Asynchronous,System.Security.Principal.TokenImpersonationLevel.None);
                pipe.Connect(timeout);
                pipe.ReadMode = PipeTransmissionMode.Message;
                IsConnected = true;
                _pipe = pipe;
                _ss = new MessageTransceiver(pipe);
                _ss.ReceiveAsync(ReceiveCallback);
            }
            catch (TimeoutException)
            {
                pipe.Dispose();
            }
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">消息。</param>
        public void Send(Message message)
        {
            if (!IsConnected) throw new InvalidOperationException("The proxy need connection");
            _ss.Send(message);
        }

        /// <summary>
        /// 断开代理。
        /// </summary>
        public void Disconnect()
        {
            if (!IsConnected) return;
            _requestStop = true;
            _pipe.Close();
        }

        #endregion

        #region Private

        private void HandleDisconnection()
        {
            _requestStop = false;
            _pipe = null;
            IsConnected = false;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private void ReceiveCallback(Message? m)
        {
            if (_requestStop) return;
            if (!m.HasValue)
            {
                HandleDisconnection();
            }
            else
            {
                if (m.Value != Message.Invalid)
                {
                    DispatchMessage(m.Value);
                }
                _ss.ReceiveAsync(ReceiveCallback);
            }
        }

        /// <summary>
        /// 调度消息。
        /// </summary>
        /// <param name="message">消息。</param>
        private void DispatchMessage(Message message)
        {
            switch (message.Code)
            {
                case (Int32)SystemMessageCode.TaskTerminateEvent:
                    {
                        TaskTerminateEventArgs args = message.GetContent<TaskTerminateEventArgs>();
                        OnTaskTerminate(args);
                    }
                    break;
                case (Int32)SystemMessageCode.TaskEngineErrorEvent:
                    {
                        Disconnect();
                        TaskEnginErrorEventArgs args = message.GetContent<TaskEnginErrorEventArgs>();
                        OnTaskEngineError(args);
                    }
                    break;
                default:
                    OnReceived(message);
                    break;
            }
        }

        /// <summary>
        /// 处理消息。
        /// </summary>
        /// <param name="str">接收到的字符串。</param>
        /// <param name="message">将接收到的字符串转换为Message类型实例。</param>
        /// <returns>如果为true，表示收到正确格式的消息；如果为false，表示消息无法被识别；如果为null，表示连接断开。</returns>
        private Boolean? HandleMessage(String str, out Message message)
        {
            //读取到null，表示客户端断开连接
            if (str == null)
            {
                message = Message.Invalid;
                return null;
            }
            message = Message.ToMessage(str);
            return message != Message.Invalid;
        }

        #endregion

        #endregion
    }
}
