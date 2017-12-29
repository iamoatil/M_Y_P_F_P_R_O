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
    public class MTPWordDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public MTPWordDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{20FD02CE-631A-437F-9A7B-DA49ADA51A88}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Word);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_FileExtraction);
            pluginInfo.DeviceOSType = EnumOSType.Android | EnumOSType.IOS;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.MTP;
            pluginInfo.GroupIndex = 6;
            pluginInfo.OrderIndex = 3;

            pluginInfo.AppName = "Word";
            pluginInfo.Icon = "\\icons\\word.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_MTPWord);
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

                if (FileHelper.IsValidDictory(pi.SourcePath[0].Local))
                {
                    FileDataParser.GetWordFiles(ds, pi.SaveDbPath, pi.SourcePath[0].Local);
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取MTP设备文档文件信息出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
