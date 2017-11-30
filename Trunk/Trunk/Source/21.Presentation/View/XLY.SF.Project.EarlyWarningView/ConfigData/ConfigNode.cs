/* ==============================================================================
* Description：
*           

* Author     ：litao
* Create Date：2017/11/25 13:35:51
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.EarlyWarningView
{
    interface INodeName
    {
        string NodeName { get; set; }
    }

    abstract class AbstractConfigNode : INodeName
    {
        /// <summary>
        /// 节点的孩子
        /// </summary>
        public readonly Dictionary<string, INodeName> Children = new Dictionary<string, INodeName>();

        public string NodeName { get; set; }
    }

    class RootNodeManager: AbstractConfigNode
    {

    }

    class RootNode : AbstractConfigNode
    {
        public RootNode(string nodeName)
        {
            NodeName = nodeName;
        }
    }

    class CategoryNode : INodeName
    {
        public string NodeName { get; set; }

        public readonly List<DataNode> DataList = new List<DataNode>();
    }

    class DataNode : INodeName
    {
        public string NodeName { get; set; }        

        public SensitiveData SensitiveData { get; set; }
    }
}
