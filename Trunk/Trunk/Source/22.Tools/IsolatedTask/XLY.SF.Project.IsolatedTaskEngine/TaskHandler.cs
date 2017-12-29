using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using XLY.SF.Project.IsolatedTaskEngine.Common;

namespace XLY.SF.Project.IsolatedTaskEngine
{
    /// <summary>
    /// 任务处理器。
    /// </summary>
    internal class TaskHandler
    {
        #region Event

        /// <summary>
        /// 当TaskHandler的任务结束时触发。
        /// </summary>
        public event EventHandler Terminate;

        #endregion

        #region Fields

        private Boolean _isRuning;

        private readonly ITaskExecutor _executor;

        private readonly MessageTransceiver _ss;

        private readonly NamedPipeServerStream _pipe;

        #endregion

        #region Cosntructors

        public TaskHandler(TaskManager owner)
        {
            Owner = owner;
            NamedPipeServerStream pipe = new NamedPipeServerStream(owner.Setup.TransceiverName, PipeDirection.InOut, owner.Setup.MaxParallelTask, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            _ss = new MessageTransceiver(pipe);
            _pipe = pipe;
            ITaskExecutor executor = (ITaskExecutor)Activator.CreateInstance(owner.Setup.EntryType);
            if (executor.Logger == null) executor.Logger = TaskEngine.Logger;
            executor.RequestSendMessage = (m) => Send(m);
            _executor = executor;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 是否启动成功。
        /// </summary>
        public Boolean IsLaunched => _isRuning;

        /// <summary>
        /// 拥有者。
        /// </summary>
        public TaskManager Owner { get; }

        /// <summary>
        /// 任务处理器的唯一标识。
        /// </summary>
        public Guid HandlerId => _executor.ExecurtorId;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 启动任务。
        /// </summary>
        public void Launch()
        {
            try
            {
                _pipe.WaitForConnection();
                if (_executor.Launch())
                {
                    _isRuning = true;
                    _ss.ReceiveAsync(ReceiveCallback);
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
        /// 关闭处理器并清理资源。
        /// </summary>
        public void Close()
        {
            //关闭执行器
            _executor.Close();
        }

        #endregion

        #region Private

        /// <summary>
        /// 将执行器的消息转发给客户端。
        /// </summary>
        /// <param name="message">消息。</param>
        private void Send(Message message)
        {
            _ss.Send(message);
        }

        /// <summary>
        /// 接收消息并将消息交由执行器处理器。
        /// </summary>
        private void ReceiveCallback(Message? m)
        {
            if (!m.HasValue)
            {
                _isRuning = false;
                TaskEngine.Logger.Info($"Task handler terminated:{HandlerId}");
                //断开连接
                _pipe.Disconnect();
                //关闭管道
                _pipe.Close();
                _executor.Close();
                Terminate?.Invoke(this, EventArgs.Empty);
                TaskEngine.Logger.Info($"Terminate:{HandlerId}");
            }
            else
            {
                if (m.Value != Message.Invalid)
                {
                    TaskEngine.Logger.Debug($"Received message:[Code]{m.Value.Code},[Token]{m.Value.Token}");
                    _executor.Receive(m.Value);
                    _ss.ReceiveAsync(ReceiveCallback);
                }
                else
                {
                    TaskEngine.Logger.Error($"Unrecognizable message:[Task]{HandlerId}");
                }
            }
        }

        /// <summary>
        /// 触发任务引擎错误事件。
        /// </summary>
        /// <param name="ex">异常信息。</param>
        private void OnTaskEngineError(Exception ex)
        {
            TaskEngine.Logger.Error($"Task engine Error:{HandlerId}", ex);
            Message message = Message.CreateSystemMessage((Int32)SystemMessageCode.TaskEngineErrorEvent, new TaskEnginErrorEventArgs(ex));
        }

        #endregion

        #endregion
    }
}
