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
    public class IOSInstalledAppDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public IOSInstalledAppDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{995B18D3-0B6B-4592-B9FE-F02F1C388BAB}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_InstalledApp);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.IOS;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 1;

            pluginInfo.AppName = "com.app";
            pluginInfo.Icon = "\\icons\\app.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_IosInstalledApp);
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
                    if (apps.IsInvalid())
                    {
                        apps = pi.Phone.FindInstalledApp();
                    }
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
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取IOS安装应用数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
