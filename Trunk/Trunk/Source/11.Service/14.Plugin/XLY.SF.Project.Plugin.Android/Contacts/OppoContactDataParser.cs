using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class OppoContactDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public OppoContactDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{EB7D1120-A2A4-4DA9-BDAB-21F2182FD58E}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Contacts);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 2;
            pluginInfo.Manufacture = "Oppo";

            pluginInfo.AppName = "Contact";
            pluginInfo.Icon = "\\icons\\contact.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidOppoContacts);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/Contact/#F");

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
                    var vcfFile = Path.Combine(contactsPath, "contact.vcf");
                    if (FileHelper.IsValid(vcfFile))
                    {
                        var paser = new OppoContactsDataParseCoreV1_0(vcfFile);
                        paser.BuildData(ds);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取Oppo手机备份联系人数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
