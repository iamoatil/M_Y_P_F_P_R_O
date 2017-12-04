using XLY.SF.Framework.Language;

namespace XLY.SF.Project.Plugin.Language
{
    public static class LanguageHelper
    {
        static LanguageHelper()
        {
            LanguageManager = LanguageManager.Current;
        }

        private static LanguageManager LanguageManager { get; }


        public static string GetString(string key)
        {
            return LanguageManager[key];
        }
    }
}
