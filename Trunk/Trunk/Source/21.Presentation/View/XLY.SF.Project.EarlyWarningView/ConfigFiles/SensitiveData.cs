/* ==============================================================================
* Description：SensitiveData  
* Author     ：litao
* Create Date：2017/11/22 17:40:47
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 敏感数据
    /// </summary>
    class SensitiveData
    {
        public SensitiveData(string type,string value)
        {
            Type = type;
            Value = value;
        }
        public string Type { get; private set; }

        public string Value { get; private set; }
    }
}
