using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class AndroidCallDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidCallDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{EE9D9548-1206-45EA-B2D3-98B1CCF055A2}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Call);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 3;

            pluginInfo.AppName = "com.android.providers.contacts";
            pluginInfo.Icon = "\\icons\\call.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidCall);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.contacts/databases/#F");
            pluginInfo.SourcePath.AddItem("APPCmd:calllog_info");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            CallDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new CallDataSource(pi.SaveDbPath);

                var items = new List<Call>();

                //1.从数据库获取
                var contactsPath = pi.SourcePath[0].Local;
                if (FileHelper.IsValidDictory(contactsPath))
                {
                    var contacts2dbFile = Path.Combine(contactsPath, "contacts2.db");
                    var call2dbFile = Path.Combine(contactsPath, "calls.db");

                    if (FileHelper.IsValid(contacts2dbFile) || FileHelper.IsValid(call2dbFile))
                    {
                        var paser = new AndroidCallDataParseCoreV1_0(contacts2dbFile, call2dbFile);

                        items.AddRange(paser.BuildData());
                    }
                }

                //2.从APP植入获取
                var calllog_info = pi.SourcePath[1].Local;
                if (FileHelper.IsValid(calllog_info))
                {
                    BuildData(calllog_info, ref items);
                }

                ds.Items.AddRange(items);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓通话记录数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        /// <summary>
        /// 解析APP植入获取的通话记录信息
        /// </summary>
        /// <param name="calllog_info"></param>
        /// <param name="items"></param>
        private void BuildData(string calllog_info, ref List<Call> items)
        {
            try
            {
                var name = string.Empty;
                var number = string.Empty;
                var date = string.Empty;
                var duration = string.Empty;
                var type = string.Empty;

                Call call;
                foreach (JObject jCall in JArray.Parse(FileHelper.FileToUTF8String(calllog_info)))
                {
                    name = jCall["name"].ToSafeString();
                    number = jCall["number"].ToSafeString();
                    date = jCall["date"].ToSafeString();
                    duration = jCall["duration"].ToSafeString();
                    type = jCall["type"].ToSafeString();

                    call = new Call();
                    call.DataState = EnumDataState.Normal;
                    call.Number = number;
                    call.Name = name;
                    call.StartDate = DynamicConvert.ToSafeDateTime(date);
                    call.DurationSecond = DynamicConvert.ToSafeInt(duration);
                    switch (type)
                    {
                        case "1"://呼入
                            call.Type = 0 == call.DurationSecond ? EnumCallType.MissedCallIn : EnumCallType.CallIn;
                            break;
                        case "2"://呼出
                            call.Type = 0 == call.DurationSecond ? EnumCallType.MissedCallOut : EnumCallType.CallOut;
                            break;
                        case "3"://呼入未接
                            call.Type = EnumCallType.MissedCallIn;
                            call.DurationSecond = 0;
                            break;
                    }

                    var ss = items.FirstOrDefault(i => i.Number == call.Number && i.StartDate == call.StartDate && i.DurationSecond == call.DurationSecond);
                    if (ss != null)
                    {
                        if (ss.Name.IsInvalid())
                        {
                            ss.Name = call.Name;
                        }
                    }
                    else
                    {
                        items.Add(call);
                    }
                }
            }
            catch (Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓通话记录APP植入数据出错！", ex);
            }
        }
    }
}
