using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataPump.Misc
{
    public class KuPaiBackFileStategy : IProcessControllableStrategy
    {
        #region Methods

        #region Public

        public static Boolean IsKuPaiBackFile(DataPumpControllableExecutionContext context, out String backFilePath)
        {
            String path = context.GetContextData<String>("sourcePath");
            string key = "recentcalls.zip";

            var res = Directory.GetFiles(path, key, SearchOption.AllDirectories);
            if (res.IsValid())
            {
                backFilePath = new FileInfo(res[0]).DirectoryName;
                return true;
            }
            backFilePath = null;
            return false;
        }

        public void Process(DataPumpControllableExecutionContext context)
        {
            String destPath = context.GetContextData<String>("destPath");
            String sourcePath = context.GetContextData<String>("sourcePath");

            //contacts.zip
            String sourceFile = Path.Combine(sourcePath, "contacts.zip");
            if (FileHelper.IsValid(sourceFile))
            {
                //asyn.Advance(0, String.Format(LanguageHelper.Get("LANGKEY_KaiShiJieYa_01484"), sourceFile));
                ZipFile.ExtractToDirectory(sourceFile, destPath.TrimEnd('/', '\\') + @"\data\contacts");
                //asyn.Advance(0, String.Format(LanguageHelper.Get("LANGKEY_JieYaChengGong_01486"), sourceFile));
            }

            //recentcalls.zip
            sourceFile = Path.Combine(sourcePath, "recentcalls.zip");
            if (FileHelper.IsValid(sourceFile))
            {
                //asyn.Advance(0, String.Format(LanguageHelper.Get("LANGKEY_KaiShiJieYa_01484"), sourceFile));
                ZipFile.ExtractToDirectory(sourceFile, destPath.TrimEnd('/', '\\') + @"\data\recentcalls");
                //asyn.Advance(0, String.Format(LanguageHelper.Get("LANGKEY_JieYaChengGong_01486"), sourceFile));
            }

            //sms.zip
            sourceFile = Path.Combine(sourcePath, "sms.zip");
            if (FileHelper.IsValid(sourceFile))
            {
                //asyn.Advance(0, String.Format(LanguageHelper.Get("LANGKEY_KaiShiJieYa_01484"), sourceFile));
                ZipFile.ExtractToDirectory(sourceFile, destPath.TrimEnd('/', '\\') + @"\data\sms");
                //asyn.Advance(0, String.Format(LanguageHelper.Get("LANGKEY_JieYaChengGong_01486"), sourceFile));
            }
        }

        #endregion

        #endregion
    }
}
