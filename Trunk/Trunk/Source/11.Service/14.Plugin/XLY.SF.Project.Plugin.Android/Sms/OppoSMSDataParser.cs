using System;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class OppoSMSDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public OppoSMSDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{3B840306-4CDC-45C5-94BE-B7DF3EE9CDC4}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Sms);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 4;
            pluginInfo.Manufacture = "Oppo";

            pluginInfo.AppName = "Sms";
            pluginInfo.Icon = "\\icons\\sms.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidOppoSms);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/Sms/#F");

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
                if (FileHelper.IsValidDictory(smsdbPath))
                {
                    var smsdbFile = Path.Combine(smsdbPath, "sms.vmsg");
                    if (FileHelper.IsValid(smsdbFile))
                    {
                        var paser = new OppoSmsDataParseCoreV1_0(smsdbFile);

                        paser.BuildData(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取OPPO手机备份短信数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
