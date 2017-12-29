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
    /// 应用数据自定义预览视图插件配置信息
    /// </summary>
    [Serializable]
    public class DataPreviewPluginInfo : AbstractZipPluginInfo
    {
        /// <summary>
        /// 该数据视图对应的类型列表，格式为:"插件ID.类名;插件ID.类名;..."
        /// </summary>
        public List<DataPreviewSupportItem> ViewType { get; set; }

        /// <summary>
        /// 插件文件名，相对路径，表示解析器调用的主插件
        /// </summary>
        public override string ScriptFile => "index.html";
    }

    /// <summary>
    /// 该数据视图对应的类型列表
    /// </summary>
    [Serializable]
    public class DataPreviewSupportItem
    {
        /// <summary>
        /// 该数据视图对应的插件名称，用于描述，比如“微信”
        /// </summary>
        [XmlAttribute]
        public string PluginName { get; set; }

        /// <summary>
        /// 该数据视图对应的插件ID或文件后缀名：
        /// 1：表示为数据对象，此时为插件的ID，*表示通用匹配，比如“432A5C38-4580-49BA-84CB-64C2BD98974A”；
        /// 2：表示为文件后缀，*.*为通用匹配，多个以|分开，比如“*.doc|*.ppt”
        /// </summary>
        [XmlAttribute]
        public string PluginId { get; set; }

        /// <summary>
        /// 该数据视图对应的类名，比如MessageCore，在PluginId表示为数据对象时有效
        /// </summary>
        [XmlAttribute]
        public string TypeName { get; set; }

        /// <summary>
        /// 显示的快捷方式对应的ID，该方法为对当前已存在的插件的扩展。比如Vedio插件支持5种文件后缀，通过设置ShortcutId为Vedio插件的ID，可以扩展其支持的文件类型。
        /// 设置该属性后不需要实现具体的GetControl方法，会直接合并到Vedio插件中
        /// </summary>
        public string ShortcutId { get; set; }

        public override string ToString()
        {
            return $"{PluginName}({PluginId}).{TypeName}";
        }
    }
}
