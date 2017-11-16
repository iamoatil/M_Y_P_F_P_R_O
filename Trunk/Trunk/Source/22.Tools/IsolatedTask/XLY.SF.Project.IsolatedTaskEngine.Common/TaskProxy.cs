using System;
using System.Collections.Generic;
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

        #region ActivatorError

        /// <summary>
        /// 任务激活错误事件。
        /// </summary>
        public event EventHandler<ActivatorErrorEventArgs> ActivatorError;

        /// <summary>
        /// 触发任务激活器错误事件。
        /// </summary>
        /// <param name="args">事件参数。</param>
        private void OnActivatorError(ActivatorErrorEventArgs args)
        {
            ActivatorError?.Invoke(this, args);
        }

        #endregion

        #endregion

        #region Fields

        private readonly MessageClientTransceiver _transceiver;

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
        public TaskProxy(String name, String serverName = ".", Int32 timeout = 20000)
        {
            _transceiver = new MessageClientTransceiver(name, serverName, timeout);
            _transceiver.Disconnect += (a, b) =>
            {
                Terminate();
                DisconnectCallback?.Invoke();
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// 接收消息的回调方法。
        /// </summary>
        public Action<Message> ReceiveCallback { get; set; }

        /// <summary>
        /// 断开连接的回调方法。
        /// </summary>
        public Action DisconnectCallback { get; set; }

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
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Init()
        {
            if (_isInit) return true;
            if (_transceiver.Launch())
            {
                Task.Factory.StartNew(Receive,TaskCreationOptions.LongRunning);
                _isInit = true;
            }
            return _isInit;
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">消息。</param>
        public void Send(Message message)
        {
            if (_requestStop) throw new InvalidOperationException("The proxy is terminated");
            if (!_isInit) throw new InvalidOperationException("The proxy need initialization");
            _transceiver.Send(message);
        }

        /// <summary>
        /// 终止代理。
        /// </summary>
        public void Terminate()
        {
            if (_isInit)
            {
                _requestStop = true;
                _transceiver.Close();
            }
        }

        #endregion

        #region Private

        private void Receive()
        {
            Message message = null;
            while (!_requestStop)
            {
                message = _transceiver.Receive();
                if (message != null)
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
                        case (Int32)SystemMessageCode.ActivatorErrorEvent:
                            {
                                Terminate();
                                ActivatorErrorEventArgs args = message.GetContent<ActivatorErrorEventArgs>();
                                OnActivatorError(args);
                            }
                            break;
                        default:
                            ReceiveCallback?.Invoke(message);
                            break;
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
