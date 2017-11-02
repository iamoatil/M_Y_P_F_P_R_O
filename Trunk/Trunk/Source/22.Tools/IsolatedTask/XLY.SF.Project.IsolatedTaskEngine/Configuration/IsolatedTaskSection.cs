using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace XLY.SF.Project.IsolatedTaskEngine.Configuration
{
    /// <summary>
    /// 隔离任务的配置节点。
    /// </summary>
    public class IsolatedTaskSection : ConfigurationSection
    {
        /// <summary>
        /// 注入的业务所在类型。
        /// </summary>
        [ConfigurationProperty("entryType", IsRequired = true)]
        public String EntryType
        {
            get => (String)base["entryType"];
            set => base["entryType"] = value;
        }

        /// <summary>
        /// 引擎启动参数。
        /// </summary>
        [ConfigurationProperty("params", IsRequired = false)]
        public ParamsElement Params
        {
            get => base["params"] as ParamsElement;
            set => base["params"] = value;
        }
    }

}
