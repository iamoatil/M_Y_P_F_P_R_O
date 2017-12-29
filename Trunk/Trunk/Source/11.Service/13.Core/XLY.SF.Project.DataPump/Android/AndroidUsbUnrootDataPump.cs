using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// Android 未ROOT数据泵。
    /// </summary>
    public class AndroidUsbUnrootDataPump : DataPumpBase
    {
        #region Fields

        /// <summary>
        /// SPFSocket 命令结果保存路径
        /// </summary>
        private String APPCmdSavePath { get; set; }

        /// <summary>
        /// ADB备份保存路径
        /// </summary>
        private String BackupSavePath { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.AndroidUsbUnrootDataPump 。
        /// </summary>
        static AndroidUsbUnrootDataPump()
        {
        }

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.AndroidUsbUnrootDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public AndroidUsbUnrootDataPump(Pump metadata)
            : base(metadata)
        {
        }

        #endregion

        #region Methods

        #region protected

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpExecutionContext context)
        {
            var sfi = context.Source;
            switch (sfi.ItemType)
            {
                case SourceFileItemType.AndroidCmdPath:
                    var cmdFile = Path.Combine(APPCmdSavePath, $"{sfi.APPCmd}.bin");
                    if (FileHelper.IsValid(cmdFile))
                    {
                        sfi.Local = cmdFile;
                    }
                    break;
                case SourceFileItemType.AndroidSDCardPath:
                    Device device = context.PumpDescriptor.Source as Device;
                    String path = FileHelper.ConnectLinuxPath(device.SDCardPath, context.Source.SDCardConfig);
                    context.Source.Local = device.CopyFile(path, context.TargetDirectory, null);
                    break;
                case SourceFileItemType.NormalPath:
                    var dp = Path.Combine(BackupSavePath, context.Source.Config.TrimEnd("#F").Replace('/', '\\').TrimStart("\\"));
                    if (FileHelper.IsValidDictory(dp) || FileHelper.IsExist(dp))
                    {
                        context.Source.Local = dp;
                    }
                    break;
            }
        }

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitializeCore()
        {
            if (PumpDescriptor.Source is Device device)
            {
                //1.植入APP
                APPCmdSavePath = FileHelper.ConnectPath(PumpDescriptor.SourceStorePath, "CmdData");
                AndroidHelper.Instance.InstallPackageGetData(device, APPCmdSavePath);

                //2.ADB备份
                BackupSavePath = FileHelper.ConnectPath(PumpDescriptor.SourceStorePath, "Backup");
                FileHelper.CreateExitsDirectory(BackupSavePath);

                var rarFile = AndroidHelper.Instance.BackupAndResolve(device, FileHelper.ConnectPath(BackupSavePath, $"{device.SerialNumber}.rar"));

                ResolveAndroidBackupFile(rarFile);
            }
            return true;
        }

        private void ResolveAndroidBackupFile(string rarFilePath)
        {
            try
            {
                if (!FileHelper.IsValid(rarFilePath))
                {
                    return;
                }

                //1.解压
                SevenZipHelper.ExtractArchive(rarFilePath, Path.Combine(BackupSavePath, "data"));
                FileHelper.DeleteFileSafe(rarFilePath);

                //2.获取apps文件夹
                var appsPath = Path.Combine(BackupSavePath, "data\\apps");
                if (!FileHelper.IsValidDictory(appsPath))
                {
                    return;
                }
                FileHelper.ReNameDirectory(Path.Combine(BackupSavePath, "data"), "apps", "data");

                //3.处理应用文件夹
                foreach (var appDir in Directory.GetDirectories(Path.Combine(BackupSavePath, "data\\data")))
                {
                    FileHelper.ReNameDirectory(appDir, "db", "databases");
                    FileHelper.ReNameDirectory(appDir, "f", "files");
                    FileHelper.ReNameDirectory(appDir, "sp", "shared_prefs");
                    FileHelper.MoveDirectory(Path.Combine(appDir, "r"), appDir);
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "处理安卓手机adb备份失败！");
            }
        }


        #endregion

        #endregion
    }
}
