using System;
using System.ComponentModel.Composition;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class AndroidContactsDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidContactsDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Name = "联系人";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;

            pluginInfo.AppName = "com.android.providers.contacts";
            pluginInfo.Icon = "\\icons\\contact.png";
            pluginInfo.Description = "提取安卓设备联系人信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.contacts/databases/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            ContactDataSource ds = null;

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new ContactDataSource(pi.SaveDbPath);

                var contactsPath = pi.SourcePath[0].Local;

                if (!FileHelper.IsValidDictory(contactsPath))
                {
                    return ds;
                }

                var contacts2dbFile = Path.Combine(contactsPath, "contacts2.db");
                if (!FileHelper.IsValid(contacts2dbFile))
                {
                    return ds;
                }

                var paser = new AndroidContactsDataParseCoreV1_0(contacts2dbFile);
                paser.BuildData(ds);
            }
            catch
            {//TODO:异常处理

            }

            return ds;
        }
    }
}
