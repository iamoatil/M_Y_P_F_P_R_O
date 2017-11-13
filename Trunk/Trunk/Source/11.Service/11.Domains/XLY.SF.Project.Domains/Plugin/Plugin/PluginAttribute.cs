/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/13 19:40:38 
 * explain :  
 *
*****************************************************************************/

using System;

namespace XLY.SF.Project.Domains.Plugin
{
    /// <summary>
    /// 执行该类为C#插件  
    /// 供NetPluginLoader使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
    public class PluginAttribute : Attribute
    {

    }
}
