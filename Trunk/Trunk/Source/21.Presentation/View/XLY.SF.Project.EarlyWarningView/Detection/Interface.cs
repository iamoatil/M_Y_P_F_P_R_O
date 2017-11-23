/* ==============================================================================
* Description：IDetection  
* Author     ：litao
* Create Date：2017/11/22 16:40:42
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 检测输入的字符串是否符合规范,不符合就返回其违反的条目
    /// </summary>
    interface IDetection
    {
        SensitiveData Detect(string input);
    }

    /// <summary>
    ///用一个规范配置文件来初始化检测规则
    /// </summary>
    interface IInitialize
    {
        bool Initialize(List<SensitiveData> sensitiveList);
    }
}
