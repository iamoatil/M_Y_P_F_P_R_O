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

        public static readonly String[] Commands;

        private static readonly String ApkPath;

        private String _savePath;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.Android.AndroidUsbUnrootDataPump 。
        /// </summary>
        static AndroidUsbUnrootDataPump()
        {
            Commands = new[]
            {
                "basic_info","app_info","sms_info",
                "contact_info","calllog_info"
            };
            ApkPath = FileHelper.GetPhysicalPath(@"Toolkits\app\SPFSocket.apk");
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
            Device device = (Device)Metadata.Source;
            AndroidHelper.Instance.BackupAndResolve(device, FileHelper.ConnectPath(_savePath, $"{device.SerialNumber}.rar"));

            String content = String.Empty;
            foreach (String command in Commands)
            {
                content = AndroidHelper.Instance.ExecuteSPFAppCommand(device, command);
                File.WriteAllText(FileHelper.ConnectPath(_savePath, $"{command}.txt"), content);
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
                if (AndroidHelper.Instance.InstallPackage(ApkPath, device))
                {
                    String path = FileHelper.ConnectPath(Metadata.SourceStorePath, $"AndroidData_{Guid.NewGuid()}");
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    FileHelper.CreateDirectory(path);
                    _savePath = path;
                    return true;
                }
            }
            return false;
        }

        #endregion

        #endregion
    }
}
