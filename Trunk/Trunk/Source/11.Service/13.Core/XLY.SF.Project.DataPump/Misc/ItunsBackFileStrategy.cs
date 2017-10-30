using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.BaseUtility;

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
                    backFilePath = resPath.TrimEnd(key);
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
                    backFilePath = resPath.TrimEnd(key);
                    return true;
                }
            }
            backFilePath = null;
            return false;
        }

        public void Process(DataPumpControllableExecutionContext context)
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
            if (0 != res)
            {
                // LogHelper.Error(LanguageHelper.Get("LANGKEY_BenDiTiQuHuiFuitunsBeiFenShuJu_01479") + res);
            }
            else
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

                        //asyn.Advance(0, string.Format(LanguageHelper.Get("LANGKEY_ChuLiAPPWenJianJia_01480"), newFileName));
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
