using System;
using System.IO;
using System.IO.Compression;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataPump
{
    public class KuPaiBackFileStategy : IProcessControllableStrategy
    {
        private String _destPath;

        #region Methods

        #region Public

        public void InitExecution(string sourcePath, string destPath)
        {
            _destPath = destPath;

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
