using System;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class XiaoMiSMSDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public XiaoMiSMSDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{65D2175E-914B-44AC-9F4D-838B1D4E8D5A}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Sms);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 4;
            pluginInfo.Manufacture = "XiaoMi";

            pluginInfo.AppName = "com.android.mms";
            pluginInfo.Icon = "\\icons\\sms.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidXiaomiSms);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.mms/miui_bak/_tmp_bak");
            pluginInfo.SourcePath.AddItem("/data/data/com.android.mms/sms.store");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SmsDataSource ds = null;

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SmsDataSource(pi.SaveDbPath);

                if (FileHelper.IsValid(pi.SourcePath[0].Local) || FileHelper.IsValid(pi.SourcePath[1].Local))
                {
                    var paser = new XiaomiSmsDataParseCoreV1_0(pi.SourcePath[0].Local, pi.SourcePath[1].Local);
                    paser.BuildData(ds);
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取小米手机备份短信数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
