/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/12/26 15:13:40 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace X64Service
{
    /// <summary>
    /// 微信电脑备份和解析DLL
    /// </summary>
    public static class WechatBackupCoreDll
    {
        private const string _dllPath = @"Lib\vcdllX64\WXbackup\WXBakDLL.dll";

        /// <summary>
        /// 初始化DLL
        /// </summary>
        /// <param name="handle">句柄</param>
        /// <param name="bakFileDir">存放备份文件的目录</param>
        /// <returns></returns>
        [DllImport(_dllPath, EntryPoint = "init_DLL")]
        public static extern int Init_DLL(ref IntPtr handle, string bakFileDir);

        /// <summary>
        /// 解析微信消息
        /// </summary>
        /// <param name="handle">句柄</param>
        /// <param name="textFilePath">TEXT文件路径</param>
        /// <param name="nfirstReadAddress">开始读取的位置</param>
        /// <param name="nlength">读取的长度</param>
        /// <param name="sWXCR">数据</param>
        /// <returns></returns>
        [DllImport(_dllPath, EntryPoint = "WXbak_analysisMsg")]
        public static extern int WXbak_analysisMsg(IntPtr handle, string textFilePath, Int64 nfirstReadAddress, Int64 nlength, ref IntPtr sWXCR);

        /// <summary>
        /// 释放数据缓冲
        /// </summary>
        /// <param name="sWXCR">数据</param>
        /// <returns></returns>
        [DllImport(_dllPath, EntryPoint = "freeSWXCRStruct")]
        public static extern void FreeSWXCRStruct(ref IntPtr sWXCR);

        /// <summary>
        /// 获取媒体信息
        /// </summary>
        /// <param name="handle">句柄</param>
        /// <param name="textFilePath">MEDIA文件路径</param>
        /// <param name="nfirstReadAddress">开始读取的位置</param>
        /// <param name="nlength">读取的长度</param>
        /// <param name="outFilePath">文件存放路径</param>
        /// <returns></returns>
        [DllImport(_dllPath, EntryPoint = "WXbak_ResInfo")]
        public static extern int WXbak_ResInfo(IntPtr handle, string mediaFilePath, Int64 nfirstReadAddress, Int64 nlength, string outFilePath);

        /// <summary>
        /// 注销动态库
        /// </summary>
        /// <param name="handle">句柄</param>
        /// <returns></returns>
        [DllImport(_dllPath, EntryPoint = "WXbak_freeDLL")]
        public static extern int FreeDLL(ref IntPtr handle);

    }

    /// <summary>
    /// 微信聊天记录
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SWeiXinChatRecord
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public IntPtr strSendder;

        /// <summary>
        /// 接收者
        /// </summary>
        public IntPtr strRcver;

        /// <summary>
        /// 消息类型
        /// </summary>
        public IntPtr strMsgType;

        /// <summary>
        /// 发送的消息内容  16进制码
        /// </summary>
        public IntPtr strMsg;

        /// <summary>
        /// 串条数
        /// </summary>
        public IntPtr strBunchNum;

        /// <summary>
        /// 串信息  多条 用分号隔开
        /// </summary>
        public IntPtr strBunchInfo;

        /// <summary>
        /// 缩略图  或者 音频数据 的长度
        /// </summary>
        public Int32 nHexSize;

        /// <summary>
        /// 此结构的大小
        /// </summary>
        public Int32 nStructArrSize;

        /// <summary>
        /// 图片的缩略图 音频的数据  16进制码
        /// </summary>
        public IntPtr strHexData;

        /// <summary>
        /// 此消息时间
        /// </summary>
        public IntPtr strDataTime;

    }
}
