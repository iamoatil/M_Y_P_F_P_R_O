/* ==============================================================================
* Description：EarlyWarningItem  
* Author     ：litao
* Create Date：2017/11/22 11:01:34
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.EarlyWarningView
{
    class EarlyWarningItem : IEnable
    {
        public bool IsEnable { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 功能需要的数据所在的路径或目录
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        List<IEnable> Items { get; set; }
    }
}
