using System.Collections.Generic;

/* ==============================================================================
* Description：AbstractConfigFileDir  
* Author     ：litao
* Create Date：2017/11/22 17:44:01
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    abstract class AbstractConfigNode
    {
        /// <summary>
        /// 
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 节点的孩子
        /// </summary>
        public readonly List<AbstractConfigNode> Children = new List<AbstractConfigNode>();
    }
}
