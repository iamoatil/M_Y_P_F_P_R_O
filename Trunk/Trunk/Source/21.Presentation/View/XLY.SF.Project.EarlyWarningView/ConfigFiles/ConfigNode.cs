/* ==============================================================================
* Description：
*           

* Author     ：litao
* Create Date：2017/11/25 13:35:51
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    class NodeDefinition
    {
        public static string RootName = "Root";

    }

    class RootNode : AbstractConfigNode
    {
        public RootNode()
        {
            NodeName= NodeDefinition.RootName;
        }

        public RootNode(RootNode node)
        {
            NodeName = node.NodeName;
        }
    }

    class CategoryNode : AbstractConfigNode
    {
    }

    class DataNode : AbstractConfigNode
    {
        public SensitiveData Data { get; set; }
    }

    //class AppNameConfigNode : AbstractConfigNode
    //{
    //    protected override string RootName { get { return NodeDefinition.AppNameCollection; } }
    //}

    //class KeyWordConfigNode : AbstractConfigNode
    //{
    //    protected override string RootName { get { return NodeDefinition.KeyWordCollection; } }
    //}

    //class Md5ConfigNode : AbstractConfigNode
    //{
    //    protected override string RootName { get { return NodeDefinition.FileMd5Collection; } }
    //}

    //class NetAddressConfigNode : AbstractConfigNode
    //{
    //    protected override string RootName { get { return NodeDefinition.NetAddressCollection; } }
    //}
}
