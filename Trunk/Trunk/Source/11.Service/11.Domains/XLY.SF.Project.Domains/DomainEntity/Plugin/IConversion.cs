/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/12/20 10:07:26 
 * explain :  
 *
*****************************************************************************/

using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 会话模式接口
    /// </summary>
    public interface IConversion
    {
        /// <summary>
        /// 发送者姓名
        /// </summary> 
        string SenderName { get; set; }

        /// <summary>
        /// 发送者图片
        /// </summary>
        string SenderImage { get; set; }

        /// <summary>
        /// 时间，可空
        /// </summary>
        DateTime? Date { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        EnumColumnType Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// 发送状态
        /// </summary>
        EnumSendState SendState { get; set; }
    }
}
