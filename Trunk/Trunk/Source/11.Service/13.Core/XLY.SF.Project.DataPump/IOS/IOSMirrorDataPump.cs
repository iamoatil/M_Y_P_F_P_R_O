using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// IOS镜像数据泵。
    /// </summary>
    public class IOSMirrorDataPump : InitAtExecutionDataPump
    {
        #region Fields

        private String _destPath;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.IOSMirrorDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public IOSMirrorDataPump(Pump metadata)
            : base(metadata)
        {

        }

        #endregion

        #region Methods

        #region Protected

        protected override Boolean InitializeCore()
        {
            String mirrorFile = PumpDescriptor.Source as String;
            if (String.IsNullOrWhiteSpace(mirrorFile)) return false;

            String destPath = PumpDescriptor.SourceStorePath;
            if (Directory.Exists(destPath))
            {
                Directory.Delete(destPath, true);
            }
            _destPath = destPath;
            return true;
        }

        protected override Boolean InitAtFirstTime(DataPumpExecutionContext context)
        {
            String mirrorFile = (String)PumpDescriptor.Source;

            Framework.BaseUtility.WinRARCSharp.UnRAR(_destPath, FileHelper.GetFilePath(mirrorFile), FileHelper.GetFileName(mirrorFile));

            //3.处理app文件夹
            var directories = Directory.GetDirectories(_destPath);
            foreach (string path in directories)
            {
                if (path.Contains("AppDomain-"))
                {
                    Directory.Move(path, path.Replace("AppDomain-", String.Empty));
                }
            }
            return true;
        }

        protected override void OverrideExecute(DataPumpExecutionContext context)
        {
            if (context.Source.ItemType == Domains.SourceFileItemType.NormalPath)
            {
                context.Source.Local = FileHelper.ConnectPath(_destPath, context.Source.Config.Replace('/', '\\'));
            }
        }

        #endregion

        #endregion
    }
}
