using System;
using System.IO;
using System.Runtime.InteropServices;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataPump.Misc
{
    public class ItunsBackFileStategy : IProcessControllableStrategy
    {
        #region Methods

        #region Public

        public static Boolean IsItunsBackFile(DataPumpControllableExecutionContext context, out String backFilePath)
        {
            String path = context.GetContextData<String>("sourcePath");
            string key = "Manifest.mbdb";

            var res = Directory.GetFiles(path, key, SearchOption.AllDirectories);
            if (res.IsValid())
            {
                foreach (var resPath in res)
                {
                    if (!File.Exists(resPath.Replace(key, "Info.plist")))
                    {
                        continue;
                    }
                    if (!File.Exists(resPath.Replace(key, "Manifest.plist")))
                    {
                        continue;
                    }
                    if (!File.Exists(resPath.Replace(key, "Status.plist")))
                    {
                        continue;
                    }
                    backFilePath = new FileInfo(resPath).DirectoryName;
                    return true;
                }
            }

            key = "Manifest.db";
            res = Directory.GetFiles(path, key, SearchOption.AllDirectories);
            if (res.IsValid())
            {
                foreach (var resPath in res)
                {
                    if (!File.Exists(resPath.Replace(key, "Info.plist")))
                    {
                        continue;
                    }
                    if (!File.Exists(resPath.Replace(key, "Manifest.plist")))
                    {
                        continue;
                    }
                    if (!File.Exists(resPath.Replace(key, "Status.plist")))
                    {
                        continue;
                    }
                    backFilePath = new FileInfo(resPath).DirectoryName;
                    return true;
                }
            }
            backFilePath = null;
            return false;
        }

        public void InitExecution(DataPumpControllableExecutionContext context)
        {
            String destPath = context.GetContextData<String>("destPath");
            String sourcePath = context.GetContextData<String>("sourcePath");

            string target = Path.Combine(destPath, "data");
            var res = IOSDeviceCoreDll.AnalyzeItunesBackupDATAPWD(sourcePath, target, (f, t, a) =>
            {
                //asyn.Advance(0, string.Format(LanguageHelper.Get("LANGKEY_HuiFuWenJian_01478"), f));
                return 0;
            }, (b) =>
            {
                var password = context.GetContextData<String>("password") ?? String.Empty;
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

        public void Process(DataPumpControllableExecutionContext context)
        {
            if (context.Source.ItemType == Domains.SourceFileItemType.NormalPath)
            {
                String destPath = context.GetContextData<String>("destPath");

                string path = context.Source.Config.TrimEnd("#F");
                if (path.StartsWith("/data/data/"))
                {
                    path = ("/data/" + path.TrimStart("/data/data/"));
                }
                path = path.Replace("/", @"\");

                context.Source.Local = FileHelper.ConnectPath(destPath, "data", path);
            }
        }

        #endregion

        #endregion
    }
}
