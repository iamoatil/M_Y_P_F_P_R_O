/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 16:57:01 
 * explain :  
 *
*****************************************************************************/

using System;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidMirrorWordDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidMirrorWordDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{64A202A7-3AE4-44A7-863F-5207BD4F0308}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Word);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_FileExtraction);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.Mirror;
            pluginInfo.GroupIndex = 6;
            pluginInfo.OrderIndex = 3;

            pluginInfo.AppName = "Word";
            pluginInfo.Icon = "\\icons\\word.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidMirrorWord);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("$Word,txt;rtf;doc;wps;wpt;doc;dot;docx;dotx;docm;dotm;et;ett;xls;xlt;xlsx;xlsm;xltx;xltm;dps;dpt;ppt;pot;pptm;potx;potm;pptx;pps;ppsx;ppsm;pdf;epub;mobi;ch;zip;rar");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            FileTreeDataSource ds = new FileTreeDataSource();
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                FileDataParser.GetWordFiles(ds, pi.SaveDbPath, pi.SourcePath[0].Local);
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓镜像文档文件信息出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
