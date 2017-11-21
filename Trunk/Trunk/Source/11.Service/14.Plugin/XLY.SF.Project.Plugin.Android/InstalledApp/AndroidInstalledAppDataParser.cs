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

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidInstalledAppDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidInstalledAppDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{D9C21B1D-945F-4BB9-B580-7E8599347AC2}";
            pluginInfo.Name = "安装应用";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 1;

            pluginInfo.AppName = "com.app";
            pluginInfo.Icon = "\\icons\\app.png";
            pluginInfo.Description = "提取安卓设备安装的应用列表";
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
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓安装应用数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
