using System;
using System.IO;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidCallDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidCallDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Name = "通话记录";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;

            pluginInfo.AppName = "com.android.providers.contacts";
            pluginInfo.Icon = "\\icons\\call.png";
            pluginInfo.Description = "提取安卓设备通话记录信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.contacts/databases/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            CallDataSource ds = null;

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new CallDataSource(pi.SaveDbPath);

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

                var paser = new AndroidCallDataParseCoreV1_0(contacts2dbFile, null);
                paser.BuildData(ds);
            }
            catch
            {//TODO:异常处理

            }

            return ds;
        }
    }
}
