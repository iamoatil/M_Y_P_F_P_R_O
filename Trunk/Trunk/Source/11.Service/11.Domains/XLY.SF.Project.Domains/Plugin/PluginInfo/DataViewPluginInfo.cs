using System;
using System.Collections.Generic;
using System.Xml.Serialization;


/* ==============================================================================
* Assembly：	XLY.SF.Project.Domains.DataViewPluginInfo
* Description：	DataScriptPlugin  
* Author     ：	Fhjun
* Create Date：	2017/7/3 17:14:45
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 应用数据自定义视图插件配置信息
    /// </summary>
    [XmlRoot("plugin")]
    [Serializable]
    public class DataViewPluginInfo : AbstractZipPluginInfo
    {
        /// <summary>
        /// 该数据视图对应的类型列表，每一项都表示支持的类型
        /// </summary>
        [XmlArrayItem("item")]
        [XmlArray("view")]
        public List<DataViewSupportItem> ViewType { get; set; }

        /// <summary>
        /// 插件文件名，相对路径，表示解析器调用的主插件
        /// </summary>
        public override string ScriptFile => "index.html";
    }

    /// <summary>
    /// 该数据视图对应的类型列表
    /// </summary>
    [Serializable]
    public class DataViewSupportItem
    {
        /// <summary>
        /// 该数据视图对应的插件名称，用于描述，比如“微信”
        /// </summary>
        [XmlAttribute]
        public string PluginName { get; set; }

        /// <summary>
        /// 该数据视图对应的插件ID，用于精确匹配插件，为*表示通用匹配，比如“432A5C38-4580-49BA-84CB-64C2BD98974A”
        /// </summary>
        [XmlAttribute]
        public string PluginId { get; set; }

        /// <summary>
        /// 该数据视图对应的类名，比如MessageCore
        /// </summary>
        [XmlAttribute]
        public string TypeName { get; set; }

        /// <summary>
        /// 是否支持继承匹配，默认为false;
        /// true则表示当前类型或父类型符合条件也算匹配成功；此时对应的Type必须为C#的实际类型
        /// </summary>
        [XmlAttribute]
        public bool Inherit { get; set; } = false;

        public override string ToString()
        {
            return $"{PluginName}({PluginId}).{TypeName}";
        }
    }
}
