using System.Windows.Data;
using XLY.SF.Framework.Language;

namespace XLY.SF.Project.PreviewFilesView
{
    internal static class LanguageHelper
    {
        static LanguageHelper()
        {
            LanguageProvider = new XmlDataProvider
            {
                XPath = "LanguageResource"
            };

            LanguageManager lm = LanguageManager.Current;
            lm.Switched += (a, b) => LanguageProvider.Document = LanguageManager.Document;
            LanguageManager = lm;
#if DEBUG
            lm.Switch(LanguageType.Cn);
#endif
        }

        public static LanguageManager LanguageManager { get; }

        public static XmlDataProvider LanguageProvider { get; } 
    }
}
