using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 邮件实体。
    /// </summary>
    [Serializable]
    public class EmailInfo : AbstractDataItem
    {
        /// <summary>
        /// 发送者。
        /// </summary>
        [Display]
        public string Sender { get; set; }

        /// <summary>
        /// 接收者。
        /// </summary>
        [Display]
        public string Receiver { get; set; }

        /// <summary>
        /// 主题。
        /// </summary>
        [Display]
        public string Subject { get; set; }

        /// <summary>
        /// 邮件内容。
        /// </summary>
        [Display]
        public string TextContent { get; set; }

        /// <summary>
        /// 发送时间。
        /// </summary>
        [Display]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 接收时间。
        /// </summary>
        [Display]
        public DateTime? RecvDataTime { get; set; }

    }

    [Serializable]
    public class EmailAccount : AbstractDataItem
    {
        public int Id { get; set; }

        [Display]
        public string Nick { get; set; }

        [Display]
        public string EmailAddress { get; set; }
    }
}
