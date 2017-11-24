/* ==============================================================================
* Description：ExtactionData  
* Author     ：litao
* Create Date：2017/11/24 15:08:09
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    abstract class AbstractExtactionData
    {
        public string Content { get; set; }
    }

    class Md5Data: AbstractExtactionData
    {
    }
    class KeywordData : AbstractExtactionData
    {
    }

    class NetAddressData : AbstractExtactionData
    {
    }
    class AppNameData : AbstractExtactionData
    {
    }
}
