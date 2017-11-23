using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using XLY.SF.Framework.Language;

namespace XLY.SF.Project.DataDisplayView
{
    internal static class LanguageHelper
    {
        static LanguageHelper()
        {
            LanguageManager lm = LanguageManager.Current;
            LanguageProvider = new XmlDataProvider
            {
                XPath = "LanguageResource",
                Document = lm.Document
            };
            lm.Switched += (a, b) =>
            {
                LanguageProvider.Document = LanguageManager.Document;
            };
            LanguageManager = lm;
        }

        public static LanguageManager LanguageManager { get; }

        public static XmlDataProvider LanguageProvider { get; }

        public static string Get(string key)
        {
            return LanguageHelper.LanguageManager[key];
        }
    }

    /// <summary>
    /// 自定义的Xaml中的语言绑定
    /// </summary>
    public class Lang : MarkupExtension
    {
        public string Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return LanguageHelper.LanguageManager["//" + Key];
        }
    }
    
}
