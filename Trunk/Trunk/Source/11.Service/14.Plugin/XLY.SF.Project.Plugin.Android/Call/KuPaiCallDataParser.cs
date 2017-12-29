using System;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class KuPaiCallDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public KuPaiCallDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{06E82B84-3810-45EB-B8B0-3805826A5E6C}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Call);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 3;
            pluginInfo.Manufacture = "KuPai";

            pluginInfo.AppName = "calllog";
            pluginInfo.Icon = "\\icons\\call.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidKupaiCall);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/recentcalls/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            CallDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new CallDataSource(pi.SaveDbPath);

                var path = pi.SourcePath[0].Local;
                if (FileHelper.IsValidDictory(path))
                {
                    var xmlFile = Path.Combine(path, "calls.xml");

                    if (FileHelper.IsValid(xmlFile))
                    {
                        var paser = new KupaiCallDataParseCoreV1_0(xmlFile);

                        paser.BuildData(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取酷派手机备份通话记录数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
