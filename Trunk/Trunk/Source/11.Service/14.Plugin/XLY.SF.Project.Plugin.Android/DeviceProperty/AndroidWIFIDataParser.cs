/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/19 16:29:10 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidWIFIDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidWIFIDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{FCED4544-3A7F-4CCB-B5D5-66FEBE025367}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_WIFI);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 7;

            pluginInfo.AppName = "com.android.WIFI";
            pluginInfo.Icon = "\\icons\\wifi.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidWIFI);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/misc/wifi/wpa_supplicant.conf");
            pluginInfo.SourcePath.AddItem("/data/misc/wifi/wpa_supplicant.conf");
            pluginInfo.SourcePath.AddItem("/data/wifi/bcm_supp.conf");
            pluginInfo.SourcePath.AddItem("/data/misc/wifi/p2p_supplicant.conf");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SimpleDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SimpleDataSource();
                ds.Type = typeof(WiFiInfo);
                ds.Items = new DataItems<WiFiInfo>(pi.SaveDbPath);

                string file = "";
                if (FileHelper.IsValid(pi.SourcePath[0].Local))
                {
                    file = pi.SourcePath[0].Local;
                }
                else if (FileHelper.IsValid(pi.SourcePath[1].Local))
                {
                    file = pi.SourcePath[1].Local;
                }
                else if (FileHelper.IsValid(pi.SourcePath[2].Local))
                {
                    file = pi.SourcePath[2].Local;
                }
                else if (FileHelper.IsValid(pi.SourcePath[3].Local))
                {
                    file = pi.SourcePath[3].Local;
                }

                string content = FileHelper.FileToUTF8String(file);
                if (content.IsValid())
                {
                    List<WiFiInfo> items = new List<WiFiInfo>();
                    var groups = content.Split(new string[] { "network" }, StringSplitOptions.RemoveEmptyEntries);
                    int len = groups.Length;
                    if (len <= 1)
                    {
                        return null;
                    }
                    var ssid = new Regex(@"(?<=\bssid="").*(?=\b"")");
                    var psk = new Regex(@"(?<=\bpsk="").*(?=\b"")");
                    var key = new Regex(@"(?<=\bkey_mgmt=)\b.*");
                    var pri = new Regex(@"(?<=\bpriority=)\d*");
                    for (int i = 1; i < len; i++)
                    {
                        var id = ssid.Match(groups[i]).Value;
                        if (id.IsValid())
                        {
                            WiFiInfo item = new WiFiInfo();
                            item.Name = id;
                            item.Pwd = psk.Match(groups[i]).Value;
                            item.Type = key.Match(groups[i]).Value;
                            item.Priority = pri.Match(groups[i]).Value.ToSafeInt();
                            items.Add(item);
                        }
                    }

                    ds.Items.AddRange(items);
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓设备WIFI信息数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
