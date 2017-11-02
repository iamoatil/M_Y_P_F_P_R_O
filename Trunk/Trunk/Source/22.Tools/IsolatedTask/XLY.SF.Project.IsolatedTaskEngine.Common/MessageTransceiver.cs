using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 消息收发器。
    /// </summary>
    public abstract class MessageTransceiver : IDisposable
    {
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
        protected PipeStream Pipe => _pipe;

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
        public abstract void Launch();

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">消息。</param>
        public void Send(Message message)
        {
            try
            {
                _writer.AutoFlush = true;
                String json = JsonConvert.SerializeObject(message);
                _writer.WriteLine(json);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            }
            catch(JsonReaderException)
            {
            }
            catch (JsonSerializationException)
            {
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
                _pipe.ReadMode = PipeTransmissionMode.Message;
                do
                {
                    temp = _reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(temp)) continue;
                    sb.Append(temp);
                } while (!_pipe.IsMessageComplete);
                return JsonConvert.DeserializeObject<Message>(sb.ToString());
            }
            catch (JsonReaderException)
            {
                return null;
            }
            catch (JsonSerializationException)
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
        /// 清理资源。
        /// </summary>
        /// <param name="isDisposing">true表示显示清理，否则表示系统自动清理。</param>
        protected virtual void Dispose(Boolean isDisposing)
        {
            if (IsDisposed) return;
            if (isDisposing)
            {
                _writer.Close();
                _reader.Close();
            }
            IsDisposed = true;
        }

        #endregion

        #endregion
    }
}
