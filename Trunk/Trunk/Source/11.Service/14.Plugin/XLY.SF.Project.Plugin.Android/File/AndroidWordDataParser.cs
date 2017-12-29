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
    public class AndroidWordDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidWordDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{D35E9D4A-279C-420A-83CE-B8491412BE60}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Word);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_FileExtraction);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB;
            pluginInfo.GroupIndex = 6;
            pluginInfo.OrderIndex = 3;

            pluginInfo.AppName = "Word";
            pluginInfo.Icon = "\\icons\\word.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidWord);
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
                    var savePath = Path.Combine(pi.SourcePath[0].Local.Replace('/', '\\').TrimEnd('\\').TrimEnd(@"\data\data\com.android.providers.media\databases"), "Word");

                    FileDataParser.GetAndroidPhoneTreeFiles(pi.Phone, pi.SourcePath[0].Local, savePath, ds, pi.SaveDbPath, EnumColumnType.Word);
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓设备文档文件信息出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
