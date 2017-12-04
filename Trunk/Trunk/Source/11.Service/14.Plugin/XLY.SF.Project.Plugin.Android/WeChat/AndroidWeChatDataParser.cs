using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidWeChatDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidWeChatDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{986488FD-952F-49AC-A5F6-CEA9528709F3}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Wechat);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_SocialChat);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 1;
            pluginInfo.OrderIndex = 0;

            pluginInfo.AppName = "com.tencent.mm";
            pluginInfo.Icon = "\\icons\\weixin.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidWechat);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mm/MicroMsg/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mm/shared_prefs/#F");//注意，C#插件本身不会使用该文件夹内的数据，但是底层数据库解密需要，所以不能删除！

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

                //com.tencent.mm文件夹路径
                var mmPath = new DirectoryInfo(databasesPath).Parent.FullName;

                var parser = new AndroidWeChatDataParseCoreV1_0(pi.SaveDbPath, LanguageHelper.GetString(Languagekeys.PluginName_Wechat), mmPath, "");

                var qqNode = parser.BuildTree();

                if (null != qqNode)
                {
                    ds.TreeNodes.Add(qqNode);
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓微信数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
