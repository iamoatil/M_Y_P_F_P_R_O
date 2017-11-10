using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Language
{
    /// <summary>
    /// 描述包含语言的程序集中的语言文件。
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class LanguageDescriptorAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 LanguageDescriptorAttribute 实例。
        /// </summary>
        /// <param name="type">语言类型。</param>
        /// <param name="embedResourceName">嵌入的资源名称。</param>
        public LanguageDescriptorAttribute(LanguageType type, String embedResourceName)
        {
            Type = type;
            EmbedResourceName = embedResourceName ?? throw new ArgumentNullException("embedResourceName");
        }

        #endregion

        #region Properties

        /// <summary>
        /// 语言类型。
        /// </summary>
        public LanguageType Type { get; }

        /// <summary>
        /// 嵌入的资源名称。
        /// </summary>
        public String EmbedResourceName { get; }

        #endregion
    }
}
