/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/31 10:10:50 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 华为手机备份短信解析
    /// </summary>
    public class HuaweiSmsDataParseCoreV1_0
    {
        /// <summary>
        /// sms.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// contact.db文件路径
        /// </summary>
        private string ContactDbPath { get; set; }

        /// <summary>
        /// 华为手机备份短信解析
        /// </summary>
        /// <param name="mainDbPath">sms.db文件路径</param>
        /// <param name="contactDbPath">contact.db文件路径</param>
        public HuaweiSmsDataParseCoreV1_0(string mainDbPath, string contactDbPath)
        {
            MainDbPath = mainDbPath;
            ContactDbPath = contactDbPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public void BuildData(CallDataSource datasource)
        {
            if (!FileHelper.IsValid(MainDbPath))
            {
                return;
            }

            using (var context = new SqliteContext(MainDbPath))
            {
                using (var contactDb = new SqliteContext(ContactDbPath))
                {
                    var smsList = context.Find(new SQLiteString("SELECT address,date,body,type,read FROM sms_tb"));
                    foreach (var smsData in smsList)
                    {
                        SMS smsTemp = new SMS();
                        smsTemp.Content = DynamicConvert.ToSafeString(smsData.body);
                        smsTemp.Number = DynamicConvert.ToSafeString(smsData.address);
                        smsTemp.Number = smsTemp.Number.TrimStart("+86");
                        smsTemp.ContactName = FindName(contactDb, smsTemp.Number);
                        smsTemp.StartDate = new DateTime(1970, 1, 1).AddSeconds(DynamicConvert.ToSafeLong(smsData.date) / 1000).AddHours(8);

                        string type = DynamicConvert.ToSafeString(smsData.type);
                        switch (type)
                        {
                            case "1":
                                smsTemp.SmsState = EnumSMSState.ReceiveSMS;

                                string read = DynamicConvert.ToSafeString(smsData.read);
                                switch (read)
                                {
                                    case "1":
                                        smsTemp.ReadState = EnumReadState.Read;
                                        //smsTemp.SmsReadState = LanguageHelper.Get("LANGKEY_YiDu_01076");
                                        break;
                                    case "0":
                                        smsTemp.ReadState = EnumReadState.Unread;
                                        //smsTemp.SmsReadState = LanguageHelper.Get("LANGKEY_WeiDu_01077");
                                        break;
                                }
                                break;
                            case "2":
                                smsTemp.SmsState = EnumSMSState.SendSMS;
                                break;
                            case "3":
                                smsTemp.SmsState = EnumSMSState.DraftSMS;
                                break;
                            case "5":
                                smsTemp.SmsState = EnumSMSState.SendSMS;
                                break;
                            default:
                                smsTemp.SmsState = EnumSMSState.None;
                                break;
                        }

                        smsTemp.DataState = EnumDataState.Normal;

                        datasource.Items.Add(smsTemp);
                    }

                    var smsListA = context.Find(new SQLiteString("SELECT send_msg_status,msg_content,msg_date,address FROM table_broadcastchat_tb"));
                    foreach (var smsData in smsListA)
                    {
                        SMS smsTemp = new SMS();
                        smsTemp.Content = DynamicConvert.ToSafeString(smsData.msg_content);
                        smsTemp.Number = DynamicConvert.ToSafeString(smsData.address);
                        smsTemp.Number = smsTemp.Number.TrimStart("+86");
                        smsTemp.ContactName = FindName(contactDb, smsTemp.Number);
                        smsTemp.StartDate = new DateTime(1970, 1, 1).AddSeconds(DynamicConvert.ToSafeLong(smsData.msg_date) / 1000).AddHours(8);

                        string type = DynamicConvert.ToSafeString(smsData.send_msg_status);
                        switch (type)
                        {
                            case "2"://群发
                            case "5"://群发
                                smsTemp.SmsState = EnumSMSState.SendSMS;
                                break;
                            case "3"://发送
                                smsTemp.SmsState = EnumSMSState.SendSMS;
                                break;
                            case "8"://草稿箱
                                smsTemp.SmsState = EnumSMSState.DraftSMS;
                                break;
                            default:
                                smsTemp.SmsState = EnumSMSState.None;
                                break;
                        }

                        smsTemp.DataState = EnumDataState.Normal;

                        datasource.Items.Add(smsTemp);
                    }
                }
            }
        }

        private string FindName(SqliteContext context, string phonenumber)
        {
            if (null == context)
            {
                return string.Empty;
            }

            string sqlStr = string.Format(@"SELECT
                                          	b.data1
                                          FROM
                                          	data_tb a,
                                          	data_tb b
                                          WHERE
                                          	a.mimetype = 'vnd.android.cursor.item/phone_v2'
                                          AND a.data1 = '{0}'
                                          AND b.mimetype = 'vnd.android.cursor.item/name'
                                          AND a.raw_contact_id = b.raw_contact_id", phonenumber);

            var res = context.Find(new SQLiteString(sqlStr));
            if (res.IsValid())
            {
                return DynamicConvert.ToSafeString(res.First().data1);
            }

            return string.Empty;
        }
    }
}
