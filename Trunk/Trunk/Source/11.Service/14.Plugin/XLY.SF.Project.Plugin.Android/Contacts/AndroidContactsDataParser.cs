using Newtonsoft.Json.Linq;
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
    public class AndroidContactsDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public AndroidContactsDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{8DB88FCD-F725-4F9D-864F-78E13EF6BCE1}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Contacts);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "0.0";
            pluginInfo.Pump = EnumPump.USB | EnumPump.Mirror | EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 2;

            pluginInfo.AppName = "com.android.providers.contacts";
            pluginInfo.Icon = "\\icons\\contact.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidContact);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/com.android.providers.contacts/databases/#F");
            pluginInfo.SourcePath.AddItem("APPCmd:contact_info");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            ContactDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new ContactDataSource(pi.SaveDbPath);

                var items = new List<Contact>();

                //1.从数据库获取
                var contactsPath = pi.SourcePath[0].Local;
                if (FileHelper.IsValidDictory(contactsPath))
                {
                    var contacts2dbFile = Path.Combine(contactsPath, "contacts2.db");
                    if (FileHelper.IsValid(contacts2dbFile))
                    {
                        var paser = new AndroidContactsDataParseCoreV1_0(contacts2dbFile);
                        items.AddRange(paser.BuildData());
                    }
                }

                //2.从APP植入获取
                var contact_info = pi.SourcePath[1].Local;
                if (FileHelper.IsValid(contact_info))
                {
                    BuildData(contact_info, ref items);
                }

                ds.Items.AddRange(items);
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓联系人数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }

        /// <summary>
        /// 解析APP植入获取的联系人信息
        /// </summary>
        /// <param name="contact_info"></param>
        /// <param name="items"></param>
        private void BuildData(string contact_info, ref List<Contact> items)
        {
            try
            {
                var name = string.Empty;
                var number = string.Empty;

                foreach (JObject jContact in JArray.Parse(FileHelper.FileToUTF8String(contact_info)))
                {
                    name = jContact["name"].ToSafeString();
                    foreach (JObject jNumber in jContact["number"] as JArray)
                    {
                        number = jNumber["number"].ToSafeString();

                        if (!items.Any(i => i.Name == name && i.Number == number))
                        {
                            items.Add(new Contact() { DataState = EnumDataState.Normal, Name = name, Number = number });
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取安卓联系人APP植入数据出错！", ex);
            }
        }
    }
}
