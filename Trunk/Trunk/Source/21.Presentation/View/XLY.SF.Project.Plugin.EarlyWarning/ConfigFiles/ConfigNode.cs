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
        public List<DataNode> ValidateDataNodes
        {
            get { return _validateDataNodes; }
        }
        private readonly List<DataNode> _validateDataNodes = new List<DataNode>();
    }

    class RootNode : AbstractConfigNode
    {
        public RootNode(string nodeName,string path)
        {
            NodeName = nodeName;
            Path = path;
            IsEnable = false;
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// RootNode所在文件路径
        /// </summary>
        public string Path { get; private set; }
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
