/* ==============================================================================
* Description：SettingItem  
* Author     ：litao
* Create Date：2017/11/22 11:01:34
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    class SettingItem : ISetting
    {
        public bool IsEnable { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
