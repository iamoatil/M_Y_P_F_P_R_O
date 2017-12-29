/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/19 16:29:10 
 * explain :  
 *
*****************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidBluetoothDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidBluetoothDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{7AE1B4F1-C3DE-4942-982F-DD80FC5BCA23}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Bluetooth);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 6;

            pluginInfo.AppName = "com.android.bluetooth";
            pluginInfo.Icon = "\\icons\\bluetooth.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidBluetooth);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/misc/bluetoothd/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SimpleDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SimpleDataSource();
                ds.Type = typeof(Bluetooth);
                ds.Items = new DataItems<Bluetooth>(pi.SaveDbPath);

                if (FileHelper.IsValidDictory(pi.SourcePath[0].Local))
                {
                    BuildData(ds, pi.SourcePath[0].Local);
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓设备蓝牙信息数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        private void BuildData(SimpleDataSource dataSource, string dataPath)
        {
            List<Bluetooth> items = new List<Bluetooth>();

            string[] allFolders = Directory.GetDirectories(dataPath);
            //1.find folder
            foreach (var f in allFolders)
            {
                //2.find files
                var file = Path.Combine(f, "names");
                if (!File.Exists(file))
                {
                    continue;
                }

                var content = FileHelper.FileToUTF8String(file).Trim();
                var local = f.TrimEnd('\\');
                local = local.Substring(local.LastIndexOf(@"\") + 1, local.Length - local.LastIndexOf(@"\") - 1);

                TryParseBluetoothNames(content, local.Replace("_", ":"), items);

                //解析连接时间
                file = Path.Combine(f, "lastseen");
                if (!File.Exists(file))
                {
                    file = Path.Combine(f, "lastused");
                    if (!File.Exists(file))
                    {
                        continue;
                    }
                }

                var times = FileHelper.FileToUTF8String(file).Trim();
                TryParseBluetoothTimes(times, items);
            }

            dataSource.Items.AddRange(items);
        }

        /// <summary>
        /// 解析bluetooth名称
        /// </summary>
        private void TryParseBluetoothNames(string content, string local, List<Bluetooth> items)
        {
            if (!content.IsValid())
            {
                return;
            }
            var list = content.Replace("\r", "").Replace("\n", "$").Split('$');
            if (!list.IsValid())
            {
                return;
            }
            foreach (var b in list)
            {
                var array = b.Split(' ');
                if (array.IsValid() && array.Length >= 2)
                {
                    Bluetooth item = new Bluetooth();
                    item.LocalAddress = local;
                    item.TargetAddress = array[0];
                    var name = string.Join(" ", array.Skip(1));
                    item.TargetName = name
                        .Encode(Encoding.Unicode);
                    items.Add(item);
                }
            }
        }

        /// <summary>
        /// 解析bluetooth连接时间
        /// </summary>
        private void TryParseBluetoothTimes(string content, List<Bluetooth> items)
        {
            if (!content.IsValid())
            {
                return;
            }

            foreach (var item in items)
            {
                var reg = new Regex(string.Format(@"(?<={0})(\d| |-|:)*", item.TargetAddress));
                var dt = reg.Match(content).Value;
                if (dt.IsValid())
                {
                    item.LastConnectDateTime = dt.Trim().ToDateTime().ToDateTimeString();
                }
            }
        }

    }
}
