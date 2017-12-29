using System;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class VivoSMSDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public VivoSMSDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{02F80B99-D67A-4AEC-A3B2-1D1FDF097060}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Sms);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 4;
            pluginInfo.Manufacture = "Vivo";

            pluginInfo.AppName = "sms";
            pluginInfo.Icon = "\\icons\\sms.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidVivoSms);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/sms/#F");

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
                    var smsdbFile = Path.Combine(smsdbPath, "sms.json");
                    if (FileHelper.IsValid(smsdbFile))
                    {
                        var paser = new VivoSmsDataParseCoreV1_0(smsdbFile, "");

                        paser.BuildData(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取Vivo手机备份短信数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
