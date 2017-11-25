/* ==============================================================================
* Description：Md5Detection  
* Author     ：litao
* Create Date：2017/11/22 16:40:14
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 检测
    /// </summary>
    abstract class AbstractDetection : IDetection, IInitialize, IName
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool _isInitialize;

        /// <summary>
        /// 从配置文件中读取的敏感数据列表
        /// </summary>
        List<SensitiveData> _sensitiveDataList;

        public string Name { get; set; }

        /// <summary>
        /// 检测输入的字符串是否符合规范,不符合就返回其违反的条目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public SensitiveData Detect(string input)
        {
            if (!_isInitialize)
            {
                return null;
            }

            foreach (var item in _sensitiveDataList)
            {
                if (item.Value == input)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 为了提高效率，可以把多个字符一起拿来检测
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public List<SensitiveData> Detect(List<string> inputList)
        {
            return null;
        }

        /// <summary>
        /// 用一个规范配置文件来初始化检测规则
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Initialize(List<SensitiveData> sensitiveList)
        {
            _isInitialize = false;
            if (sensitiveList == null
                || sensitiveList.Count < 1)
            {
                return _isInitialize;
            }
            
            _sensitiveDataList = sensitiveList;
            _isInitialize = true;
            return _isInitialize;
        }
    }
}
