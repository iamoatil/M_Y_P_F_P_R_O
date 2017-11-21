using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 开关机时间
    /// </summary>
    [Serializable]
    public class SwitchTimeInfo : AbstractDataItem
    {
        /// <summary>
        /// 开关机类型
        /// </summary>
        [Display]
        public EnumSwitchTimeType Type { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [Display]
        public DateTime? SwitchTimeInfoDate { get; set; }

    }
}
