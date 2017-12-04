/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 9:55:00 
 * explain :  
 *
*****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Plugin.Language;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS短信解析
    /// </summary>
    internal class IOSSMSDataParseCoreV1_0
    {
        /// <summary>
        /// HomeDomain\Library\SMS\sms.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// MediaDomain路径 保存了附件
        /// </summary>
        private string MediaDomainPath { get; set; }

        /// <summary>
        /// IOS短信数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">HomeDomain\Library\SMS\sms.db文件路径</param>
        /// <param name="attendDirPath">附件路径  MediaDomain\</param>
        public IOSSMSDataParseCoreV1_0(string mainDbPath, string attendDirPath)
        {
            MainDbPath = mainDbPath;
            MediaDomainPath = attendDirPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="dataSource"></param>
        public void BuildData(SmsDataSource dataSource)
        {
            if (!FileHelper.IsValid(MainDbPath))
            {
                return;
            }

            SqliteContext mainContext = null;

            string attendDirPath = MediaDomainPath;

            try
            {
                var rMainDbFile = SqliteRecoveryHelper.DataRecovery(MainDbPath, @"chalib\IOS_Sms\sms.db.charactor",
                    "message,handle,attachment,chat,chat_message_join,chat_handle_join,message_attachment_join", true);
                mainContext = new SqliteContext(rMainDbFile);

                var handleDynamicList = mainContext.FindByName("handle");
                var chathandlejoinDynamicList = mainContext.Find(new SQLiteString("SELECT chat_id,handle_id FROM chat_handle_join WHERE chat_id NOTNULL")).ToList();

                var chatmessagejoinDynamicList = mainContext.Find(new SQLiteString("SELECT chat_id,message_id FROM chat_message_join WHERE chat_id NOTNULL"));
                var messageAttachmentJoinDynamicList = mainContext.Find(new SQLiteString("SELECT message_id,attachment_id FROM message_attachment_join WHERE message_id NOTNULL")).ToList();
                var attachmentDynamicList = mainContext.FindByName("attachment");

                var dateStr = "";

                mainContext.UsingSafeConnection(new SQLiteString("select * from message order by date desc"), r =>
                {
                    dynamic smsObj;
                    while (r.Read())
                    {
                        smsObj = r.ToDynamic();

                        var sms = new SMS();

                        //短信内容
                        sms.Content = DynamicConvert.ToSafeString(smsObj.text);
                        sms.Content = FragmentHelper.RemoveNullityDataNew(sms.Content);
                        // 验证内容是否为空
                        if (FragmentHelper.IsEmptyString(sms.Content))
                        {
                            continue;
                        }

                        sms.DataState = DynamicConvert.ToEnumByValue(smsObj.XLY_DataType, EnumDataState.Normal);

                        //发送时间和读取时间。

                        dateStr = DynamicConvert.ToSafeString(smsObj.date);
                        if (dateStr.Length > 9)
                        {
                            dateStr = dateStr.Substring(0, 9);
                        }

                        sms.StartDate = DynamicConvert.ToSafeDateTime(dateStr, 2001);
                        //sms.ReadTime = DynamicConvert.ToSafeDateTime(smsObj.date_read, 2001);

                        int smsId = DynamicConvert.ToSafeInt(smsObj.ROWID);
                        sms.Remark = string.Empty;
                        int hanldeId = DynamicConvert.ToSafeInt(smsObj.handle_id);
                        if (hanldeId == 0)
                        {
                            //群发消息
                            var someChatMsgObj = chatmessagejoinDynamicList.FirstOrDefault(cmj => DynamicConvert.ToSafeInt(cmj.message_id) == smsId && DynamicConvert.ToSafeInt(cmj.chat_id) != 0);
                            if (someChatMsgObj != null)
                            {
                                var handleIdList = chathandlejoinDynamicList.FindAll(chj => DynamicConvert.ToSafeInt(chj.chat_id) == someChatMsgObj.chat_id);

                                var numbersBuilder = new StringBuilder();
                                foreach (var oneHandle in handleIdList)
                                {
                                    numbersBuilder.Append(GetPhoteNumber(DynamicConvert.ToSafeInt(oneHandle.handle_id), handleDynamicList));
                                    numbersBuilder.Append(";");
                                }
                                sms.Remark = LanguageHelper.GetString(Languagekeys.PluginSMS_QunSend) + "; ";
                                sms.Number = numbersBuilder.ToString().TrimEnd(";");
                            }
                        }
                        else
                        {
                            sms.Number = GetPhoteNumber(hanldeId, handleDynamicList);
                        }
                        if (sms.Number.IsValid() && sms.DataState == EnumDataState.Fragment)
                        {
                            if (!FragmentHelper.IsValidFragment(sms.Number))
                                continue;
                        }
                        //发送或者接收
                        sms.SmsState = DynamicConvert.ToSafeInt(smsObj.is_from_me) == 1 ? EnumSMSState.SendSMS : EnumSMSState.ReceiveSMS;
                        int isDelivered = DynamicConvert.ToSafeInt(smsObj.is_delivered);
                        int isSent = DynamicConvert.ToSafeInt(smsObj.is_sent);
                        if (sms.SmsState == EnumSMSState.ReceiveSMS)
                        {
                            sms.Remark += DynamicConvert.ToSafeInt(smsObj.is_read) == 1 ?
                            string.Format("{0};", LanguageHelper.GetString(Languagekeys.PluginSMS_IsRead)) : string.Format("{0};", LanguageHelper.GetString(Languagekeys.PluginSMS_NotRead));
                        }
                        else
                        {
                            if (isDelivered == 0 && isSent == 0)
                            {
                                sms.Remark += LanguageHelper.GetString(Languagekeys.PluginSMS_SendFail) + "; ";
                            }
                        }

                        #region 附件解析

                        int cache_has_attachments = DynamicConvert.ToSafeInt(smsObj.cache_has_attachments);
                        if (cache_has_attachments == 1)
                        {
                            var attachmentIdList = messageAttachmentJoinDynamicList.FindAll(maj => DynamicConvert.ToSafeInt(maj.message_id) != 0 && DynamicConvert.ToSafeInt(maj.message_id) == smsId);
                            sms.Remark += LanguageHelper.GetString(Languagekeys.PluginSMS_Attachment) + "; ";

                            foreach (var oneAttachIdObj in attachmentIdList)
                            {
                                int attId = DynamicConvert.ToSafeInt(oneAttachIdObj.attachment_id);

                                var attachObj =
                                    attachmentDynamicList.FirstOrDefault(att => DynamicConvert.ToSafeInt(att.ROWID) == attId);

                                if (attachObj != null)
                                {
                                    string attPath = DynamicConvert.ToSafeString(attachObj.filename);
                                    attPath = attPath.Replace('/', '\\').TrimStart('~');
                                    sms.Remark += FileHelper.ConnectPath(attendDirPath, attPath) + ";";
                                }
                            }
                        }

                        #endregion

                        sms.Remark = sms.Remark.TrimEnd("；");
                        if (sms.Remark.Contains(LanguageHelper.GetString(Languagekeys.PluginSMS_NotRead)))
                            sms.ReadState = EnumReadState.Unread;
                        else
                            sms.ReadState = EnumReadState.Read;

                        if (sms.Number.IsInvalid() && sms.Content.IsInvalid())
                        {
                            continue;
                        }

                        if (sms.Number.IsInvalid())
                        {
                            var address = DataParseHelper.NumberToStu(DynamicConvert.ToSafeString(smsObj.address));
                            sms.Number = DynamicConvert.ToSafeString(address);
                        }

                        dataSource.Items.Add(sms);
                    }
                });

            }
            finally
            {
                mainContext?.Dispose();
                mainContext = null;
            }
        }

        private string GetPhoteNumber(int handleId, IEnumerable<dynamic> handleDynamicList)
        {
            var handleObj = handleDynamicList.FirstOrDefault(h => DynamicConvert.ToSafeInt(h.ROWID) == handleId);

            if (handleObj != null)
            {
                return DataParseHelper.NumberToStu(handleObj.id);
            }

            return string.Empty;
        }
    }
}
