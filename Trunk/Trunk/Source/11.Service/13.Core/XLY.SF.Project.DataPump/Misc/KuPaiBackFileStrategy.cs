using System;
using System.IO;
using System.IO.Compression;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataPump.Misc
{
    public class KuPaiBackFileStategy : IProcessControllableStrategy
    {
        #region Methods

        #region Public

        public void InitExecution(DataPumpControllableExecutionContext context)
        {
            String destPath = context.GetContextData<String>("destPath");
            String sourcePath = context.GetContextData<String>("sourcePath");

            //contacts.zip
            String sourceFile = Path.Combine(sourcePath, "contacts.zip");
            if (FileHelper.IsValid(sourceFile))
            {
                var savePath = Path.Combine(destPath, @"data\contacts");
                FileHelper.CreateDirectory(savePath);

                ZipFile.ExtractToDirectory(sourceFile, savePath);
            }

            //recentcalls.zip
            sourceFile = Path.Combine(sourcePath, "recentcalls.zip");
            if (FileHelper.IsValid(sourceFile))
            {
                var savePath = Path.Combine(destPath, @"data\recentcalls");
                FileHelper.CreateDirectory(savePath);

                ZipFile.ExtractToDirectory(sourceFile, savePath);
            }

            //sms.zip
            sourceFile = Path.Combine(sourcePath, "sms.zip");
            if (FileHelper.IsValid(sourceFile))
            {
                var savePath = Path.Combine(destPath, @"data\sms");
                FileHelper.CreateDirectory(savePath);

                ZipFile.ExtractToDirectory(sourceFile, savePath);
            }
        }

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
