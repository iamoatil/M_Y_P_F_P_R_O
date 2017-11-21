/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 16:25:01 
 * explain :  
 *
*****************************************************************************/

using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 彩信
    /// </summary>
    [Serializable]
    public class MMS : AbstractDataItem
    {
        /// <summary>
        /// 发送者姓名
        /// </summary> 
        [Display]
        public string SenderName { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        [Display]
        public DateTime? Date { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>
        [Display]
        public DateTime? ReadDate { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display]
        public string Content { get; set; }

        #region IConversion

        /// <summary>
        /// 用于会话模式的头像
        /// </summary>
        [Display]
        public string SenderImage { get; set; }

        /// <summary>
        /// 用于会话模式的数据类型
        /// </summary>
        [Display]
        public EnumColumnType Type { get; set; }

        /// <summary>
        /// 用于会话模式的信息发送状态
        /// </summary>
        [Display]
        public EnumSendState SendState { get; set; }

        #endregion

    }
}
