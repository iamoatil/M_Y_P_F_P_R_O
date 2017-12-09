using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.Android
{
    /// <summary>
    /// Android 未ROOT数据泵。
    /// </summary>
    public class AndroidUsbUnrootDataPump : ControllableDataPumpBase
    {
        #region Fields

        /// <summary>
        /// SPFSocket.apk文件路径
        /// </summary>
        private static readonly String ApkPath;

        /// <summary>
        /// SPFSocket.apk 支持的命令
        /// </summary>
        public static readonly String[] Commands;

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
        /// 初始化类型 XLY.SF.Project.DataPump.Android.AndroidUsbUnrootDataPump 。
        /// </summary>
        static AndroidUsbUnrootDataPump()
        {
            Commands = new[]
            {
                "base_info","app_info","browser_info","sms_info",
                "contact_info","calllog_info","location_info","account_info"
            };
            ApkPath = FileHelper.GetPhysicalPath(@"Lib\adb\SPFSocket.apk");
        }

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.Android.AndroidUsbUnrootDataPump 实例。
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
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            var sfi = context.Source;
            switch (sfi.ItemType)
            {
                case SourceFileItemType.AndroidCmdPath:
                    sfi.Local = Path.Combine(APPCmdSavePath, $"{sfi.APPCmd}.bin");
                    break;
                case SourceFileItemType.AndroidSDCardPath:
                    break;
                case SourceFileItemType.NormalPath:
                    break;
            }
        }

        /// <summary>
        /// 初始化当前的执行流程。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        protected override Boolean InitializeCore()
        {
            if (Metadata.Source is Device device)
            {
                //1.植入APP
                APPCmdSavePath = FileHelper.ConnectPath(Metadata.SourceStorePath, "CmdData");
                FileHelper.CreateExitsDirectory(APPCmdSavePath);

                if (FileHelper.IsValid(ApkPath) && AndroidHelper.Instance.InstallPackage(ApkPath, device))
                {
                    String content = String.Empty;
                    foreach (String command in Commands)
                    {
                        content = AndroidHelper.Instance.ExecuteSPFAppCommand(device, command);
                        File.WriteAllText(FileHelper.ConnectPath(APPCmdSavePath, $"{command}.bin"), content);
                    }
                }

                //2.ADB备份
                BackupSavePath = FileHelper.ConnectPath(Metadata.SourceStorePath, "Backup");
                FileHelper.CreateExitsDirectory(BackupSavePath);

                //var rarFile = AndroidHelper.Instance.BackupAndResolve(device, FileHelper.ConnectPath(BackupSavePath, $"{device.SerialNumber}.rar"));
            }
            return true;
        }

        #endregion

        #endregion
    }
}
