using System;
using System.Windows.Data;

/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/2 15:50:56
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.PreviewFiles.Language
{
    /// <summary>
    /// 语言类型
    /// </summary>
    public enum LanguageType
    {
        /// <summary>
        /// 英语
        /// </summary>
        En,
        /// <summary>
        /// 中文
        /// </summary>
        Cn
    }

    public class PageLanguageHelper
    {
        static PageLanguageHelper()
        {
            XmlProvider = new XmlDataProvider();
            XmlProvider.Source = new Uri("Pack://application:,,,/XLY.SF.Project.PreviewFiles;Component/Language/Language_Cn.xml", UriKind.RelativeOrAbsolute);
            XmlProvider.XPath = "LanguageResource";
        }

        /// <summary>
        /// 加载界面语言
        /// </summary>
        public static void LoadPageLanguage(Uri packUri)
        {
            if (packUri != null)
            {
                XmlProvider.Source = packUri;
                XmlProvider.XPath = "LanguageResource";
            }
        }

        public static XmlDataProvider XmlProvider { get; private set; }
    }
}
