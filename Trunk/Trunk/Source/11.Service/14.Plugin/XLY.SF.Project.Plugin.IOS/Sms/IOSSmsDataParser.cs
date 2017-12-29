/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/19 14:48:33 
 * explain :  
 *
*****************************************************************************/

using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.IOS
{
    public class IOSSmsDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public IOSSmsDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{245006E7-5C32-44FD-B373-47515B9104C8}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Sms);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.IOS;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 4;

            pluginInfo.AppName = "HomeDomain";
            pluginInfo.Icon = "\\icons\\sms.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_IosSms);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/HomeDomain/Library/SMS");
            pluginInfo.SourcePath.AddItem("/MediaDomain");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SmsDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SmsDataSource(pi.SaveDbPath);

                var dbPath = pi.SourcePath[0].Local;

                if (!FileHelper.IsValidDictory(dbPath))
                {
                    return ds;
                }

                var dbFile = Path.Combine(dbPath, "sms.db");
                if (!FileHelper.IsValid(dbFile))
                {
                    return ds;
                }

                var paser = new IOSSMSDataParseCoreV1_0(dbFile, pi.SourcePath[1].Local);
                paser.BuildData(ds);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取IOS短信数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
