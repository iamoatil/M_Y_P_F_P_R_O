/* ==============================================================================
* Description：ConfigFileDir  
*           此文件中定义了4个关于 ConfigFileDir 的类，他们都继承于 AbstractConfigFileDir 类
*           以下左边的是右边的具体类型           
*               AppNameConfigFileDir        -------》 AbstractConfigFileDir
*               KeyWordConfigFileDir        -------》 AbstractConfigFileDir
*               Md5ConfigFileDir            -------》 AbstractConfigFileDir
*               NetAddressConfigFileDir     -------》 AbstractConfigFileDir
*           

* Author     ：litao
* Create Date：2017/11/25 13:35:51
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    class AppNameConfigFileDir : AbstractConfigFileDir
    {
        protected override string RootName { get { return "AppNameCollection"; } }
    }

    class KeyWordConfigFileDir : AbstractConfigFileDir
    {
        protected override string RootName { get { return "KeyWordCollection"; } }
    }

    class Md5ConfigFileDir : AbstractConfigFileDir
    {
        protected override string RootName { get { return "FileMd5Collection"; } }
    }

    class NetAddressConfigFileDir : AbstractConfigFileDir
    {
        protected override string RootName { get { return "NetAddressCollection"; } }
    }
}
