using System.IO;
using System.IO.Compression;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataMirror
{
    /// <summary>
    /// IOS手机镜像服务
    /// </summary>
    public class IOSDeviceMirrorService : AbstractMirrorService
    {
        private IOSDeviceManager DeviceManager { get; set; }

        /// <summary>
        /// 是否用户停止镜像
        /// </summary>
        private bool IsUserStop { get; set; }

        public override void Execute(Mirror mirror, DefaultAsyncTaskProgress asyn)
        {
            var device = mirror.Source as Device;

            IsUserStop = false;
            DeviceManager = device.DeviceManager as IOSDeviceManager;

            //数据缓存路径
            var tempSavePath = FileHelper.ConnectPath(mirror.Target, "temp");
            FileHelper.CreateExitsDirectorySafe(tempSavePath);

            //数据备份
            var resPath = DeviceManager.CopyUserData(device, tempSavePath, asyn);

            if (!IsUserStop)
            {//镜像结束
                if (FileHelper.IsValidDictory(resPath))
                {
                    asyn?.OnProgress(string.Empty, 0.99, "数据拷贝完成, 准备后期合并处理……");

                    var name = $"{System.Guid.NewGuid().ToString()}.zip";

                    Framework.BaseUtility.WinRARCSharp.RAR(resPath, mirror.Target, name);

                    File.Move(Path.Combine(mirror.Target, name), mirror.Local);
                }
            }

            //删除缓存文件
            FileHelper.DeleteDirectorySafe(tempSavePath);
        }

        public override void Stop()
        {
            IsUserStop = true;
            DeviceManager?.StopCopyUserData();
            DeviceManager = null;
        }
    }
}
