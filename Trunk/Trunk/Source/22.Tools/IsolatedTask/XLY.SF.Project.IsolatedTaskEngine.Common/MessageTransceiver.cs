using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 读写字符串流。
    /// </summary>
    public class MessageTransceiver
    {
        #region Fields

        private readonly StreamReader _reader;

        private readonly StreamWriter _writer;

        private readonly ConcurrentQueue<Message> _sendQueue;

        private Boolean _isDisposed;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.IsolatedTaskEngine.Common.StreamString  实例。
        /// </summary>
        /// <param name="ioStream">流。</param>
        public MessageTransceiver(Stream ioStream)
        {
            _reader = new StreamReader(ioStream, Encoding.UTF8);
            _writer = new StreamWriter(ioStream, Encoding.UTF8);
            _sendQueue = new ConcurrentQueue<Message>();
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 接收消息。
        /// </summary>
        public void ReceiveAsync(Action<Message?> callback)
        {
            _reader.ReadLineAsync().ContinueWith(t =>
            {
                Message? message = null;
                //读取到null，表示客户端断开连接
                if (t.Result != null)
                {
                    message = Message.ToMessage(t.Result);
                }
                callback?.Invoke(message);
            });
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="str">字符串。</param>
        public void Send(Message message)
        {
            lock (_writer)
            {
                if (_isDisposed) return;
                try
                {
                    _writer.WriteLine(message.SerializeObject());
                    _writer.Flush();
                }
                catch (ObjectDisposedException)
                {
                    _isDisposed = true;
                }
            }
        }

        #endregion

        #endregion
    }
}
