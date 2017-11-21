using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.PluginContainerAttribute
* Description：	描述为插件容器的类型，用于装载脚本插件
* Author     ：	fhjun
* Create Date：	2017/11/17 9:59:24
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 描述为插件容器的类型，用于装载脚本插件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginContainerAttribute : Attribute
    {
        public PluginContainerAttribute() { }
        public PluginContainerAttribute(PluginType type)
        {
            PluginType = type;
        }
        /// <summary>
        /// 当前容器对应插件的类型
        /// </summary>
        public PluginType PluginType { get; set; }
    }
}
