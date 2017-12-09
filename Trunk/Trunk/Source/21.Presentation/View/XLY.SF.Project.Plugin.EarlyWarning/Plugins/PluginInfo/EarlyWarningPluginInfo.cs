/* ==============================================================================
* Description：EarlyWarningPluginInfo  
* Author     ：litao
* Create Date：2017/12/2 10:21:20
* ==============================================================================*/

using System.Collections.Generic;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 预警的插件信息
    /// </summary>
    public class EarlyWarningPluginInfo : AbstractZipPluginInfo
    {

        public virtual bool Match(IDataSource dataSource)
        {
            return false;
        }
    }

    public class UrlEarlyWarningPluginInfo : EarlyWarningPluginInfo
    {
        private string _groupName;
        public UrlEarlyWarningPluginInfo()
        {
            _groupName = Domains.LanguageHelper.GetString(Languagekeys.PluginGroupName_WebMark);
        }

        public override bool Match(IDataSource dataSource)
        {
            TreeDataSource ds = dataSource as TreeDataSource;
            if (ds != null
                && ds.PluginInfo.Group == _groupName)
            {
                return true;
            }
            return false;
        }
    }

    public class Md5EarlyWarningPluginInfo : EarlyWarningPluginInfo
    {

    }

    public class AppEarlyWarningPluginInfo : EarlyWarningPluginInfo
    {
        private string _name;
        public AppEarlyWarningPluginInfo()
        {
            _name = LanguageHelper.GetString(Languagekeys.PluginName_InstalledApp);
        }

        public override bool Match(IDataSource dataSource)
        {
            SimpleDataSource ds = dataSource as SimpleDataSource;
            if (ds != null
                && ds.PluginInfo.Name == _name)
            {
                return true;
            }
            return false;
        }
    }

    public class PhoneEarlyWarningPluginInfo : EarlyWarningPluginInfo
    {
        private List<string> _names;
        public PhoneEarlyWarningPluginInfo()
        {
            _names = new List<string>()
            {
                LanguageHelper.GetString(Languagekeys.PluginName_Call),
                LanguageHelper.GetString( Languagekeys.PluginName_Sms),
                LanguageHelper.GetString(Languagekeys.PluginName_Mms),
                LanguageHelper.GetString(Languagekeys.PluginName_Contacts),
            };
        }

        public override bool Match(IDataSource dataSource)
        {
            AbstractDataSource ds = dataSource as AbstractDataSource;
            if (ds != null
                && _names.Contains(ds.PluginInfo.Name))
            {
                return true;
            }
            return false;
        }
    }

    public class KeyWordEarlyWarningPluginInfo : EarlyWarningPluginInfo
    {

        public override bool Match(IDataSource dataSource)
        {
            return false;
        }
    }
}
