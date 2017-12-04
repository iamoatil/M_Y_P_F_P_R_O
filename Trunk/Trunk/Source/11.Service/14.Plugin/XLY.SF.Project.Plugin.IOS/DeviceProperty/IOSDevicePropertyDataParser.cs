/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 16:57:01 
 * explain :  
 *
*****************************************************************************/

using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class IOSDevicePropertyDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public IOSDevicePropertyDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{3FFB8BFE-1B67-43A1-8177-DC9C7711F19B}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_DeviceProperty);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.IOS;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 0;

            pluginInfo.AppName = "com.app.ios";
            pluginInfo.Icon = "\\icons\\device.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_IosDeviceProperty);
            pluginInfo.SourcePath = new SourceFileItems();

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SimpleDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SimpleDataSource();
                ds.Type = typeof(KeyValueItem);
                ds.Items = new DataItems<KeyValueItem>(pi.SaveDbPath);

                BuildData(ds, pi);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取IOS设备信息数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        private void BuildData(SimpleDataSource dataSource, DataParsePluginInfo info)
        {
            string serialnumber = string.Empty;
            string name = string.Empty;
            string manufacture = string.Empty;
            string model = string.Empty;
            string OSType = string.Empty;
            string OSVersion = string.Empty;
            string root = string.Empty;
            string IMEI = string.Empty;
            string IMSI = string.Empty;
            string WiFiAddress = string.Empty;
            string BMac = string.Empty;
            string TMac = string.Empty;

            //从设备获取相关信息
            var device = info.Phone;
            if (null != device)
            {
                device.Properties = device.GetProperties();

                serialnumber = device.SerialNumber;
                name = device.Name;
                manufacture = device.Manufacture;
                model = device.Model;
                OSType = device.OSType.GetDescription();
                OSVersion = device.OSVersion;
                root = device.IsRootDesc;
                IMEI = device.IMEI;
                IMSI = device.IMSI;
                BMac = device.BMac;
                TMac = device.TMac;

                if (device.Properties.IsValid() && device.Properties.Keys.Contains("WiFiAddress"))
                {
                    WiFiAddress = device.Properties["WiFiAddress"];
                }
            }

            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_Serialnumber), serialnumber));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_DeviceName), name));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_Manufacture), manufacture));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_Model), model));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_OSType), OSType));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_OSVersion), OSVersion));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_IsJailbreak), root));
            dataSource.Items.Add(new KeyValueItem("IMEI", IMEI));
            dataSource.Items.Add(new KeyValueItem("IMSI", IMSI));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_WiFiAddress), WiFiAddress));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_BMac), BMac));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_TMac), TMac));
        }
    }
}
