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

        /// <summary>
        /// 根类型
        /// </summary>
        public string RootNodeName { get; private set; }

        /// <summary>
        /// 敏感数据
        /// </summary>
        public string Value { get; private set; }

        public string CategoryName { get; private set; }

        /// <summary>
        /// SensitiveId
        /// </summary>
        public int SensitiveId { get; private set; }

        public SensitiveData(string rootNodeName, string categoryName, string sensitiveId,string value)
        {
            this.RootNodeName = rootNodeName;
            this.CategoryName = categoryName;
            this.Value = value;
            if(!int.TryParse(sensitiveId,out int SensitiveId))
            {
                SensitiveId = 0;
            }
        }
    }
}
