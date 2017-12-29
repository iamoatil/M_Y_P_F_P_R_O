using System.IO;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;

namespace XLY.SF.Project.Plugin.Android
{
    public class XiaomiContactDataParser : AbstractDataParsePlugin
    {
        public override IPluginInfo PluginInfo { get; set; }

        public XiaomiContactDataParser()
        {
            DataParsePluginInfo pluginInfo = new DataParsePluginInfo();
            pluginInfo.Guid = "{082DA9BE-D4D2-4177-ACCF-97082D5E8B53}";
            pluginInfo.Name = LanguageHelper.GetString(Languagekeys.PluginName_Contacts);
            pluginInfo.Group = LanguageHelper.GetString(Languagekeys.PluginGroupName_BasicInfo);
            pluginInfo.DeviceOSType = EnumOSType.Android;
            pluginInfo.VersionStr = "1.0";
            pluginInfo.Pump = EnumPump.LocalData;
            pluginInfo.GroupIndex = 0;
            pluginInfo.OrderIndex = 2;
            pluginInfo.Manufacture = "XiaoMi";

            pluginInfo.AppName = "contacts";
            pluginInfo.Icon = "\\icons\\contact.png";
            pluginInfo.Description = LanguageHelper.GetString(Languagekeys.PluginDescription_AndroidXiaomiContacts);
            pluginInfo.SourcePath = new SourceFileItems();
            pluginInfo.SourcePath.AddItem("/data/data/contacts/addressbook.store");
            pluginInfo.SourcePath.AddItem("/data/data/contacts/miui_bak/_tmp_bak");

            PluginInfo = pluginInfo;
        }

        public override object Execute(object arg, IAsyncTaskProgress progress)
        {
            ContactDataSource ds = null;
            try
            {
                var pi = PluginInfo as DataParsePluginInfo;

                ds = new ContactDataSource(pi.SaveDbPath);

                if (FileHelper.IsValid(pi.SourcePath[0].Local) || FileHelper.IsValid(pi.SourcePath[1].Local))
                {
                    var paser = new XiaomiContactsDataParseCoreV1_0(pi.SourcePath[0].Local, pi.SourcePath[1].Local);
                    paser.BuildData(ds);
                }
            }
            catch (System.Exception ex)
            {
                Framework.Log4NetService.LoggerManagerSingle.Instance.Error("提取小米手机备份联系人数据出错！", ex);
            }
            finally
            {
                ds?.BuildParent();
            }

            return ds;
        }
    }
}
