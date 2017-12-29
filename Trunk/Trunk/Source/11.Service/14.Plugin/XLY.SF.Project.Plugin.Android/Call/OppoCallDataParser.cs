using System;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class OppoCallDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public OppoCallDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{B1331265-C712-4E70-85F4-10A168024FA4}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Call);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 3;
            pluginInfo.Manufacture = "Oppo";

            pluginInfo.AppName = "CallRecord";
            pluginInfo.Icon = "\\icons\\call.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidOppoCall);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/CallRecord/#F");

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
                    var xmlFile = Path.Combine(path, "callrecord_backup.xml");

                    if (FileHelper.IsValid(xmlFile))
                    {
                        var paser = new OppoCallDataParseCoreV1_0(xmlFile);

                        paser.BuildData(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取OPPO手机备份通话记录数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
