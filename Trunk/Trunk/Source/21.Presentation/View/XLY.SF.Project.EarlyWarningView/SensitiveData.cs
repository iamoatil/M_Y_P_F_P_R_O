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
        public SensitiveData(string value,string rootCategory)
        {
            Value = value;
            RootCategory = rootCategory;
        }

        /// <summary>
        /// 根类型
        /// </summary>
        public string RootCategory { get; private set; }

        /// <summary>
        /// 敏感数据
        /// </summary>
        public string Value { get; private set; }
    }
}
