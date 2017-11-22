/* ==============================================================================
* Description：IDetection  
* Author     ：litao
* Create Date：2017/11/22 16:40:42
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 检测输入的字符串是否符合规范
    /// </summary>
    interface IDetection
    {
        bool Detect(string input);
    }

    /// <summary>
    ///用一个规范配置文件来初始化检测规则
    /// </summary>
    interface IInitialize
    {
        bool Initialize(List<string> sensitiveList);
    }
}
