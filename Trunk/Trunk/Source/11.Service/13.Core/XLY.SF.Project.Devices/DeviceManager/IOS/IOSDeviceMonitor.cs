using System;
using System.Collections.Generic;
using X64Service;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// IOS设备监控
    /// </summary>
    public sealed class IOSDeviceMonitor : AbstractDeviceMonitor
    {
        private readonly StartMonitoringCallbackDelegate _StartMonitoringCallback;

        /// <summary>
        /// Ios设备监听构造函数。
        /// </summary>
        public IOSDeviceMonitor()
        {
            _StartMonitoringCallback = DeviceMonitoringCallback;
        }

        public override bool Start()
        {
            //打开IOS设备监听
            //var m = IOSDeviceCoreDll.DeviceMount();
            var m = IOSDeviceCoreDll.StartDeviceMonitoring(_StartMonitoringCallback);
            if (m != 1)
            {
                LoggerManagerSingle.Instance.Error(string.Format("启动IOS设备监控失败, 错误码:{0}", m));
                return false;
            }

            return true;
        }

        public override void Close()
        {
            IOSDeviceCoreDll.CloseDeviceMonitoring();
        }

        private int DeviceMonitoringCallback(uint connetctStatus, string uniqueDeviceID)
        {
            var device = new Device(uniqueDeviceID);
            device.OSType = EnumOSType.IOS;
            device.Status = EnumDeviceStatus.Offline;

            if (connetctStatus == 1)
            {//新加入设备
                IOSDeviceManager iosDeviceManager = new IOSDeviceManager();
                device.DeviceManager = iosDeviceManager;

                Dictionary<string, string> properties = iosDeviceManager.GetProperties(device);
                device.Name = properties.ContainsKey("DeviceName") ? properties["DeviceName"] : "";
                device.Manufacture = LanguageManager.Current[Languagekeys.DeviceLanguage_DeviceType_IOS];
                device.Brand = "iPhone";
                device.Model = properties.ContainsKey("ProductType") ? properties["ProductType"] : "";
                device.OSVersion = properties.ContainsKey("ProductVersion") ? properties["ProductVersion"] : "";
                device.Status = EnumDeviceStatus.Online;
                device.IsRoot = properties.ContainsKey("IsJailbreak") ? properties["IsJailbreak"].ToUpper() != "NO" : false;
                device.IMEI = properties.ContainsKey("IMEI") ? properties["IMEI"] : "";
                device.SerialNumber = properties.ContainsKey("SerialNumber") ? properties["SerialNumber"] : device.ID;
                device.Properties = properties;

                /*
                 *  TotalDiskCapacity  总容量
                 *  TotalDataCapacity 数据区容量
                 *  TotalDataAvailable 数据区可用容量
                 *  TotalSystemCapacity 系统容量
                 *  MobileApplicationUsage 应用数据使用量
                 *  
                 * */
                var TotalDiskCapacity = properties.ContainsKey("TotalDiskCapacity") ? properties["TotalDiskCapacity"] : "0";
                //var TotalDataCapacity = properties.ContainsKey("TotalDataCapacity") ? properties["TotalDataCapacity"] : "0";
                var TotalDataAvailable = properties.ContainsKey("TotalDataAvailable") ? properties["TotalDataAvailable"] : "0";
                //var TotalSystemCapacity = properties.ContainsKey("TotalSystemCapacity") ? properties["TotalSystemCapacity"] : "0";
                //var MobileApplicationUsage = properties.ContainsKey("MobileApplicationUsage") ? properties["MobileApplicationUsage"] : "0";

                device.TotalDiskCapacity = UInt64.Parse(TotalDiskCapacity);
                device.TotalDiskAvailable = UInt64.Parse(TotalDataAvailable);
                device.TotalDataCapacity = 0;
                device.TotalDataAvailable = 0;


                //发出新设备连入通知
                OnConnected(device);
            }
            else if (connetctStatus == 2)
            {
                //断开设备通知
                OnDisconnected(device);
            }

            return 0;
        }

    }
}
