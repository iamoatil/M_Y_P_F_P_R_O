using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidQQLiteDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidQQLiteDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{D20DF00D-8E5E-432E-9F1A-563CD0C2EDCE}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_QQLite);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_SocialChat);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 1;
            pluginInfo.OrderIndex = 2;

            pluginInfo.AppName = "com.tencent.qqlite";
            pluginInfo.Icon = "\\icons\\Icon-qq.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidQQLite);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.qqlite/databases/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.qqlite/shared_prefs/#F");
            pluginInfo.SourcePath.AddItem("SDCard:/tencent/QQLite/#F");//多媒体文件夹

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

                //com.tencent.qqlite
                var qqPath = new DirectoryInfo(databasesPath).Parent.FullName;

                var parser = new AndroidQQLiteDataParseCoreV1_0(pi.SaveDbPath, LanguageHelper.GetString(Languagekeys.PluginName_QQLite), qqPath, mediaPath);

                var qqNode = parser.BuildTree();

                if (null != qqNode)
                {
                    ds.TreeNodes.Add(qqNode);
                }

            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓QQ轻聊版数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
