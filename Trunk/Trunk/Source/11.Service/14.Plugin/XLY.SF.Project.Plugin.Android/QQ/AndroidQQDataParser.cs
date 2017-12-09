using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidQQDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidQQDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{7BFC4BE3-EBAC-41C3-8D0D-2C7AD659C9AB}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_QQ);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_SocialChat);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 1;
            pluginInfo.OrderIndex = 1;

            pluginInfo.AppName = "com.tencent.mobileqq";
            pluginInfo.Icon = "\\icons\\Icon-qq.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidQQ);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mobileqq/databases/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mobileqq/shared_prefs/#F");
            pluginInfo.SourcePath.AddItem("SDCard:/tencent/MobileQQ/#F");//多媒体文件夹

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            TreeDataSource ds = new TreeDataSource();

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;
                var databasesPath = pi.SourcePath[0].Local;
                var mediaPath = pi.SourcePath[2].Local;

                if (!FileHelper.IsValidDictory(databasesPath))
                {
                    return ds;
                }

                if (!FileHelper.IsValidDictory(mediaPath))
                {
                    mediaPath = string.Empty;
                }

                //com.tencent.mobileqq文件夹路径
                var qqPath = new DirectoryInfo(databasesPath).Parent.FullName;

                var parser = new AndroidQQDataParseCoreV1_0(pi.SaveDbPath, LanguageHelper.GetString(Languagekeys.PluginName_QQ), qqPath, mediaPath);

                var qqNode = parser.BuildTree();

                if (null != qqNode)
                {
                    ds.TreeNodes.Add(qqNode);
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓QQ数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
