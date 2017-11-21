/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 16:20:20 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidMMSDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidMMSDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{73C8EEBD-EE62-4560-BADE-1F7A208B05CB}";
            pluginInfo.Name = "彩信";
            pluginInfo.Group = "基本信息";
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 5;

            pluginInfo.AppName = "com.android.providers.telephony";
            pluginInfo.Icon = "\\icons\\sms.png";
            pluginInfo.Description = "提取安卓设备彩信信息";
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.telephony/databases/mmssms.db");
            pluginInfo.SourcePath.AddItem("/data/user/0/com.android.providers.telephony/app_parts/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.telephony/app_parts/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            MMSDataSource ds = null;

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new MMSDataSource(pi.SaveDbPath);

                var databasesFilePath = pi.SourcePath[0].Local;

                if (!FileHelper.IsValid(databasesFilePath))
                {
                    return ds;
                }

                BuildData(ds, databasesFilePath);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓彩信数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        private void BuildData(MMSDataSource ds, string databasesFilePath)
        {
            var nfile = SqliteRecoveryHelper.DataRecovery(databasesFilePath, "", "canonical_addresses,pdu,part");
            using (var sqliteContext = new SqliteContext(nfile))
            {
                MMS item = null;
                // 获取所有彩信信息;
                List<dynamic> itemsMMS = sqliteContext.FindByName("pdu").ToList();
                List<dynamic> itemsAddr = sqliteContext.FindByName("canonical_addresses").ToList();
                List<dynamic> itemsPart = sqliteContext.Find(new SQLiteString("select * from part where ct not in ('application/smil')")).ToList();
                foreach (var o in itemsMMS)
                {
                    try
                    {
                        item = new MMS();
                        item.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(o.XLY_DataType, EnumDataState.Normal);
                        item.SendState = DynamicConvert.ToSafeInt(o.msg_box) == 1
                                             ? EnumSendState.Receive
                                             : EnumSendState.Send;
                        item.Date = DynamicConvert.ToSafeDateTime(o.date);
                        // 获取彩信的扩展;
                        var resultAddr = itemsAddr.Find(addr => DynamicConvert.ToSafeInt(addr.xly_id) == DynamicConvert.ToSafeInt(o.thread_id));
                        if (resultAddr == null) continue;
                        item.SenderName = DataParseHelper.NumberToStu(DynamicConvert.ToSafeString(resultAddr.address));

                        // 内容扩展
                        var resultPart = itemsPart.FindAll(part => DynamicConvert.ToSafeInt(part.mid) == DynamicConvert.ToSafeInt(o.xly_id));

                        // 这段方法虽然看起来不爽，但是别删!有个别手机数据结构不同!这里主要兼容; wangxi 2014-7-14 17:14:05
                        if (string.IsNullOrEmpty(item.SenderName) && resultPart.Count != 0)
                        {
                            var num = itemsAddr.Find(addr => DynamicConvert.ToSafeInt(resultPart.First().xly_id) == DynamicConvert.ToSafeInt(addr.xly_id));
                            item.SenderName = DynamicConvert.ToSafeString(num.address);
                            item.SenderName = DataParseHelper.NumberToStu(item.SenderName);
                        }
                        if (resultPart.Count == 0)
                        {
                            item.Content = DynamicConvert.ToSafeString(o.ct_l);
                            item.Type = EnumColumnType.URL;
                        }
                        else
                        {
                            if (resultPart.Count == 1)
                            {
                                if (DynamicConvert.ToSafeString(resultPart.First().xly_data) != "")
                                {
                                    item.Content = DynamicConvert.ToSafeString(resultPart.First().xly_data).TrimStart('/').Replace("/", @"\");
                                }
                                else
                                {
                                    item.Content = DynamicConvert.ToSafeString(resultPart.First().text);
                                }
                            }
                            if (resultPart.Count > 1)
                            {
                                // 获取所有附加消息;
                                var allMsg = resultPart.FindAll(part => DynamicConvert.ToSafeString(part.ct).Equals("text/plain"));
                                var allPart = resultPart.FindAll(part => DynamicConvert.ToSafeString(part.text) == string.Empty);
                                string message = string.Empty;
                                foreach (var mess in allMsg)
                                {
                                    message += string.Format("{0}{1}", mess.text, Environment.NewLine);
                                }
                                string parts = string.Empty;
                                foreach (var part in allPart)
                                {
                                    if (!string.IsNullOrEmpty(DynamicConvert.ToSafeString(part.xly_data)))
                                    {
                                        string npath = DynamicConvert.ToSafeString(part.xly_data);
                                        parts += string.Format("{0}{1}", npath, Environment.NewLine);
                                    }
                                }
                                item.Content = parts + message;
                            }
                        }
                        item.Content = item.Content.Trim();

                        ds.Items.Add(item);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
