using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class KuPaiContactDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public KuPaiContactDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{0B147C4C-72D1-4D05-9BD7-08553D691A61}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Contacts);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 2;
            pluginInfo.Manufacture = "KuPai";

            pluginInfo.AppName = "contact";
            pluginInfo.Icon = "\\icons\\contact.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidKupaiContacts);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/contacts/#F");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            ContactDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new ContactDataSource(pi.SaveDbPath);

                var contactsPath = pi.SourcePath[0].Local;
                if (FileHelper.IsValidDictory(contactsPath))
                {
                    var vcfFile = Path.Combine(contactsPath, "contacts.vcf");
                    if (FileHelper.IsValid(vcfFile))
                    {
                        var paser = new KupaiContactsDataParseCoreV1_0(vcfFile);
                        paser.BuildData(ds);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取酷派手机备份联系人数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
