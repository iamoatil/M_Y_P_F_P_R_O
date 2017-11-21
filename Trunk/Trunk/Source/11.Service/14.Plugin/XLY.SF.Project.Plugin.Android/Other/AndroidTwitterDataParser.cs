/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/16 15:49:02 
 * explain :  
 *
*****************************************************************************/

using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓Twitter解析插件
    /// </summary>
    public class AndroidTwitterDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidTwitterDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{8C058225-6D5D-4327-A1A5-612E81BDF934}";
            pluginInfo.Name = "Twitter";
            pluginInfo.Group = "社交聊天";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 1;
            pluginInfo.OrderIndex = 4;

            pluginInfo.AppName = "com.twitter.android";
            pluginInfo.Icon = "\\icons\\twitter.png";
            pluginInfo.Description = "提取安卓设备Twitter信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.twitter.android/databases/#F");

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

                new AndroidTwitterDataParserCoreV1_0(pi.SaveDbPath, pi.SourcePath[0].Local).BuildData(ds);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓Twitter数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
