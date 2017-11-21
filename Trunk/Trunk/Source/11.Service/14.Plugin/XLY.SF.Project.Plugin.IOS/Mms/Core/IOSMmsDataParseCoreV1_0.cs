/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 9:55:00 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS彩信解析
    /// </summary>
    internal class IOSMmsDataParseCoreV1_0
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
        /// IOS彩信数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">HomeDomain\Library\SMS\sms.db文件路径</param>
        /// <param name="attendDirPath">附件路径  MediaDomain\</param>
        public IOSMmsDataParseCoreV1_0(string mainDbPath, string attendDirPath)
        {
            MainDbPath = mainDbPath;
            MediaDomainPath = attendDirPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="dataSource"></param>
        public void BuildData(MMSDataSource dataSource)
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

                var messageSource = mainContext.Find(new SQLiteString("SELECT * FROM MESSAGE WHERE CACHE_HAS_ATTACHMENTS = 1 and ROWID not null"));
                var allContact = mainContext.FindByName("HANDLE");
                foreach (var message in messageSource)
                {
                    var sb = new StringBuilder(
                            "select distinct atta.[filename],atta.[mime_type],atta.[created_date] from MESSAGE_ATTACHMENT_JOIN mes ");
                    sb.Append("left join attachment atta on mes.attachment_id = atta.[ROWID] ");
                    sb.AppendFormat("where mes.[message_id] = {0} and atta.filename not null", DynamicConvert.ToSafeString(message.ROWID));
                    var contact = allContact.First(cct => DynamicConvert.ToSafeInt(cct.ROWID) == DynamicConvert.ToSafeInt(message.handle_id));
                    var mms = new MMS();
                    mms.SenderName = DynamicConvert.ToSafeString(contact.uncanonicalized_id);
                    mms.SendState = DynamicConvert.ToSafeInt(message.is_from_me) == 1
                                        ? EnumSendState.Send
                                        : EnumSendState.Receive;
                    mms.Date = DynamicConvert.ToSafeDateTime(message.date);
                    mms.ReadDate = DynamicConvert.ToSafeDateTime(message.date_read);
                    mms.Content = DynamicConvert.ToSafeString(message.text).TrimStart('?') + Environment.NewLine;
                    // 获取当前消息的所有附件;
                    var attaSource = mainContext.Find(new SQLiteString(sb.ToString()));
                    foreach (var atta in attaSource)
                    {
                        if (FragmentHelper.IsValidFragment(atta)) continue;
                        string attPath = DynamicConvert.ToSafeString(atta.filename).TrimStart('~').TrimStart('/').Replace("/", @"\");
                        mms.Content += string.Format("{0}{1}", Path.Combine(attendDirPath, attPath), Environment.NewLine);
                    }

                    mms.Content = FragmentHelper.RemoveNullityDataNew(mms.Content.Trim());
                    // 验证内容是否为空
                    if (FragmentHelper.IsEmptyString(mms.Content))
                    {
                        continue;
                    }

                    dataSource.Items.Add(mms);
                }
            }
            finally
            {
                mainContext?.Dispose();
                mainContext = null;
            }
        }
    }
}
