using Newtonsoft.Json;
using System;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 消息。
    /// </summary>
    public struct Message : IEquatable<Message>
    {
        #region Fields

        /// <summary>
        /// 表示一个无效的消息。
        /// </summary>
        public static readonly Message Invalid = new Message { IsValid = false };

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化结构 Message 。
        /// </summary>
        static Message()
        {
        }

        /// <summary>
        /// 初始化结构 Message 实例。
        /// </summary>
        /// <param name="code">消息码。</param>
        public Message(Int32 code)
            : this(Guid.NewGuid().GetHashCode(), code)
        {
        }

        /// <summary>
        /// 初始化结构 Message 实例。
        /// </summary>
        /// <param name="code">消息码。</param>
        /// <param name="content">消息内容。</param>
        public Message(Int32 code, Object content)
            : this(Guid.NewGuid().GetHashCode(), code, content)

        {
        }

        /// <summary>
        /// 初始化结构 Message 实例。
        /// </summary>
        /// <param name="token">消息唯一标识。</param>
        /// <param name="code">消息码。</param>
        public Message(Int32 token, Int32 code)
            : this(token, code, null)
        {
        }

        /// <summary>
        /// 初始化结构 Message 实例。
        /// </summary>
        /// <param name="token">消息唯一标识。</param>
        /// <param name="code">消息码。</param>
        /// <param name="content">消息内容。</param>
        public Message(Int32 token, Int32 code, Object content)
            : this(token, code, content, false)
        {
        }

        /// <summary>
        /// 初始化结构 Message 实例。
        /// </summary>
        /// <param name="token">消息唯一标识。</param>
        /// <param name="code">消息码。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="isSystemMessage">是否是系统消息。</param>
        private Message(Int32 token, Int32 code, Object content, Boolean isSystemMessage)
        {
            if (!isSystemMessage && code < 0)
            {
                throw new ArgumentOutOfRangeException("The code must be greater or equal than 0");
            }
            Token = token;
            Code = code;
            ContentJson = content.SerializeObject();
            IsValid = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 消息唯一标识。
        /// </summary>
        public Int32 Token { get;  private set; }

        /// <summary>
        /// 命令码。
        /// </summary>
        public Int32 Code { get; private set; }

        /// <summary>
        /// 消息内容。
        /// </summary>
        public String ContentJson { get; private set; }

        /// <summary>
        /// 是否是一个有效的消息。
        /// </summary>
        public Boolean IsValid { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 判断两个结构的实例是否相等。
        /// </summary>
        /// <param name="other">另一个实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public Boolean Equals(Message other)
        {
            return other.IsValid == IsValid
                && other.Token == Token;
        }

        /// <summary>
        /// 判断两个结构的实例是否相等。
        /// </summary>
        /// <param name="other">另一个实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public override Boolean Equals(Object obj)
        {
            if (obj is Message m)
            {
                return Equals(m);
            }
            return false;
        }

        /// <summary>
        /// 获取消息的内容。
        /// </summary>
        /// <typeparam name="T">内容的类型。</typeparam>
        /// <returns>消息内容。</returns>
        public T GetContent<T>()
        {
            return ContentJson.DeserializeObject<T>();
        }

        /// <summary>
        /// 获取对象的Hash值。
        /// </summary>
        /// <returns>对象的Hash值。</returns>
        public override Int32 GetHashCode()
        {
            return Token;
        }

        /// <summary>
        /// 创建响应命消息。
        /// </summary>
        /// <param name="returnValue">返回</param>
        /// <returns>响应消息。</returns>
        public Message CreateResponse(Object returnValue = null)
        {
            return new Message(Token, Code, returnValue);
        }

        /// <summary>
        /// 获取对象的字符串表示。
        /// </summary>
        /// <returns>对象的字符串表示。</returns>
        public override String ToString()
        {
            return ContentJson;
        }

        /// <summary>
        /// 将JSON字符串转换为Message。
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>将JSON字符串转换为Message</returns>
        public static Message ToMessage(String json)
        {
            try
            {
                return json.DeserializeObject<Message>(Message.Invalid);
            }
            catch (JsonException)
            {
                return Message.Invalid;
            }
            catch (FormatException)
            {
                return Message.Invalid;
            }
        }

        /// <summary>
        /// 重载Message的==操作符。
        /// </summary>
        /// <param name="messageA">Message结构实例。</param>
        /// <param name="messageB">Message结构实例。</param>
        /// <returns>相等返回true；否则返回false。</returns>
        public static Boolean operator ==(Message messageA, Message messageB)
        {
            return messageA.Equals(messageB);
        }

        /// <summary>
        /// 重载Message的!=操作符。
        /// </summary>
        /// <param name="messageA">Message结构实例。</param>
        /// <param name="messageB">Message结构实例。</param>
        /// <returns>相等返回false；否则返回true。</returns>
        public static Boolean operator !=(Message messageA, Message messageB)
        {
            return !(messageA == messageB);
        }

        #endregion

        #region Internal

        /// <summary>
        /// 创建系统消息。
        /// </summary>
        /// <param name="token">消息唯一标识。</param>
        /// <param name="code">消息码。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>系统消息。</returns>
        internal static Message CreateSystemMessage(Int32 token, Int32 code, Object content = null)
        {
            return new Message(token, code, content, true);
        }

        /// <summary>
        /// 创建系统消息。
        /// </summary>
        /// <param name="code">消息码。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>系统消息。</returns>
        internal static Message CreateSystemMessage(Int32 code, Object content = null)
        {
            return CreateSystemMessage(Guid.NewGuid().GetHashCode(), code, content);
        }

        #endregion

        #endregion
    }
}
