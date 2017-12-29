using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump
{
    /// <summary>
    /// Android USB数据泵。
    /// </summary>
    public class AndroidUsbDataPump : DataPumpBase
    {

        /// <summary>
        /// SPFSocket 命令结果保存路径
        /// </summary>
        private String APPCmdSavePath { get; set; }

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.AndroidUsbDataPump 实例。
        /// </summary>
        /// <param name="metadata">与此数据泵关联的元数据信息。</param>
        public AndroidUsbDataPump(Pump metadata)
            : base(metadata)
        {
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpExecutionContext context)
        {
            Device device = context.PumpDescriptor.Source as Device;
            if (device == null) return;
            switch (context.Source.ItemType)
            {
                case SourceFileItemType.AndroidSDCardPath:
                    HandleWithSDCardPath(device, context);
                    break;
                case SourceFileItemType.NormalPath:
                    HandleWithNormalPath(device, context);
                    break;
                case SourceFileItemType.AndroidCmdPath:
                    var cmdFile = Path.Combine(APPCmdSavePath, $"{context.Source.APPCmd}.bin");
                    if (FileHelper.IsValid(cmdFile))
                    {
                        context.Source.Local = cmdFile;
                    }
                    break;
                default:
                    break;
            }
        }

        protected override bool InitializeCore()
        {
            if (PumpDescriptor.Source is Device device)
            {
                //植入APP
                APPCmdSavePath = FileHelper.ConnectPath(PumpDescriptor.SourceStorePath, "CmdData");
                AndroidHelper.Instance.InstallPackageGetData(device, APPCmdSavePath);
            }

            return true;
        }

        #endregion

        #region Private

        private void HandleWithSDCardPath(Device device, DataPumpExecutionContext context)
        {
            SourceFileItem source = context.Source;
            String path = FileHelper.ConnectLinuxPath(device.SDCardPath, source.SDCardConfig);
            source.Local = device.CopyFile(path, context.TargetDirectory, null);
        }

        private void HandleWithNormalPath(Device device, DataPumpExecutionContext context)
        {
            String path = context.Source.Config;
            context.Source.Local = device.CopyFile(path, context.TargetDirectory, null);

            if (path == "/data/data/com.tencent.mm/MicroMsg/#F")
            {//查找微信分身
                try
                {
                    var ah = AndroidHelper.Instance;
                    foreach (var pa in ah.FindFiles(device, "/data/data/#F").Where(f => f.IsFolder && f.Name != "com.tencent.mm"))
                    {
                        if (ah.FindFiles(device, $"{pa.FullPath}#F").Any(f => f.IsFolder && f.Name == "MicroMsg"))
                        {
                            device.CopyFile(pa.FullPath, context.TargetDirectory, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    XLY.SF.Framework.Log4NetService.LoggerManagerSingle.Instance.Error(ex, "安卓手机USB提取查找微信分身出错！");
                }
            }
        }

        #endregion

        #endregion
    }
}
