/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 16:57:01 
 * explain :  
 *
*****************************************************************************/

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidDevicePropertyDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidDevicePropertyDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{BE64DDEE-6CB6-41E9-99DC-614AD88978D5}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_DeviceProperty);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 0;

            pluginInfo.AppName = "com.app.android";
            pluginInfo.Icon = "\\icons\\device.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidDeviceProperty);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.oppo.safe/shared_prefs/TMSProperties.xml");
            pluginInfo.SourcePath.AddItem("/data/misc/bluetoothd/#F");
            pluginInfo.SourcePath.AddItem("/data/misc/bluedroid/bt_config.xml");
            pluginInfo.SourcePath.AddItem("/data/misc/dhcp/dhcpcd-wlan0.lease");
            pluginInfo.SourcePath.AddItem("/data/misc/dhcp/dhcpcd-wlan0.lease_0");
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.telephony/databases/telephony.db");
            pluginInfo.SourcePath.AddItem("/data/data/com.samsung.simcardmanagement/databases/simcardmanagement.db");
            pluginInfo.SourcePath.AddItem("/data/system/users/0/settings_global.xml");
            pluginInfo.SourcePath.AddItem("APPCmd:base_info");

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
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓设备信息数据出错！", ex);
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
            List<string> listsimdata = new List<string>();

            //从设备获取相关信息
            var device = info.Phone;
            if (null != device)
            {
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

            //获取蓝牙MAC属性
            try
            {
                // /data/data/com.oppo.safe/shared_prefs/TMSProperties.xml
                if (FileHelper.IsValid(info.SourcePath[0].Local))
                {//其它手机
                    XDocument doc = XDocument.Load(info.SourcePath[0].Local);
                    var maps = doc.Elements("map").ToList().FirstOrDefault();
                    var mac = maps.Elements("string").ToList();
                    mac = mac.Where(p => p.Attribute("name").Value == "wup.mac").ToList();

                    BMac = mac.FirstOrDefault().ToSafeString().ToUpper();
                }
                // /data/misc/bluetoothd
                else if (FileHelper.IsValidDictory(info.SourcePath[1].Local))
                {//三星手机
                    string cfgFile = Directory.GetFiles(info.SourcePath[1].Local, "names", SearchOption.AllDirectories).FirstOrDefault();
                    if (cfgFile.IsValid())
                    {
                        string macFolder = Path.GetDirectoryName(cfgFile).Split('\\')[Path.GetDirectoryName(cfgFile).Split('\\').Length - 1];

                        BMac = macFolder.Replace("_", ":").ToUpper();
                    }
                }
                // /data/misc/bluedroid/bt_config.xml
                else if (FileHelper.IsValid(info.SourcePath[2].Local))
                {//华为手机
                    XDocument doc = XDocument.Load(info.SourcePath[2].Local);
                    var list = doc.Descendants().ToList();
                    var mac = list.Where(p => (p.Attribute("Tag") != null) && (p.Attribute("Tag").Value == "Address"));
                    if (mac.Count() != 0)
                    {
                        BMac = mac.ToList().FirstOrDefault().Value.ToUpper();
                    }
                }
            }
            catch { }

            //获取Wifi的MAC属性
            try
            {
                string wifiConfigPath = "";
                // /data/misc/dhcp/dhcpcd-wlan0.lease
                if (FileHelper.IsValid(info.SourcePath[3].Local))
                {
                    wifiConfigPath = info.SourcePath[3].Local;
                }
                // /data/misc/dhcp/dhcpcd-wlan0.lease_0
                else if (FileHelper.IsValid(info.SourcePath[4].Local))
                {
                    wifiConfigPath = info.SourcePath[4].Local;
                }

                if (wifiConfigPath.IsValid())
                {
                    using (FileStream myStream = new FileStream(wifiConfigPath, FileMode.Open, FileAccess.Read))
                    {
                        string wifiMac = "";
                        string wifiIP = "";
                        BinaryReader read = new BinaryReader(myStream);
                        int count = (int)myStream.Length;
                        byte[] buffer = new byte[count];
                        read.Read(buffer, 0, buffer.Length);
                        for (int i = 28; i < 34; i++)
                        {
                            if (i == 28)
                                wifiMac = string.Format("{0:X}", buffer[i]).PadLeft(2, '0');
                            else
                                wifiMac += ":" + string.Format("{0:X}", buffer[i]).PadLeft(2, '0');
                        }
                        for (int i = 16; i < 20; i++)
                        {
                            if (i == 16)
                                wifiIP = buffer[i].ToString();
                            else
                                wifiIP += "." + buffer[i].ToString();
                        }

                        TMac = wifiMac.ToUpper();
                    }
                }
            }
            catch { }

            //获取手机号,imsi,iccid信息
            try
            {
                // /data/data/com.android.providers.telephony/databases/telephony.db
                if (FileHelper.IsValid(info.SourcePath[5].Local))
                {
                    string simdata = "";

                    using (var context = new SqliteContext(info.SourcePath[5].Local))
                    {
                        var siminfo = context.FindByName("siminfo").Distinct();
                        foreach (var sim in siminfo)
                        {
                            simdata = "ICCID:" + sim.icc_id + ",";
                            if (!string.IsNullOrEmpty(sim.number))
                                simdata += LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_PhoneNumber) + sim.number + ",";
                            else
                                simdata += LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_PhoneNumber) + ":NA";

                            if (!string.IsNullOrEmpty(sim.display_name))
                                simdata += LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_CarrierOperator) + sim.display_name;
                            else
                                simdata += LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_CarrierOperator) + ":NA";

                            listsimdata.Add(simdata);
                        }
                    }
                }

                // /data/data/com.samsung.simcardmanagement/databases/simcardmanagement.db
                if (FileHelper.IsValid(info.SourcePath[6].Local))
                {
                    string simdata = "";

                    using (var context = new SqliteContext(info.SourcePath[6].Local))
                    {
                        var siminfo = (from u in context.FindByName("registerinfo") select new { u.card_iccid, u.card_number, u.card_name, u.card_id }).Distinct();
                        foreach (var sim in siminfo)
                        {
                            simdata = "ICCID:" + sim.card_iccid + ",";
                            if (!string.IsNullOrEmpty(sim.card_number))
                                simdata += LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_PhoneNumber) + sim.card_number + ",";
                            else
                                simdata += LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_PhoneNumber) + ":NA";

                            if (!string.IsNullOrEmpty(sim.card_name))
                                simdata += LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_CarrierOperator) + sim.card_name;
                            else
                                simdata += LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_CarrierOperator) + ":NA";

                            if (!string.IsNullOrEmpty(sim.card_id))
                                simdata += "IMSI" + sim.card_id;
                            else
                                simdata += "IMSI:NA";

                            listsimdata.Add(simdata);
                        }
                    }
                }
            }
            catch { }

            //获取设备名称
            try
            {
                // /data/system/users/0/settings_global.xml
                if (FileHelper.IsValid(info.SourcePath[7].Local))
                {
                    XDocument doc = XDocument.Load(info.SourcePath[7].Local);
                    var nameStr = doc.Elements("settings").First().Elements("setting").FirstOrDefault(e => e.Attribute("name") != null && e.Attribute("name").Value == "device_name").Attribute("value").Value;
                    if (nameStr.IsValid())
                    {
                        name = nameStr;
                    }
                }
            }
            catch { }

            //从APP植入获取设备信息

            if (FileHelper.IsValid(info.SourcePath[8].Local))
            {
                try
                {
                    JObject jo = JObject.Parse(FileHelper.FileToUTF8String(info.SourcePath[8].Local));
                    if (serialnumber.IsInvalid())
                    {
                        serialnumber = jo["serial"].ToSafeString();
                    }
                    if (manufacture.IsInvalid())
                    {
                        manufacture = jo["manufacturer"].ToSafeString();
                    }
                    if (model.IsInvalid())
                    {
                        model = jo["model"].ToSafeString();
                    }
                    if (OSVersion.IsInvalid())
                    {
                        OSVersion = jo["release"].ToSafeString();
                    }
                    if (IMEI.IsInvalid())
                    {
                        IMEI = jo["imei"].ToSafeString();
                    }
                    if (IMSI.IsInvalid())
                    {
                        IMSI = jo["imsi"].ToSafeString();
                    }
                    if (WiFiAddress.IsInvalid())
                    {
                        WiFiAddress = jo["wifiMac"].ToSafeString();
                    }
                    if (BMac.IsInvalid())
                    {
                        BMac = jo["btMac"].ToSafeString();
                    }
                }
                catch { }
            }

            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_Serialnumber), serialnumber));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_DeviceName), name));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_Manufacture), manufacture));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_Model), model));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_OSType), OSType));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_OSVersion), OSVersion));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_Root), root));
            dataSource.Items.Add(new KeyValueItem("IMEI", IMEI));
            dataSource.Items.Add(new KeyValueItem("IMSI", IMSI));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_WiFiAddress), WiFiAddress));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_BMac), BMac));
            dataSource.Items.Add(new KeyValueItem(LanguageHelper.GetString(Languagekeys.PluginDeviceProperty_TMac), TMac));

            int id = 0;
            foreach (var sim in listsimdata)
            {
                dataSource.Items.Add(new KeyValueItem("SIM" + (id++).ToString(), sim));
            }
        }
    }
}
