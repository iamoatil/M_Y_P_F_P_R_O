/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/18 13:40:15 
 * explain :  
 *
*****************************************************************************/

using System.Xml;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidMirrorInstalledAppDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidMirrorInstalledAppDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{2F8C34F8-54C8-461E-A308-DC001DF4678E}";
            pluginInfo.Name = "安装应用";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.Mirror;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 1;

            pluginInfo.AppName = "com.app";
            pluginInfo.Icon = "\\icons\\app.png";
            pluginInfo.Description = "提取安卓设备安装的应用列表（镜像）";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/system/packages.xml");
            //pluginInfo.SourcePath.AddItem("$Apk,apk");

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
                else if (FileHelper.IsValid(pi.SourcePath[0].Local))
                {
                    //var apkRootPath = pi.SourcePath[1].Local;
                    //bool HasApkFile = FileHelper.IsValidDictory(apkRootPath);

                    XmlDocument doc = new XmlDocument();
                    doc.Load(pi.SourcePath[0].Local);
                    var root = doc.DocumentElement;
                    if (null != root)
                    {
                        // 装载app列表
                        var packageList = root.GetElementsByTagName("package");
                        string apkPath = string.Empty;
                        //var reader = new Reader();
                        foreach (XmlNode package in packageList)
                        {
                            var appEntity = new AppEntity();

                            appEntity.AppId = package.GetSafeAttributeValue("name");
                            appEntity.Name = appEntity.AppId;
                            appEntity.InstallPath = package.GetSafeAttributeValue("codePath");
                            appEntity.DataPath = package.GetSafeAttributeValue("nativeLibraryPath");
                            //if (HasApkFile)
                            //{
                            //    //apkPath = Path.Combine(apkRootPath, "apk", appEntity.InstallPath.Replace("/system", "").Replace(@"/", @"\"));
                            //    //appEntity.VersionDesc = reader.GetApkVersion(apkPath);
                            //}

                            ds.Items.Add(appEntity);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓安装应用（镜像）数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
