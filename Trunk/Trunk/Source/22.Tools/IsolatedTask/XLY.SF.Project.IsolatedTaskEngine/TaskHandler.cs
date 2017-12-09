using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.IsolatedTaskEngine.Common;

namespace XLY.SF.Project.IsolatedTaskEngine
{
    /// <summary>
    /// 任务处理器。
    /// </summary>
    internal class TaskHandler : IDisposable
    {
        #region Event

        /// <summary>
        /// 当TaskHandler的任务结束时触发。
        /// </summary>
        public event EventHandler Terminate;

        #endregion

        #region Fields

        private Boolean _isRuning;

        private readonly TaskActivator _activator;

        private readonly StreamString _ss;

        private readonly NamedPipeServerStream _pipe;

        #endregion

        #region Cosntructors

        public TaskHandler(TaskManager owner)
        {
            Owner = owner;
            NamedPipeServerStream pipe = new NamedPipeServerStream(owner.Setup.TransceiverName, PipeDirection.InOut, owner.Setup.MaxParallelTask, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            _ss = new StreamString(pipe);
            _pipe = pipe;
            TaskActivator activator = (TaskActivator)Activator.CreateInstance(owner.Setup.EntryType);
            activator.Logger = TaskEngine.Logger;
            activator.RequestSendMessageCallback = (m) => Send(m);
            activator.RequestTerminateTask = () => _pipe.Disconnect();
            _activator = activator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 是否启动成功。
        /// </summary>
        public Boolean IsLaunched => _isRuning;

        /// <summary>
        /// 是否已被清理。
        /// </summary>
        public Boolean IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// 拥有者。
        /// </summary>
        public TaskManager Owner { get; }

        /// <summary>
        /// 任务唯一标识。
        /// </summary>
        public Guid Token => _activator.ActivatorToken;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 启动任务。
        /// </summary>
        public void Launch()
        {
            if (_isRuning || IsDisposed) return;
            try
            {
                _pipe.WaitForConnection();
                if (_activator.Launch())
                {
                    _isRuning = true;
                    Task.Factory.StartNew(Receive, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent).ContinueWith(t =>
                    {
                        Release();
                    }, TaskContinuationOptions.AttachedToParent);
                    TaskEngine.Logger.Info("Task handler launched");
                }
                else
                {
                    OnTaskEngineError(new Exception("Task handler launched failed"));
                }
            }
            catch (IOException)
            {
                return;
            }
            catch (Exception ex)
            {
                OnTaskEngineError(ex);
            }
        }

        /// <summary>
        /// 清理资源。
        /// </summary>
        public void Dispose()
        {
            Release();
        }

        /// <summary>
        /// 关闭任务。
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        #endregion

        #region Private

        private void Send(Message message)
        {
            String str = message.SerializeObject();
            _ss.WriteString(str);
        }

        private void Receive()
        {
            Message message = Message.Invalid;
            String str = null;
            Boolean? result = null;
            while (_isRuning)
            {
                str = _ss.ReadString();
                result = HandleMessage(str,out message);
                if (!result.HasValue) break;
                if (result.Value)
                {
                    _activator.OnReceive(message);
                }
                else
                {
                    OnTaskEngineError(new InvalidDataException($"Unrecognizable message:[Task]{Token}"));
                }
            }
            _isRuning = false;
            TaskEngine.Logger.Info($"Task handler terminated:{_activator.ActivatorToken}");
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
                TaskEngine.Logger.Info($"Disconnect:[Task]{Token}");
                message = Message.Invalid;
                return null;
            }
            message = Message.ToMessage(str);
            //读取到无效的消息，表示数据格式不正确
            if (message == Message.Invalid)
            {
                TaskEngine.Logger.Debug($"Unrecognizable message:[Task]{Token}");
                return false;
            }

            TaskEngine.Logger.Debug($"Received message:[Code]{message.Code},[Token]{message.Token}");
            return true;
        }

        /// <summary>
        /// 释放资源，断开连接以及关闭管道。
        /// </summary>
        private void Release()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            _activator.Dispose();
            //断开连接
            if (_pipe.IsConnected)
            {
                _pipe.Disconnect();
            }
            //关闭管道
            _pipe.Close();
            Terminate?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发任务引擎错误事件。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        private void OnTaskEngineError(Exception ex)
        {
            TaskEngine.Logger.Error($"Task engine Error:{Token}", ex);
            Message message = Message.CreateSystemMessage((Int32)SystemMessageCode.TaskEngineErrorEvent, new TaskEnginErrorEventArgs(ex));
        }

        #endregion

        #endregion
    }
}
