using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 消息。
    /// </summary>
    public class Message
    {
        #region Constructors

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
        /// <param name="code">消息码。</param>
        /// <param name="arg">消息参数。</param>
        public Message(Int32 code, Object arg)
            : this(Guid.NewGuid().GetHashCode(), code, arg)
        {
        }

        /// <summary>
        /// 初始化类型 Message 实例。
        /// </summary>
        /// <param name="token">消息唯一标识。</param>
        /// <param name="code">消息码。</param>
        /// <param name="arg">消息参数。</param>
        private Message(Int32 token, Int32 code, Object arg)
        {
            Code = code;
            Arg = arg;
        }

        /// <summary>
        /// 初始化类型 Message 实例。
        /// </summary>
        public Message()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// 命令唯一标识。
        /// </summary>
        public Int32 Token { get;  set; }

        /// <summary>
        /// 命令码。
        /// </summary>
        public Int32 Code { get; set; }

        /// <summary>
        /// 命令参数。
        /// </summary>
        public Object Arg { get; set; }

        #endregion

        #region Methods

        #region Public

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
            return $"{Token};{Code};{Arg}";
        }

        #endregion

        #endregion
    }
}
