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
    public class MTPAudioDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public MTPAudioDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{EFCA9358-8772-4729-A193-5D59D5A2B311}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Audio);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_FileExtraction);
            pluginInfo.DeviceOSType = EnumOSType.Android | EnumOSType.IOS;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.MTP;
            pluginInfo.GroupIndex = 6;
            pluginInfo.OrderIndex = 1;

            pluginInfo.AppName = "Audio";
            pluginInfo.Icon = "\\icons\\audio.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_MTPAudio);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("$Audio,m4a;mpeg-4;mp3;wma;wav;ape;acc;ogg;amr;3ga;slk");

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
                    FileDataParser.GetAudioFiles(ds, pi.SaveDbPath, pi.SourcePath[0].Local);
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取MTP设备音频文件信息出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
