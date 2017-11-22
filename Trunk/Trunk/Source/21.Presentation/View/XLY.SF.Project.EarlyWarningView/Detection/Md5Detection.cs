/* ==============================================================================
* Description：Md5Detection  
* Author     ：litao
* Create Date：2017/11/22 16:40:14
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// Md5检测
    /// </summary>
    class Md5Detection : IDetection,IInitialize
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool _isInitialize;

        /// <summary>
        /// 检测输入的字符串是否符合规范
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool Detect(string input)
        {
            return true;
        }

        /// <summary>
        /// 用一个规范配置文件来初始化检测规则
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Initialize(List<string> sensitiveList)
        {
            _isInitialize = false;

            _isInitialize = true;
            return _isInitialize;
        }
    }
}
