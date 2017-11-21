using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidQQEimDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidQQEimDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{A53B67FC-AD6F-47D5-98C6-0953E01C541C}";
            pluginInfo.Name = "企业QQ";
            pluginInfo.Group = "社交聊天";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 1;
            pluginInfo.OrderIndex = 3;

            pluginInfo.AppName = "com.tencent.eim";
            pluginInfo.Icon = "\\icons\\eimQQ.png";
            pluginInfo.Description = "提取安卓设备企业QQ信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.eim/databases/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.eim/shared_prefs/#F");

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

                //com.tencent.eim
                var qqPath = new DirectoryInfo(databasesPath).Parent.FullName;

                var parser = new AndroidQQEimDataParseCoreV1_0(pi.SaveDbPath, "企业QQ", qqPath, "");

                var qqNode = parser.BuildTree();

                if (null != qqNode)
                {
                    ds.TreeNodes.Add(qqNode);
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓企业QQ数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
