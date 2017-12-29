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
    public class MTPVideoDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public MTPVideoDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{055C7C0A-B176-4B57-8867-70E0FE651390}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Video);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_FileExtraction);
            pluginInfo.DeviceOSType = EnumOSType.Android | EnumOSType.IOS;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.MTP;
            pluginInfo.GroupIndex = 6;
            pluginInfo.OrderIndex = 2;

            pluginInfo.AppName = "Video";
            pluginInfo.Icon = "\\icons\\video.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_MTPVideo);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("$Video,mp4;mpeg;mpg;dat;avi;m4v;mov;3gp;rm;flv;wmv;asf;navi;mkv;f4v;rmvb;webm;real video");

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
                    FileDataParser.GetVideoFiles(ds, pi.SaveDbPath, pi.SourcePath[0].Local);
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取MTP设备视频文件信息出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
