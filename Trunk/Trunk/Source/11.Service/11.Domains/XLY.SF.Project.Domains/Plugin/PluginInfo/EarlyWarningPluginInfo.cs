/* ==============================================================================
* Description：EarlyWarningPluginInfo  
* Author     ：litao
* Create Date：2017/12/2 10:21:20
* ==============================================================================*/

using System;
using System.Collections.Generic;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 预警的插件信息
    /// </summary>
    public class EarlyWarningPluginInfo : AbstractZipPluginInfo
    {
        /// <summary>
        /// 此插件会处理的DataSource的类型集合
        /// </summary>
        public List<Type> DataSourceTypes { get; set; }
    }
}
