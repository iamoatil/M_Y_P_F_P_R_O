/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 16:57:01 
 * explain :  
 *
*****************************************************************************/

using System;
using System.IO;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidVideoDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidVideoDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{88C80CB2-D202-440F-86BD-455E248837DA}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Video);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_FileExtraction);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB;
            pluginInfo.GroupIndex = 6;
            pluginInfo.OrderIndex = 2;

            pluginInfo.AppName = "Video";
            pluginInfo.Icon = "\\icons\\video.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidVideo);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.media/databases/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            FileTreeDataSource ds = new FileTreeDataSource();
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                if (FileHelper.IsValidDictory(pi.SourcePath[0].Local))
                {
                    var savePath = Path.Combine(pi.SourcePath[0].Local.Replace('/', '\\').TrimEnd('\\').TrimEnd(@"\data\data\com.android.providers.media\databases"), "Video");

                    FileDataParser.GetAndroidPhoneTreeFiles(pi.Phone, pi.SourcePath[0].Local, savePath, ds, pi.SaveDbPath, EnumColumnType.Video);
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓设备视频文件信息出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
