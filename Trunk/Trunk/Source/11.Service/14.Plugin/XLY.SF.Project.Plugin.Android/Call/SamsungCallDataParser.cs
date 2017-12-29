using System;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class SamsungCallDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public SamsungCallDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{69A2ED3E-E3A8-4CAF-ACE2-E40DE97FCE86}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Call);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 3;
            pluginInfo.Manufacture = "Samsung";

            pluginInfo.AppName = "com.android.providers.telephony";
            pluginInfo.Icon = "\\icons\\call.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidSansungCall);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.sec.android.provider.logsprovider/databases/#F");
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

                var logsFile = Path.Combine(pi.SourcePath[0].Local, "logs.db");
                var contacts2File = Path.Combine(pi.SourcePath[1].Local, "contacts2.db");

                if (FileHelper.IsValid(logsFile) || FileHelper.IsValid(contacts2File))
                {
                    var paser = new SamsungCallDataParseCoreV1_0(logsFile, contacts2File);

                    paser.BuildData(ds);
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取三星手机通话记录数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
