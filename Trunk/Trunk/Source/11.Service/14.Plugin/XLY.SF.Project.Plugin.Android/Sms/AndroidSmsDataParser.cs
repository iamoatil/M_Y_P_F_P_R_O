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
    public class AndroidSmsDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidSmsDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{DAB60C73-4A35-488F-A0FA-3620F6924B0F}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Sms);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 4;

            pluginInfo.AppName = "com.android.providers.telephony";
            pluginInfo.Icon = "\\icons\\sms.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidSms);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.telephony/databases/#F");
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.contacts/databases/#F");
            pluginInfo.SourcePath.AddItem("APPCmd:sms_info");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            SmsDataSource ds = null;

            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new SmsDataSource(pi.SaveDbPath);

                List<SMS> items = new List<SMS>();

                //1.从数据库获取数据
                var smsdbPath = pi.SourcePath[0].Local;
                if (FileHelper.IsValidDictory(smsdbPath))
                {
                    var smsdbFile = Path.Combine(smsdbPath, "mmssms.db");
                    if (FileHelper.IsValid(smsdbFile))
                    {
                        var contactsdbFile = Path.Combine(smsdbPath, "contacts2.db");
                        if (!FileHelper.IsValid(contactsdbFile))
                        {
                            contactsdbFile = null;
                        }

                        var paser = new AndroidSmsDataParseCoreV1_0(smsdbFile, contactsdbFile);
                        items.AddRange(paser.BuildData());
                    }
                }

                //2.从APP植入获取
                var sms_info = pi.SourcePath[2].Local;
                if (FileHelper.IsValid(sms_info))
                {
                    BuildData(sms_info, ref items);
                }

                ds.Items.AddRange(items);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓短信数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        /// <summary>
        /// 解析APP植入获取的短信
        /// </summary>
        /// <param name="sms_info"></param>
        /// <param name="items"></param>
        private void BuildData(string sms_info, ref List<SMS> items)
        {
            try
            {
                var toNumber = string.Empty;
                var toName = string.Empty;
                var date = string.Empty;
                var content = string.Empty;
                var type = string.Empty;
                var read = string.Empty;
                //var saveFolder = string.Empty;

                SMS sms;
                foreach (JObject jSms in JArray.Parse(FileHelper.FileToUTF8String(sms_info)))
                {
                    toNumber = jSms["toNumber"].ToSafeString();
                    toName = jSms["toName"].ToSafeString();
                    date = jSms["date"].ToSafeString();
                    content = jSms["content"].ToSafeString();
                    type = jSms["type"].ToSafeString();
                    //saveFolder = jSms["saveFolder"].ToSafeString();

                    sms = new SMS();
                    sms.DataState = EnumDataState.Normal;
                    sms.Number = toNumber;
                    sms.ContactName = toName;
                    sms.StartDate = DynamicConvert.ToSafeDateTime(date);
                    sms.Content = content;

                    switch (type)
                    {
                        case "1"://接收
                            sms.SmsState = EnumSMSState.ReceiveSMS;
                            read = jSms["read"].ToSafeString();
                            if (read == "1")
                            {
                                sms.ReadState = EnumReadState.Read;
                            }
                            else
                            {
                                sms.ReadState = EnumReadState.Unread;
                            }
                            break;
                        case "2"://发送
                            sms.SmsState = EnumSMSState.SendSMS;
                            break;
                    }

                    if (!items.Any(i => i.StartDate == sms.StartDate && i.Number == sms.Number && i.Content == sms.Content))
                    {
                        items.Add(sms);
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
