/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/19 14:48:33 
 * explain :  
 *
*****************************************************************************/

using System;
using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.IOS
{
    public class IOSCallDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public IOSCallDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{2021D32A-258A-40A5-8121-DB64A09B1FB4}";
            pluginInfo.Name = "通话记录";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.IOS;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 3;

            pluginInfo.AppName = "HomeDomain";
            pluginInfo.Icon = "\\icons\\call.png";
            pluginInfo.Description = "提取IOS设备通话记录信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/HomeDomain/Library/CallHistoryDB");
            pluginInfo.SourcePath.AddItem("/HomeDomain/Library/AddressBook");
            pluginInfo.SourcePath.AddItem("/WirelessDomain/Library/CallHistory");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            CallDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new CallDataSource(pi.SaveDbPath);

                var dbPath = pi.SourcePath[0].Local;
                var dbFile = Path.Combine(dbPath, "CallHistory.storedata");

                if (!FileHelper.IsValidDictory(dbPath) || !FileHelper.IsValid(dbFile))
                {
                    if (FileHelper.IsValidDictory(pi.SourcePath[2].Local))
                    {
                        var db1File = Path.Combine(pi.SourcePath[2].Local, "call_history.db");
                        if (!FileHelper.IsValid(db1File))
                        {
                            var paser = new IOSCallDataParseCoreV1_0(db1File);
                            paser.BuildData(ds);
                        }
                    }
                }
                else
                {
                    var paser = new IOSCallDataParseCoreV2_0(dbFile, Path.Combine(pi.SourcePath[1].Local, "AddressBook.sqlitedb"));
                    paser.BuildData(ds);
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error("提取IOS通话记录数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
