using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 通话记录
    /// </summary>
    [Serializable]
    public class Call : AbstractDataItem
    {
        /// <summary>
        /// 号码
        /// </summary>
        [Display]
        public string Number { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [Display]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 通话类型
        /// </summary>
        [Display]
        public EnumCallType Type { get; set; }

        /// <summary>
        /// 持续秒数
        /// </summary>
        [Display]
        public int DurationSecond { get; set; }

        /// <summary>
        /// 号码归属地
        /// </summary>
        [Display]
        public string LocationInfo { get; set; }

    }
}