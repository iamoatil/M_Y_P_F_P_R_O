/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/18 13:39:47 
 * explain :  
 *
*****************************************************************************/

using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class IOSMirrorInstalledAppDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public IOSMirrorInstalledAppDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{2CB0146E-7CF5-49E6-97DC-9C4D7811EB2E";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_InstalledApp);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.IOS;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.Mirror;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 1;

            pluginInfo.AppName = "com.app";
            pluginInfo.Icon = "\\icons\\app.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_IosMirrorInstalledApp);
            pluginInfo.SourcePath = new SourceFileItems();

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SimpleDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SimpleDataSource();
                ds.Type = typeof(AppEntity);
                ds.Items = new DataItems<AppEntity>(pi.SaveDbPath);

                if (null != pi.Phone)
                {
                    var apps = pi.Phone.InstalledApps;
                    if (apps.IsValid())
                    {
                        foreach (var app in apps)
                        {
                            ds.Items.Add(app);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取IOS镜像安装应用数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
