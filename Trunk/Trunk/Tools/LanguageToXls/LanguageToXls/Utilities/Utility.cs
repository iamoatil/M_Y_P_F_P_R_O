using System.IO;

namespace LanguageToXls
{
    public class UtilityPath
    {
        public static string ToStandardDirectory(string dir)
        {
            dir = Path.GetFullPath(dir);
            if (!dir.EndsWith("\\"))
            {
                dir += "\\";
            }
            return dir;
        }
    }
}
