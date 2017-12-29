using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    #region EnumDataState (数据状态)
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum EnumDataState
    {

        /// <summary>
        /// 未知
        /// </summary>
        None = 0,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 2,

        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 1,

        /// <summary>
        /// 碎片
        /// </summary>
        Fragment = 4,
    }
    #endregion

    #region EnumEncodingType（字符编码方式）


    #endregion

    #region EnumAlignment（对齐方式）

    /// <summary>
    /// 对齐方式
    /// </summary>
    public enum EnumAlignment
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 左对齐
        /// </summary>
        Left = 1,

        /// <summary>
        /// 居中
        /// </summary>
        Center = 2,

        /// <summary>
        /// 右对齐
        /// </summary>
        Right = 3
    }

    #endregion

    #region EnumColumnType （列数据类型）

    /// <summary>
    /// 列数据类型
    /// </summary>
    public enum EnumColumnType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        String = 0,
        /// <summary>
        /// 整形
        /// </summary>
        Int,
        /// <summary>
        /// 浮点型
        /// </summary>
        Double,
        /// <summary>
        /// 日期时间
        /// </summary>
        DateTime,
        /// <summary>
        /// 图片
        /// </summary>
        Image,
        /// <summary>
        /// URL
        /// </summary>
        URL,
        /// <summary>
        /// 视频
        /// </summary>
        Video,
        /// <summary>
        /// 音频
        /// </summary>
        Audio,
        /// <summary>
        /// 文档
        /// </summary>
        Word,
        /// <summary>
        /// html
        /// </summary>
        HTML,
        /// <summary>
        /// 枚举
        /// </summary>
        Enum,
        /// <summary>
        /// 地理位置
        /// </summary>
        Location,
        /// <summary>
        /// 名片
        /// </summary>
        Card,
        /// <summary>
        /// 视频聊天
        /// </summary>
        VideoChat,
        /// <summary>
        /// 邮件
        /// </summary>
        Mail,
        /// <summary>
        /// 集合
        /// </summary>
        List,
        /// <summary>
        /// 系统消息
        /// </summary>
        System,
        /// <summary>
        /// 未知类型
        /// </summary>
        None,
        /// <summary>
        /// 文件
        /// </summary>
        File,
        /// <summary>
        /// 语音聊天
        /// </summary>
        AudioCall,
        /// <summary>
        /// 微信红包
        /// </summary>
        WeChatRedPack,
        /// <summary>
        /// 表情
        /// </summary>
        Emoji,
        /// <summary>
        /// 缩略图
        /// </summary>
        Thumbnail,
        /// <summary>
        /// 微信转账
        /// </summary>
        WeChatTransfer,
        /// <summary>
        /// 微信支付
        /// </summary>
        WeChatZhifu,
        /// <summary>
        /// 公众号
        /// </summary>
        GongZhongHao,
    }

    #endregion

    #region EnumOrder（排序类型）

    /// <summary>
    /// 排序类型
    /// </summary>
    public enum EnumOrder
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 升序
        /// </summary>
        Asc = 1,

        /// <summary>
        /// 降序
        /// </summary>
        Desc = 2,
    }

    #endregion

    #region EnumSendState（信息发送状态）

    /// <summary>
    /// 信息发送状态
    /// </summary>
    public enum EnumSendState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// 接收
        /// </summary>
        [Description("Receive")]
        Receive = 1,

        /// <summary>
        /// 发送
        /// </summary>
        [Description("Send")]
        Send = 2,
    }

    #endregion

    #region EnumDisplayVisible (列数据显示方式)
    /// <summary>
    /// 列数据显示方式，为数据库可见/界面显示可见。
    /// 解决枚举列的问题
    /// </summary>
    public enum EnumDisplayVisibility
    {
        /// <summary>
        /// 数据库可见/界面显示可见，默认值
        /// </summary>
        Visible = 0,
        /// <summary>
        /// 仅为数据库可见，不需要展示给用户，但需要存储到数据库中
        /// </summary>
        ShowInDatabase = 1,
        /// <summary>
        /// 仅为界面显示可见，不需要存储
        /// </summary>
        ShowInUI = 2,
    }
    #endregion
    #region EnumExportState (导出报表，删除数据和未删除数据)
    public enum EnumExportState
    {
        All=0,
        Delete = 1,
        NotDelete=2
    }
    #endregion
}
