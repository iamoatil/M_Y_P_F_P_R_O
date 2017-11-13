using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 消息。
    /// </summary>
    public class Message
    {
        #region Fields

        private static readonly JsonSerializerSettings JsonSettings;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Message 。
        /// </summary>
        static Message()
        {
            JsonSettings = new JsonSerializerSettings()
            {
                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                 PreserveReferencesHandling = PreserveReferencesHandling.None,
                TypeNameHandling = TypeNameHandling.All,
            };
            DefaultContractResolver resolver = new DefaultContractResolver();
            resolver.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
            JsonSettings.ContractResolver = resolver;
        }

        /// <summary>
        /// 初始化类型 Message 实例。
        /// </summary>
        /// <param name="code">消息码。</param>
        public Message(Int32 code)
            : this(Guid.NewGuid().GetHashCode(), code)
        {
        }

        /// <summary>
        /// 初始化类型 Message 实例。
        /// </summary>
        /// <param name="token">消息唯一标识。</param>
        /// <param name="code">消息码。</param>
        public Message(Int32 token, Int32 code)
            : this(token, code, false)
        {
        }

        /// <summary>
        /// 初始化类型 Message 实例。
        /// </summary>
        /// <param name="token">消息唯一标识。</param>
        /// <param name="code">消息码。</param>
        /// <param name="isSystemMessage">是否是系统消息。</param>
        private Message(Int32 token, Int32 code, Boolean isSystemMessage)
        {
            if (!isSystemMessage && code < 0)
            {
                throw new ArgumentOutOfRangeException("The code must be greater or equal than 0");
            }
            Token = token;
            Code = code;
        }

        /// <summary>
        /// 初始化类型 Message 实例。
        /// </summary>
        private Message()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// 命令唯一标识。
        /// </summary>
        public Int32 Token { get;  private set; }

        /// <summary>
        /// 命令码。
        /// </summary>
        public Int32 Code { get; private set; }

        /// <summary>
        /// 消息内容。
        /// </summary>
        private String ContentJson { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取消息的内容。
        /// </summary>
        /// <typeparam name="T">内容的类型。</typeparam>
        /// <returns>消息内容。</returns>
        public T GetContent<T>()
        {
            return DeserializeObject<T>(ContentJson);
        }

        /// <summary>
        /// 反序列化JSON字符串。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="json">JSON字符串。</param>
        /// <returns>类型 T 的实例。</returns>
        public static T DeserializeObject<T>(String json)
        {
            try
            {
                if (json == null) return default(T);
                return JsonConvert.DeserializeObject<T>(json, Message.JsonSettings);
            }
            catch (JsonException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 反序列化JSON字符串。
        /// </summary>
        /// <param name="json">JSON字符串。</param>
        /// <param name="type">类型。</param>
        /// <returns>实例。</returns>
        public static Object DeserializeObject(String json, Type type)
        {
            try
            {
                if (json == null) return null;
                return JsonConvert.DeserializeObject(json, type, Message.JsonSettings);
            }
            catch (JsonException)
            {
                return null;
            }
        }


        /// <summary>
        /// 反序列化JSON字符串。
        /// </summary>
        /// <param name="json">JSON字符串。</param>
        /// <returns>实例。</returns>
        public static Object DeserializeObject(String json)
        {
            try
            {
                if (json == null) return null;
                return JsonConvert.DeserializeObject(json, Message.JsonSettings);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// 设置消息内容。
        /// </summary>
        /// <param name="content">消息内容。</param>
        public void SetContent(Object content)
        {
            ContentJson = JsonConvert.SerializeObject(content, Message.JsonSettings);
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
            Message response = new Message(Token, Code);
            response.SetContent(returnValue);
            return response;
        }

        /// <summary>
        /// 获取对象的字符串表示。
        /// </summary>
        /// <returns>对象的字符串表示。</returns>
        public override String ToString()
        {
            return JsonConvert.SerializeObject(this, Message.JsonSettings);
        }

        /// <summary>
        /// 将JSON字符串转换为Message。
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>将JSON字符串转换为Message</returns>
        public static Message ToMessage(String json)
        {
            return JsonConvert.DeserializeObject<Message>(json, Message.JsonSettings);
        }

        /// <summary>
        /// 创建系统消息。
        /// </summary>
        /// <param name="token">消息唯一标识。</param>
        /// <param name="code">消息码。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>系统消息。</returns>
        internal static Message CreateSystemMessage(Int32 token, Int32 code, Object content = null)
        {
            Message message = new Message(token, code, true);
            if (content != null) message.SetContent(content);
            return message;
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
