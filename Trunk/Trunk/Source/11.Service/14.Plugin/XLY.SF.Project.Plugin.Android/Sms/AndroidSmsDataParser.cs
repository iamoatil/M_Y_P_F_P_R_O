using System;
using System.ComponentModel.Composition;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidSmsDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidSmsDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{DAB60C73-4A35-488F-A0FA-3620F6924B0F}";
            pluginInfo.Name = "短信";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 3;

            pluginInfo.AppName = "com.android.providers.telephony";
            pluginInfo.Icon = "\\icons\\sms.png";
            pluginInfo.Description = "提取安卓设备短信信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.telephony/databases/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.contacts/databases/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SmsDataSource ds = null;

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SmsDataSource(pi.SaveDbPath);

                var smsdbPath = pi.SourcePath[0].Local;

                if (!FileHelper.IsValidDictory(smsdbPath))
                {
                    return ds;
                }

                var smsdbFile = Path.Combine(smsdbPath, "mmssms.db");
                if (!FileHelper.IsValid(smsdbFile))
                {
                    return ds;
                }

                var contactsdbFile = Path.Combine(smsdbPath, "contacts2.db");
                if (!FileHelper.IsValid(contactsdbFile))
                {
                    contactsdbFile = null;
                }

                var paser = new AndroidSmsDataParseCoreV1_0(smsdbFile, contactsdbFile);
                paser.BuildData(ds);
            }
            catch
            {//TODO:异常处理

            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

    }
}
