/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 14:42:54 
 * explain :  
 *
*****************************************************************************/

using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓UC浏览器数据解析
    /// </summary>
    public class AndroidUCBrowseDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 安卓UC浏览器数据解析
        /// </summary>
        public AndroidUCBrowseDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{E0D8A355-C796-4584-8681-A719115F9655}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_UCBrowse);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_WebMark);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "11.2.0.880";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 3;
            pluginInfo.OrderIndex = 1;

            pluginInfo.AppName = "com.UCMobile";
            pluginInfo.Icon = "\\icons\\Uc_icon.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidUCBrowse);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.UCMobile/UCMobile/userdata/account/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.UCMobile/databases/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.UCMobile/user/history/input_history.ucmd");
            pluginInfo.SourcePath.AddItem("/data/data/com.UCMobile/UCMobile/app_external/novel_search");
            pluginInfo.SourcePath.AddItem("SDCard:/UCDownloads/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            TreeDataSource ds = new TreeDataSource();

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                var accountPath = pi.SourcePath[0].Local;
                if (!FileHelper.IsValidDictory(accountPath))
                {
                    return ds;
                }

                new AndroidUCBrowseDataParserCoreV11_2_0_880(pi.SaveDbPath,
                    pi.SourcePath[0].Local, pi.SourcePath[1].Local, pi.SourcePath[2].Local, pi.SourcePath[3].Local, pi.SourcePath[4].Local).BuildData(ds);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓UC浏览器数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

    }
}
