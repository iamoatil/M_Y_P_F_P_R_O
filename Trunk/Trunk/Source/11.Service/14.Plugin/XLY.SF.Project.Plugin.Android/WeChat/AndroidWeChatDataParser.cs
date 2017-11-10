using System;
using System.ComponentModel.Composition;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class AndroidWeChatDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidWeChatDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Name = "微信";
            pluginInfo.Group = "社交聊天";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror;

            pluginInfo.AppName = "com.tencent.mm";
            pluginInfo.Icon = "\\icons\\weixin.png";
            pluginInfo.Description = "提取安卓设备微信信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.tencent.mm/MicroMsg/#F");

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

                //com.tencent.mm文件夹路径
                var mmPath = new DirectoryInfo(databasesPath).Parent.FullName;

                var parser = new AndroidWeChatDataParseCoreV1_0(pi.SaveDbPath, "安卓微信", mmPath, null);

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
