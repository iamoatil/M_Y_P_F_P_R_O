using System;
using System.ComponentModel.Composition;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class AndroidQQDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidQQDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Name = "QQ";
            pluginInfo.Group = "社交聊天";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;

            pluginInfo.AppName = "com.tencent.mobileqq";
            pluginInfo.Icon = "\\icons\\Icon-qq.png";
            pluginInfo.Description = "提取安卓设备QQ信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mobileqq/databases/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mobileqq/shared_prefs/#F");

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
                    return null;
                }

                //com.tencent.mobileqq文件夹路径
                var qqPath = new DirectoryInfo(databasesPath).Parent.FullName;

                var parser = new AndroidQQDataParseCoreV1_0(pi.SaveDbPath, "安卓QQ", qqPath, null);

                var qqNode = parser.BuildTree();

                if (null != qqNode)
                {
                    ds.TreeNodes.Add(qqNode);
                }
            }
            catch
            {//TODO:异常处理

            }

            return ds;
        }
    }
}
