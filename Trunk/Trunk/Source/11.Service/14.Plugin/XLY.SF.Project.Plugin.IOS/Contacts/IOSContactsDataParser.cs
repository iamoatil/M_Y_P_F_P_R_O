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

namespace XLY.SF.Project.Plugin.IOS
{
    public class IOSContactsDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public IOSContactsDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{FA1728E7-48EF-40DE-836A-8FB235656D91}";
            pluginInfo.Name = "联系人";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.IOS;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 2;

            pluginInfo.AppName = "HomeDomain";
            pluginInfo.Icon = "\\icons\\contact.png";
            pluginInfo.Description = "提取IOS设备联系人信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/HomeDomain/Library/AddressBook");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            ContactDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new ContactDataSource(pi.SaveDbPath);

                var dbPath = pi.SourcePath[0].Local;

                if (!FileHelper.IsValidDictory(dbPath))
                {
                    return ds;
                }

                var dbFile = Path.Combine(dbPath, "AddressBook.sqlitedb");
                if (!FileHelper.IsValid(dbFile))
                {
                    return ds;
                }

                var paser = new IOSContactsDataParseCoreV1_0(dbFile);
                paser.BuildData(ds);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取IOS联系人数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
