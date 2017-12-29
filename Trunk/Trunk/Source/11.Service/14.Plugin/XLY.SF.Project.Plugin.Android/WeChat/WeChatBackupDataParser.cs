using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class WeChatBackupDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public WeChatBackupDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{3432B6DC-11D8-45A6-9C9C-504A5564F270}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_WechatBackup);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_SocialChat);
            pluginInfo.DeviceOSType = EnumOSType.Android | EnumOSType.IOS;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 1;
            pluginInfo.OrderIndex = 2;

            pluginInfo.AppName = "com.wechatBackup";
            pluginInfo.Icon = "\\icons\\weixin.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_WechatBackup);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.wechatBackup/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            TreeDataSource ds = new TreeDataSource();

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;
                var databasesPath = pi.SourcePath[0].Local;

                if (!FileHelper.IsValidDictory(databasesPath))
                {
                    return ds;
                }

                var parser = new WeChatBackupDataParserCoreV1_0(pi.SaveDbPath, databasesPath);
                var qqNode = parser.BuildTree();

                if (null != qqNode)
                {
                    ds.TreeNodes.Add(qqNode);
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取微信电脑备份数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
