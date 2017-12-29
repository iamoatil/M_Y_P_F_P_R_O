/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/12/26 15:46:19 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Language;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 电脑微信备份解析
    /// </summary>
    internal class WeChatBackupDataParserCoreV1_0
    {
        /// <summary>
        /// 电脑微信备份解析核心类
        /// </summary>
        /// <param name="savedatadbpath">数据保存数据库路径</param>
        /// <param name="datapath">com.wechatBackup文件夹路径</param>
        public WeChatBackupDataParserCoreV1_0(string savedatadbpath, string datapath)
        {
            DbFilePath = savedatadbpath;
            BaseSourcePath = datapath;
        }

        #region 构造属性

        /// <summary>
        /// 数据库路径
        /// </summary>
        private string DbFilePath { get; set; }

        /// <summary>
        ///微信电脑备份数据文件根目录，例如 I:\本地数据\com.wechatBackup
        /// </summary>
        private string BaseSourcePath { get; set; }

        /// <summary>
        /// 帐号ID
        /// </summary>
        private string AccountWechatId { get; set; }

        /// <summary>
        /// 朋友信息缓存 帐号--昵称
        /// </summary>
        private Dictionary<string, string> FriendsCatch { get; set; }

        #endregion

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <returns></returns>
        public TreeNode BuildTree()
        {
            TreeNode rootNode = null;
            try
            {
                rootNode = Parser();

                return rootNode;
            }
            finally
            {
                FriendsCatch?.Clear();
                rootNode?.BuildParent();
            }
        }

        private TreeNode Parser()
        {
            TreeNode rootNode = null;

            var dllHandle = IntPtr.Zero;
            try
            {
                var res = WechatBackupCoreDll.Init_DLL(ref dllHandle, BaseSourcePath);
                if (0 != res || dllHandle == IntPtr.Zero)
                {
                    LoggerManagerSingle.Instance.Error($"WechatBackupCoreDll Init Error！错误码：{res}");
                    return null;
                }

                using (var backupDb = new SqliteContext(Path.Combine(BaseSourcePath, "Backup.db.desc")))
                {
                    AccountWechatId = GetAccountWechatId(backupDb);

                    //1.获取好友列表
                    var lsFriends = GetAllFriends(backupDb);
                    if (lsFriends.IsValid())
                    {
                        rootNode = new TreeNode
                        {
                            DataState = EnumDataState.Normal,
                            Text = LanguageHelper.GetString(Languagekeys.PluginName_WechatBackup),
                            Type = typeof(WeChatFriendShow),
                            Items = new DataItems<WeChatFriendShow>(DbFilePath)
                        };
                        rootNode.Items.AddRange(lsFriends);

                        //2.获取聊天记录
                        foreach (var firend in lsFriends)
                        {
                            var lsMsgs = GetAllMsgs(BaseSourcePath, dllHandle, backupDb, firend);

                            if (lsMsgs.IsValid())
                            {
                                var fNode = new TreeNode()
                                {
                                    DataState = EnumDataState.Normal,
                                    Text = firend.ShowName,
                                    Type = typeof(MessageCore),
                                    Items = new DataItems<MessageCore>(DbFilePath)
                                };
                                rootNode.TreeNodes.Add(fNode);

                                fNode.Items.AddRange(lsMsgs);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (dllHandle != IntPtr.Zero)
                {
                    WechatBackupCoreDll.FreeDLL(ref dllHandle);
                }
            }

            return rootNode;
        }

        /// <summary>
        /// 获取帐号
        /// </summary>
        /// <returns></returns>
        private string GetAccountWechatId(SqliteContext backupDb)
        {
            var lsData = backupDb.Find(new SQLiteString("Select Buf From Config"));
            if (lsData.IsInvalid())
            {
                return string.Empty;
            }

            try
            {
                var buff = lsData.First().Buf as byte[];
                if (buff[0] == 0x0A && buff.Length > buff[1])
                {
                    return Encoding.UTF8.GetString(buff, 2, buff[1]);
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "GetAccountWechatId Error!");
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取好友列表
        /// </summary>
        private List<WeChatFriendShow> GetAllFriends(SqliteContext backupDb)
        {
            List<WeChatFriendShow> lsFriends = new List<WeChatFriendShow>();

            FriendsCatch = new Dictionary<string, string>();

            //1.从Session表获取
            var lsSessionData = backupDb.Find(new SQLiteString("SELECT talker,NickName FROM Session"));
            WeChatFriendShow frind;
            foreach (var session in lsSessionData)
            {
                frind = new WeChatFriendShow()
                {
                    WeChatId = DynamicConvert.ToSafeString(session.talker),
                    Nick = DynamicConvert.ToSafeString(session.NickName)
                };

                lsFriends.Add(frind);
                FriendsCatch.Add(frind.WeChatId, string.Format("{0}({1})", frind.Nick, frind.WeChatId));
            }


            //2.从MsgSegments表获取
            var lsOthers = backupDb.Find(new SQLiteString("SELECT DISTINCT UsrName FROM MsgSegments WHERE UsrName not in (SELECT talker FROM Session)"));
            foreach (var other in lsOthers)
            {
                lsFriends.Add(new WeChatFriendShow()
                {
                    WeChatId = DynamicConvert.ToSafeString(other.UsrName)
                });
            }

            return lsFriends;
        }

        /// <summary>
        /// 获取聊天记录
        /// </summary>
        private List<MessageCore> GetAllMsgs(string sourcePath, IntPtr dllHandle, SqliteContext backupDb, WeChatFriendShow friend)
        {
            List<MessageCore> list = new List<MessageCore>();

            string sqlStr = string.Format("SELECT OffSet,Length,FilePath FROM MsgSegments WHERE UsrName = '{0}'", friend.WeChatId);

            long offset = 0, length = 0;
            string filepath;
            var msgData = IntPtr.Zero;
            int res;

            backupDb.UsingSafeConnection(new SQLiteString(sqlStr), r =>
            {
                while (r.Read())
                {
                    try
                    {
                        var msg = r.ToDynamic();

                        offset = DynamicConvert.ToSafeLong(msg.OffSet);
                        length = DynamicConvert.ToSafeLong(msg.Length);
                        filepath = DynamicConvert.ToSafeString(msg.FilePath);

                        var fullpath = Path.Combine(sourcePath, filepath);

                        res = WechatBackupCoreDll.WXbak_analysisMsg(dllHandle, fullpath, offset, length, ref msgData);

                        if (0 == res && msgData != IntPtr.Zero)
                        {
                            var tempData = msgData;
                            int cur = 0, count = 0;
                            do
                            {
                                var wechatRecord = (SWeiXinChatRecord)Marshal.PtrToStructure(tempData, typeof(SWeiXinChatRecord));

                                list.Add(GetSingleMsg(sourcePath, dllHandle, backupDb, wechatRecord));

                                count = wechatRecord.nStructArrSize;
                                cur++;

                                tempData += Marshal.SizeOf(wechatRecord);
                            }
                            while (cur < count);
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerManagerSingle.Instance.Error(ex, string.Format("WXbak_analysisMsg dllHandle:{0} offset:{1}  length:{2}", dllHandle, offset, length));
                    }
                    finally
                    {
                        if (IntPtr.Zero != msgData)
                        {
                            WechatBackupCoreDll.FreeSWXCRStruct(ref msgData);
                        }
                    }
                }
            });

            return list;
        }

        /// <summary>
        /// 获取单条聊天记录
        /// </summary>
        /// <returns></returns>
        private MessageCore GetSingleMsg(string sourcePath, IntPtr dllHandle, SqliteContext backupDb, SWeiXinChatRecord record)
        {
            MessageCore msg = new MessageCore();

            try
            {
                var strSendder = Marshal.PtrToStringAnsi(record.strSendder);
                var strRcver = Marshal.PtrToStringAnsi(record.strRcver);
                var strMsgType = Marshal.PtrToStringAnsi(record.strMsgType);
                var strMsg = Encoding.UTF8.GetString(strToToHexByte(Marshal.PtrToStringAnsi(record.strMsg)));
                var strDataTime = Marshal.PtrToStringAnsi(record.strDataTime);

                if (strSendder.EndsWith("@chatroom"))
                {//群聊消息   strMsg格式为　　sender:XXXX
                    var index = strMsg.IndexOf(':');
                    if (-1 != index)
                    {
                        strSendder = strMsg.Substring(0, index);
                        strMsg = strMsg.Substring(index + 1);
                    }
                }

                string strSendderName, strRcverName;
                FriendsCatch.TryGetValue(strSendder, out strSendderName);
                FriendsCatch.TryGetValue(strRcver, out strRcverName);

                DateTime dt;

                msg.SenderName = strSendderName.IsValid() ? strSendderName : strSendder;
                msg.Receiver = strRcverName.IsValid() ? strRcverName : strRcver;
                msg.DataState = EnumDataState.Normal;
                if (DateTime.TryParse(strDataTime, out dt))
                {
                    msg.Date = dt;
                }

                msg.SendState = strSendder == AccountWechatId ? EnumSendState.Send : EnumSendState.Receive;

                switch (strMsgType)
                {
                    case "1"://文字
                        msg.Type = EnumColumnType.String;
                        msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_String);
                        msg.Content = strMsg;
                        break;
                    case "3"://图片
                        msg.Type = EnumColumnType.Image;
                        msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Image);
                        msg.Content = GetMediaFile(sourcePath, dllHandle, backupDb, record, EnumColumnType.Image);
                        break;
                    case "16"://系统消息
                        msg.Type = EnumColumnType.System;
                        msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_SystemMsg);
                        msg.SenderName = LanguageHelper.GetString(Languagekeys.PluginWechat_SystemMsg);
                        msg.Content = strMsg;
                        break;
                    case "34"://语音
                        msg.Type = EnumColumnType.Audio;
                        msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Audio);
                        msg.Content = GetMediaFile(sourcePath, dllHandle, backupDb, record, EnumColumnType.Audio);
                        break;
                    case "43"://视频
                        msg.Type = EnumColumnType.Video;
                        msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Video);
                        msg.Content = GetMediaFile(sourcePath, dllHandle, backupDb, record, EnumColumnType.Video);
                        break;
                    case "47"://emoji
                        msg.Type = EnumColumnType.Emoji;
                        msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Emoji);

                        strMsg = strMsg.Replace("\"s60v5len", "\" s60v5len");

                        var xeEmoji = XElement.Parse(strMsg).Element("emoji");
                        if (null != xeEmoji.Attribute("cdnurl"))
                        {
                            msg.Content = xeEmoji.Attribute("cdnurl").Value;
                        }
                        else if (null != xeEmoji.Attribute("productid"))
                        {
                            msg.Content = xeEmoji.Attribute("productid").Value;
                        }
                        break;
                    case "48"://位置信息
                        msg.Type = EnumColumnType.Location;
                        msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_Location);

                        var xeData = XElement.Parse(strMsg).Element("location");
                        msg.Content = string.Format(LanguageHelper.GetString(Languagekeys.PluginWechat_LocationInfo),
                                                        xeData.Attribute("poiname").Value,
                                                        xeData.Attribute("label").Value,
                                                        xeData.Attribute("x").Value,
                                                        xeData.Attribute("y").Value);
                        break;
                    case "49"://XML格式消息
                        msg.Content = MessageToHTML(strMsg, ref msg);
                        break;
                    default:
                        msg.Type = EnumColumnType.String;
                        msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_String);
                        msg.Content = strMsg;
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "GetSingleMsg Error!");
            }

            return msg;
        }

        private string MessageToHTML(string msgstr, ref MessageCore msg)
        {
            string strMessage = msgstr;

            string strType = "";
            string strTitle = "";
            string strDesc = "";
            string strUrl = "";
            try
            {
                // 解析 XML 数据
                if (strMessage.Contains("<msg>"))
                    strMessage = strMessage.Substring(strMessage.IndexOf("<msg>"));
                MemoryStream msXmlLink = new MemoryStream(Encoding.UTF8.GetBytes(strMessage));
                XmlReader xmlLink = XmlReader.Create(msXmlLink);
                while (xmlLink.Read())
                {
                    if (xmlLink.NodeType == XmlNodeType.Element)
                    {
                        if (xmlLink.Name == "type")
                            strType = xmlLink.ReadString();
                        else if (xmlLink.Name == "title")
                            strTitle = xmlLink.ReadString();
                        else if (xmlLink.Name == "des")
                            strDesc = xmlLink.ReadString();
                        else if (xmlLink.Name == "url")
                            strUrl = xmlLink.ReadString();
                    }
                }
                xmlLink.Close();
                msXmlLink.Close();
            }
            catch
            {
                return strMessage;
            }

            if (strType == "2000")
            {//转账记录
                msg.Type = EnumColumnType.WeChatTransfer;
                msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_WeChatTransfer);
                if (strMessage.Contains("<paysubtype>1</paysubtype>"))
                {//转账
                    strMessage = string.Format("{0}-{1}", strTitle, strDesc);
                }
                else if (strMessage.Contains("<paysubtype>3</paysubtype>"))
                {//接收转账
                    strMessage = string.Format("{0}-{1}", strTitle, LanguageHelper.GetString(Languagekeys.PluginWechat_TransferOK));
                }
            }
            else if (strType == "2001")
            {//红包
                msg.Type = EnumColumnType.WeChatRedPack;
                msg.MessageType = LanguageHelper.GetString(Languagekeys.PluginWechat_WeChatRedPack);

                strMessage = strDesc;
            }
            else
            {
                if (strUrl.Trim().Length == 0)
                    strMessage = string.Format(LanguageHelper.GetString(Languagekeys.PluginWechat_HtmlInfoA), strTitle, strDesc);
                else
                    strMessage = string.Format(LanguageHelper.GetString(Languagekeys.PluginWechat_HtmlInfoB), strTitle, strDesc, strUrl);
            }

            return strMessage;
        }

        private readonly char[] strBunchInfoSplitChar = new char[] { ';' };
        private readonly Regex regex = new Regex(@"^\S+_backup");
        private readonly Regex regexBuncinfo = new Regex(@"^[0-9a-fA-F]+$");

        private string GetMediaFile(string sourcePath, IntPtr dllHandle, SqliteContext backupDb, SWeiXinChatRecord record, EnumColumnType msgType)
        {
            try
            {
                string strData = string.Empty;
                string strHexData = string.Empty;

                var strBunchInfo = Marshal.PtrToStringAnsi(record.strBunchInfo);

                if (strBunchInfo.IsValid())
                {
                    strData = strBunchInfo;
                }
                else
                {
                    strHexData = Marshal.PtrToStringAnsi(record.strHexData);
                    if (strHexData.IsValid())
                    {
                        if (regexBuncinfo.IsMatch(strHexData))
                        {
                            strData = Encoding.UTF8.GetString(strToToHexByte(strHexData));
                        }
                        else
                        {
                            strData = strHexData;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }

                string mediaStr = string.Empty;

                var mc = regex.Match(strData);
                if (!mc.Success)
                {
                    var fileaPath = GetMediaFilePathName(Guid.NewGuid().ToString(), msgType);

                    if (strHexData.IsValid())
                    {
                        File.WriteAllBytes(fileaPath, strToToHexByte(strHexData));
                    }
                    else
                    {
                        File.WriteAllBytes(fileaPath, Encoding.UTF8.GetBytes(strData));
                    }

                    if (msgType == EnumColumnType.Audio)
                    {
                        fileaPath = AudioDecodeHelper.Decode(fileaPath);
                    }

                    return fileaPath;
                }
                else
                {
                    mediaStr = strData;
                }

                //注意mediaStr可能包含多个 用；隔开
                //只获取最大的那个文件

                string sqlStr = string.Format(@"SELECT
	                                            f.OFFSET,
	                                            f.TotalLen,
                                                f.FileName
                                            FROM
                                            	MsgFileSegment f,
                                            	MsgMedia m
                                            WHERE
                                            	f.InnerOffSet = 0
                                            AND f.MapKey = m.MediaId
                                            AND m.MediaIdStr IN ({0})
                                            ORDER BY
                                            	f.TotalLen DESC", string.Join(",", mediaStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => string.Format("'{0}'", s))));

                var dd = backupDb.Find(new SQLiteString(sqlStr));
                if (dd.IsInvalid())
                {
                    return string.Empty;
                }

                var filePath = GetMediaFilePathName(mediaStr, msgType);

                long offset = 0, length = 0;
                string fileName = string.Empty;
                foreach (var data in dd)
                {
                    offset = DynamicConvert.ToSafeLong(data.OffSet);
                    length = DynamicConvert.ToSafeLong(data.TotalLen);
                    fileName = DynamicConvert.ToSafeString(data.FileName);

                    var fullPath = Path.Combine(sourcePath, fileName);

                    var res = WechatBackupCoreDll.WXbak_ResInfo(dllHandle, fullPath, offset, length, filePath);
                    if (res == 0 && FileHelper.IsValid(filePath))
                    {
                        break;
                    }
                }

                if (msgType == EnumColumnType.Audio)
                {
                    filePath = AudioDecodeHelper.Decode(filePath);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "GetMediaFile Error");
                return string.Empty;
            }
        }

        private string GetMediaFilePathName(string strBunchInfo, EnumColumnType msgType)
        {
            string basePath = Path.Combine(BaseSourcePath, "media", msgType.ToString());
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string suffix = "";
            switch (msgType)
            {
                case EnumColumnType.Image:
                    suffix = ".jpg";
                    break;
                case EnumColumnType.Audio:
                    suffix = ".amr";
                    break;
                case EnumColumnType.Video:
                    suffix = ".mp4";
                    break;
            }

            return Path.Combine(basePath, string.Concat(strBunchInfo, suffix));
        }

        private static byte[] strToToHexByte(string hexString)
        {
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    }
}
