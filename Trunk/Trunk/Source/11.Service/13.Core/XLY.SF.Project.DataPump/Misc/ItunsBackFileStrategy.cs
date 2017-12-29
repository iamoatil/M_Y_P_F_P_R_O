using System;
using System.IO;
using System.Runtime.InteropServices;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataPump
{
    public class ItunsBackFileStategy : IProcessControllableStrategy
    {
        private String _destPath;

        #region Methods

        #region Public

        public void InitExecution(string sourcePath, string destPath)
        {
            _destPath = destPath;
            string target = Path.Combine(destPath, "data");
            var res = IOSDeviceCoreDll.AnalyzeItunesBackupDATAPWD(sourcePath, target, (f, t, a) =>
            {
                //asyn.Advance(0, string.Format(LanguageHelper.Get("LANGKEY_HuiFuWenJian_01478"), f));
                return 0;
            }, (b) =>
            {
                var password = string.Empty;
                var pS = Marshal.StringToHGlobalAnsi(password);
                Marshal.WriteIntPtr(b, pS);
                return 0;
            });
            if (0 == res)
            {
                string[] files = Directory.GetDirectories(target);
                foreach (string file in files)
                {
                    var fileName = Path.GetFileName(file);
                    //第三方应用
                    if (!fileName.Contains("com.apple.") && fileName.Contains("AppDomain-"))
                    {
                        var newFileName = fileName.Replace("AppDomain-", "");
                        Directory.Move(Path.Combine(target, fileName), Path.Combine(target, newFileName));
                    }
                }
            }
        }

        public void Process(DataPumpExecutionContext context)
        {
            var si = context.Source;
            if (si.ItemType == Domains.SourceFileItemType.NormalPath)
            {
                string path = si.Config.TrimEnd("#F");
                if (path.StartsWith("/data/data/"))
                {
                    path = path.TrimStart("/data/data/");
                }
                else if (path.StartsWith("/data/"))
                {
                    path = path.TrimStart("/data/");
                }
                path = path.Replace("/", @"\");

                var local = Path.Combine(_destPath, "data", path.Replace('/', '\\').TrimStart('\\'));
                if (FileHelper.IsValidDictory(local) || FileHelper.IsValid(local))
                {
                    si.Local = local;
                }
            }
        }

        #endregion

        #endregion
    }
}
