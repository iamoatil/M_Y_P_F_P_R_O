using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 短信
    /// </summary>
    [Serializable]
    public class SMS : AbstractDataItem
    {
        /// <summary>
        /// 号码
        /// </summary>
        [Display]
        public string Number { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Display]
        public string ContactName { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        [Display]
        public string Content { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [Display]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 接收或者发送。
        /// </summary>
        [Display]
        public EnumSMSState SmsState { get; set; }

        /// <summary>
        /// 号码归属地
        /// </summary>
        [Display]
        public string LocationInfo { get; set; }

        /// <summary>
        /// 其他备注
        /// </summary>
        [Display]
        public string Remark { get; set; }

        /// <summary>
        /// 读取状态
        /// </summary>
        [Display]
        public EnumReadState ReadState { get; set; }

    }
}
