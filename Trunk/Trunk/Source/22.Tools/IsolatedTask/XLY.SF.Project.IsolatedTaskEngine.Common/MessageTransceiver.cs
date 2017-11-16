using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 消息收发器。
    /// </summary>
    public abstract class MessageTransceiver : IDisposable
    {
        #region Event

        /// <summary>
        /// 连接断开事件。
        /// </summary>
        public event EventHandler Disconnect;

        /// <summary>
        /// 触发连接断开事件。
        /// </summary>
        protected void OnDisconnect()
        {
            Disconnect?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Fields

        private readonly PipeStream _pipe;

        private const String Magic = "TE";

        private readonly StreamReader _reader;

        private readonly StreamWriter _writer;

        #endregion

        #region Constructors

        protected MessageTransceiver(PipeStream pipe)
        {
            _pipe = pipe;
            _writer = new StreamWriter(_pipe, Encoding.UTF8);
            _reader = new StreamReader(_pipe, Encoding.UTF8);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 管道。
        /// </summary>
        internal protected PipeStream Pipe => _pipe;

        /// <summary>
        /// 对象使用的资源是否已被清理。
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 启动消息收发器。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Launch()
        {
            if (!LaunchCore()) return false;
            try
            {
                _pipe.ReadMode = PipeTransmissionMode.Message;
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (IOException)
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
            lock (_writer)
            {
                try
                {
                    if (_pipe.IsConnected)
                    {
                        String json = message.ToString();
                        _writer.WriteLine(json);
                        _writer.Flush();
                    }
                }
                catch (ObjectDisposedException)
                {
                    OnDisconnect();
                }
                catch (IOException)
                {
                    OnDisconnect();
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 接收消息。
        /// </summary>
        /// <returns>消息。</returns>
        public Message Receive()
        {
            StringBuilder sb = new StringBuilder();
            String temp = null;
            try
            {
                do
                {
                    temp = _reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(temp))
                    {
                        OnDisconnect();
                        return null;
                    }
                    sb.Append(temp);
                } while (!_pipe.IsMessageComplete);
                return Message.ToMessage(sb.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 关闭消息收发器。
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 清理资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 清理资源。
        /// </summary>
        ~MessageTransceiver()
        {
            Dispose(false);
        }

        #endregion

        #region Protected

        /// <summary>
        /// 启动消息收发器。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected abstract Boolean LaunchCore();

        /// <summary>
        /// 清理资源。
        /// </summary>
        /// <param name="isDisposing">true表示显示清理，否则表示系统自动清理。</param>
        protected virtual void Dispose(Boolean isDisposing)
        {
            if (IsDisposed) return;
            if (isDisposing)
            {
                _pipe.Dispose();
                try
                {
                    _writer.Close();
                }
                catch (ObjectDisposedException)
                {
                }
                catch (IOException)
                {
                }
                _reader.Close();
            }
            IsDisposed = true;
        }

        #endregion

        #endregion
    }
}
