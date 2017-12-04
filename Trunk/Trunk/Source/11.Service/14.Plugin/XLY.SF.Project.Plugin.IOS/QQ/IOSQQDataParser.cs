/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/19 14:48:33 
 * explain :  
 *
*****************************************************************************/

using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.IOS
{
    public class IOSQQDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public IOSQQDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{618EED69-55ED-45EA-B87A-CF09A6180B46}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_QQ);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_SocialChat);
            pluginInfo.DeviceOSType = EnumOSType.IOS;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 1;
            pluginInfo.OrderIndex = 1;

            pluginInfo.AppName = "com.tencent.mqq";
            pluginInfo.Icon = "\\icons\\Icon-qq.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_IosQQ);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/com.tencent.mqq/Documents/");

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

                var mqqPath = new DirectoryInfo(databasesPath).Parent.FullName;

                var parser = new IOSQQDataParseCoreV1_0(pi.SaveDbPath, "IOS QQ", mqqPath);

                var qqNode = parser.BuildTree();

                if (null != qqNode)
                {
                    ds.TreeNodes.Add(qqNode);
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取IOSQQ数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
