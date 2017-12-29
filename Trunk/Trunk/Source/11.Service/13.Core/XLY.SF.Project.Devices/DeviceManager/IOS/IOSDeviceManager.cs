using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using X64Service;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// IOS设备管理服务
    /// </summary>
    [Serializable]
    public sealed class IOSDeviceManager : IDeviceManager
    {
        /// <summary>
        /// 拷贝用户数据委托。
        /// 非托管回调的方法必须声明为全局变量，否则可能被垃圾器回收导致C++接口反复调用时出错。
        /// </summary>
        private CopyUserDataCallbackDelegate _CopyUserDataCallback;

        /// <summary>
        /// 设备属性列表
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, string>> DevicesProperties;
        private readonly Dictionary<uint, string> ErrorMsgDic;

        /// <summary>
        /// IOS设备管理服务
        /// IOSDeviceManager没有使用单例模式
        /// 因为CopyUserData和StopCopyUserData使用了成员属性。多个设备同时调用的话会出问题。
        /// </summary>
        public IOSDeviceManager()
        {
            DevicesProperties = new Dictionary<string, Dictionary<string, string>>();

            ErrorMsgDic = new Dictionary<uint, string>
            {
                { 338, LanguageManager.Current[Languagekeys.ErrorMessageLanguage_ErrMsg_NotFindTable] },
                { 340, LanguageManager.Current[Languagekeys.ErrorMessageLanguage_ErrMsg_NotDeviceConnect] },
                { 321, LanguageManager.Current[Languagekeys.ErrorMessageLanguage_ErrMsg_WriteFileFail] },
                { 322, LanguageManager.Current[Languagekeys.ErrorMessageLanguage_ErrMsg_PhoneAccessFail] },
                { 323, LanguageManager.Current[Languagekeys.ErrorMessageLanguage_ErrMsg_NotExistAccessFile] },
                { 301, LanguageManager.Current[Languagekeys.ErrorMessageLanguage_ErrMsg_ReadFileFail] }
            };
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="errorID">错误码</param>
        /// <returns></returns>
        private string GetErrorMsg(uint errorID)
        {
            if (ErrorMsgDic.ContainsKey(errorID))
            {
                return ErrorMsgDic[errorID];
            }
            else
            {
                return string.Format("{0}:{1}", LanguageManager.Current[Languagekeys.ErrorMessageLanguage_ErrMsg_IOSDllErrorID], errorID);
            }
        }

        /// <summary>
        /// 从IOS手机拷贝文件
        /// </summary>
        /// <param name="device"></param>
        /// <param name="source"></param>
        /// <param name="targetPath"></param>
        /// <param name="asyn"></param>
        /// <returns></returns>
        public string CopyFile(Device device, string source, string targetPath, DefaultAsyncTaskProgress asyn)
        {
            return CopyUserData(device, targetPath, asyn);
        }

        #region CopyUserData

        /// <summary>
        /// 停止文件拷贝
        /// </summary>
        public void StopCopyUserData()
        {
            IsStop = true;
        }

        /// <summary>
        /// 采用备份方式拷贝用户数据 支持加密备份 支持IOS8.3及其以上系统
        /// </summary>
        /// <param name="device">设备</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="asyn"></param>
        /// <param name="InputPassword">回调方法，如果有密码则调用，在方法内返回密码</param>
        /// <returns></returns>
        public string CopyUserData(Device device, string targetPath, DefaultAsyncTaskProgress asyn, Func<string> InputPassword = null)
        {
            //1.初始化相关参数
            InitCopyUserData(device, 100, asyn);

            //2.文件拷贝
            var res = IOSDeviceCoreDll.CopyUserDataPWD(targetPath, device.ID, CopyUserDataCallback, (b) =>
            {
                if (null != InputPassword)
                {
                    var password = InputPassword();

                    if (password.IsValid())
                    {
                        var pS = Marshal.StringToHGlobalAnsi(password);

                        Marshal.WriteIntPtr(b, pS);
                    }
                }

                return 0;
            });

            if (0 != res)
            {
                IsCopying = false;
            }
            else
            {
                while (IsCopying)
                {
                    Thread.Sleep(500);
                }
            }

            //3.清理
            ClearCopyUserData();

            return Path.Combine(targetPath, device.ID);
        }

        private void InitCopyUserData(Device device, double totalProgress, DefaultAsyncTaskProgress asyn)
        {
            _CopyUserDataCallback = CopyUserDataCallback;

            CurrentDeviceName = device.Model;
            IsCopying = true;
            IsStop = false;
            Asyn = asyn;
        }

        private void ClearCopyUserData()
        {
            IsCopying = false;
            IsStop = true;
            Asyn = null;
        }

        private string CurrentDeviceName;
        private bool IsCopying;
        private bool IsStop;
        private DefaultAsyncTaskProgress Asyn;

        private int CopyUserDataCallback(string uniqueDeviceID, byte step, float status, ref UInt32 isStopCopy)
        {
            if (IsStop)
            {//停止文件拷贝
                isStopCopy = 1;
                return 0;
            }
            else
            {
                isStopCopy = 0;
            }

            string copyed = string.Empty;
            double progress = 0;
            string msg = string.Empty;

            switch (step)
            {
                case 1:// 拷贝前初始化 status为进度 0-100
                    msg = string.Format(LanguageManager.Current[Languagekeys.DeviceLanguage_IOSInitProgress], status);
                    progress = 0.3 * status * 0.01;
                    break;
                case 2:// 数据拷贝 status为已经拷贝数据大小，单位为 KB
                    copyed = (status * 1024).ToSafeString();
                    msg = string.Format(LanguageManager.Current[Languagekeys.DeviceLanguage_IOSCopyProgress], (status / 1024.0).ToString("F2"));
                    progress = 0.6;
                    break;
                case 3:// 拷贝结束，进行后期合并
                    if (1 == status)
                    {
                        IsCopying = false;
                        msg = LanguageManager.Current[Languagekeys.DeviceLanguage_IOSExtractLocalOver];
                        progress = 0.99;
                    }
                    else
                    {
                        msg = string.Format(LanguageManager.Current[Languagekeys.DeviceLanguage_IOSLastStepProgress], (status * 300).ToString("F2"));
                        progress = 0.9 + status / 3;
                    }
                    break;
                default:// 拷贝失败或者用户终止
                    IsCopying = false;

                    msg = step == 4 ? LanguageManager.Current[Languagekeys.DeviceLanguage_IOSCopyError] : LanguageManager.Current[Languagekeys.DeviceLanguage_IOSUserExit];

                    LoggerManagerSingle.Instance.Warn(string.Format(LanguageManager.Current[Languagekeys.DeviceLanguage_IOSDeviceCopyError], uniqueDeviceID, step, msg));
                    break;
            }

            Asyn?.OnProgress(copyed, progress, msg);

            return 0;
        }

        #endregion

        /// <summary>
        /// 获取设备属性列表
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetProperties(Device device)
        {
            if (DevicesProperties.ContainsKey(device.ID))
            {
                return DevicesProperties[device.ID];
            }

            IntPtr devicePropertyPointer = IntPtr.Zero;
            uint execResultCode = IOSDeviceCoreDll.GetDeviceProperties(device.ID, ref devicePropertyPointer);
            var dictionary = new Dictionary<string, string>();
            if (execResultCode != 0)
            {
                return dictionary;
            }

            var properties = (DevicePropertyList)Marshal.PtrToStructure(devicePropertyPointer, typeof(DevicePropertyList));
            IntPtr nodePointer = properties.PropertyNodes;

            //读取每一个属性值。根据DeviceProperty大小，逐大小读取。
            for (int i = 0; i < properties.Length; i++)
            {
                var deviceProperty = (DeviceProperty)Marshal.PtrToStructure(nodePointer, typeof(DeviceProperty));
                dictionary.Add(deviceProperty.PropertyKey, deviceProperty.PropertyValue);
                nodePointer += Marshal.SizeOf(typeof(DeviceProperty));
            }

            DevicesProperties.Add(device.ID, dictionary);
            return dictionary;
        }

        /// <summary>
        /// 获取设备安装应用列表
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public List<AppEntity> FindInstalledApp(Device device)
        {
            var appPointer = IntPtr.Zero;
            uint execResultCode = IOSDeviceCoreDll.GetALLInstalledApp(device.ID, ref appPointer);
            var appEntities = new List<AppEntity>();

            if (execResultCode != 0)
            {
                return appEntities;
            }

            var apps = (InstalledAppList)Marshal.PtrToStructure(appPointer, typeof(InstalledAppList));
            IntPtr appNodes = apps.AppNodes;

            //读取每一个应用实体。根据InstalledApp大小，逐大小读取。
            for (int i = 0; i < apps.Length; i++)
            {
                var appInfo = (InstalledApp)Marshal.PtrToStructure(appNodes, typeof(InstalledApp));
                appEntities.Add(new AppEntity { VersionDesc = appInfo.Version, AppId = appInfo.AppId, Name = appInfo.Name, InstallPath = appInfo.AppInstallPath, InstallDate = appInfo.InstallTime.ToSafeDateTime() });
                appNodes += Marshal.SizeOf(typeof(InstalledApp));
            }

            return appEntities;
        }

        /// <summary>
        /// 获取设备分区信息
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public List<Partition> GetPartitons(Device device)
        {
            var p = new Partition();
            var list = new List<Partition>();
            p.Text = LanguageManager.Current[Languagekeys.DeviceLanguage_Partition_data];
            p.Name = "data";
            list.Add(p);
            return list;
        }

        /// <summary>
        /// 判断手机是否越狱
        /// </summary>
        public bool IsRoot(Device device)
        {
            if (!DevicesProperties.ContainsKey(device.ID))
            {
                DevicesProperties.Add(device.ID, GetProperties(device));
            }

            Dictionary<string, string> dictionary = DevicesProperties[device.ID];
            string isJailbreak = dictionary["IsJailbreak"];
            if (isJailbreak.ToUpper() == "NO")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 判断手机是否可用
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool IsValid(Device device)
        {
            uint execResultCode = IOSDeviceCoreDll.CheckDeviceIsOnline(device.ID);

            if (execResultCode == 0)
            {
                return true;
            }
            else
            {
                LoggerManagerSingle.Instance.Error(GetErrorMsg(execResultCode));
                return false;
            }
        }

        #region 暂不支持的操作

        /// <summary>
        /// 清除屏幕锁
        /// </summary>
        public void ClearScreenLock(Device device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 恢复屏幕锁
        /// </summary>
        public void RecoveryScreenLock(Device device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查找所有已卸载应用列表，暂时未实现。
        /// </summary>
        public List<AppEntity> FindUnInstalledApp(Device device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取SD卡路径
        /// </summary>
        public string GetSDCardPath(Device device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 读取手机指定文件目录的内容
        /// </summary>
        public string ReadFile(Device device, string file)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
