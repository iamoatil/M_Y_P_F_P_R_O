using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
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

        public event EventHandler Disconnect;

        #region TaskOver

        /// <summary>
        /// 任务结束事件。
        /// </summary>
        public event EventHandler<TaskOverEventArgs> TaskOver;

        /// <summary>
        /// 触发任务结束事件。
        /// </summary>
        /// <param name="args">事件参数。</param>
        private void OnTaskOver(TaskOverEventArgs args)
        {
            TaskOver?.Invoke(this, args);
        }

        #endregion

        #region MessageArrived

        public event EventHandler<Message> MessageArrived;

        /// <summary>
        /// 收到消息事件。
        /// </summary>
        /// <param name="message">消息。</param>
        private void OnMessageArrived(Message message)
        {
            MessageArrived?.Invoke(this, message);
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

        private readonly NamedPipeClientStream _pipe;

        private readonly StreamString _ss;

        private Boolean _requestStop;

        private Boolean _isInit;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 TaskProxy 实例。
        /// </summary>
        /// <param name="name">消息收发器名称。该名称必须与服务进程的消息收发器名称一致。</param>
        /// <param name="timeout">超时时间。如果小于或等于0,则具有无限超时值的等待服务进程响应。</param>
        /// <param name="serverName">要连接的远程计算机的名称。默认为本地计算机。</param>
        public TaskProxy(String name, String serverName = ".")
        {
            NamedPipeClientStream pipe = new NamedPipeClientStream(serverName, name, PipeDirection.InOut, PipeOptions.Asynchronous);
            _ss = new StreamString(pipe);
            _pipe = pipe;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 代理是否已经关闭。
        /// </summary>
        public Boolean IsTerminated => _requestStop;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 初始化代理。
        /// </summary>
        /// <param name="timeout">在连接超时之前等待响应的毫秒数。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Init(Int32 timeout = -1)
        {
            if(IsTerminated) throw new InvalidOperationException("The proxy is terminated");
            if (_isInit) return true;
            try
            {
                _pipe.Connect(timeout);
                _isInit = true;
                Task.Factory.StartNew(Receive, TaskCreationOptions.LongRunning);
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">消息。</param>
        public void Send(Message message)
        {
            if (!_isInit) throw new InvalidOperationException("The proxy need initialization");
            if (IsTerminated) throw new InvalidOperationException("The proxy is terminated");
            _ss.WriteString(message.SerializeObject());
        }

        /// <summary>
        /// 终止代理。
        /// </summary>
        public void Terminate()
        {
            if (_isInit)
            {
                _requestStop = true;
                if (_pipe.IsConnected)
                {
                    _pipe.Close();
                }
            }
        }

        #endregion

        #region Private

        private void Receive()
        {
            Message message = Message.Invalid;
            String str = null;
            Boolean? result = null;
            while (!_requestStop)
            {
                str = _ss.ReadString();
                result = HandleMessage(str, out message);
                if (!result.HasValue)
                {
                    Disconnect?.Invoke(this, EventArgs.Empty);
                    break;
                }
                if (result.Value)
                {
                    DispatchMessage(message);
                }
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
                case (Int32)SystemMessageCode.TaskOverEvent:
                    {
                        Terminate();
                        TaskOverEventArgs args = message.GetContent<TaskOverEventArgs>();
                        OnTaskOver(args);
                    }
                    break;
                case (Int32)SystemMessageCode.TaskEngineErrorEvent:
                    {
                        Terminate();
                        TaskEnginErrorEventArgs args = message.GetContent<TaskEnginErrorEventArgs>();
                        OnTaskEngineError(args);
                    }
                    break;
                default:
                    OnMessageArrived(message);
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
