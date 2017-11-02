using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace XLY.SF.Project.IsolatedTaskEngine.Configuration
{
    /// <summary>
    /// 启动参数元素。
    /// </summary>
    public class ParamsElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        /// 消息收发器的名称。
        /// </summary>
        [ConfigurationProperty("transceiverName", DefaultValue = "transceiver", IsRequired = false)]
        public String TransceiverName
        {
            get => (String)base["transceiverName"];
            set => base["transceiverName"] = value;
        }

        /// <summary>
        /// 最大并行任务数量。
        /// </summary>
        [ConfigurationProperty("maxParallelTask", DefaultValue = 1, IsRequired = false)]
        public Int32 MaxParallelTask
        {
            get => (Int32)base["maxParallelTask"];
            set
            {
                if (value < 1) throw new ArgumentNullException("value must be bigger than 0");
                base["maxParallelTask"] = value;
            }
        }

        /// <summary>
        /// 是否启用AppDomain隔离任务。
        /// </summary>
        [ConfigurationProperty("enableAppDomainIsolation", DefaultValue = false, IsRequired = false)]
        public Boolean EnableAppDomainIsolation
        {
            get => (Boolean)base["enableAppDomainIsolation"];
            set => base["enableAppDomainIsolation"] = value;
        }

        #endregion
    }
}
