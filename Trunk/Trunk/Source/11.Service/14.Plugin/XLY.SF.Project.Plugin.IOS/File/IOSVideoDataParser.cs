/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 16:57:01 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    public class IOSVideoDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public IOSVideoDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{EA304FB5-AA48-4711-8237-0A3E19A90E6C}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Video);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_FileExtraction);
            pluginInfo.DeviceOSType = EnumOSType.IOS;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;
            pluginInfo.GroupIndex = 6;
            pluginInfo.OrderIndex = 2;

            pluginInfo.AppName = "Video";
            pluginInfo.Icon = "\\icons\\video.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_IosVideo);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.Add(new SourceFileItem() { Config = "" });

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            FileTreeDataSource ds = new FileTreeDataSource();
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                FileDataParser.GetVideoFiles(ds, pi.SaveDbPath, pi.SourcePath[0].Local);
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取IOS设备视频文件信息出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
