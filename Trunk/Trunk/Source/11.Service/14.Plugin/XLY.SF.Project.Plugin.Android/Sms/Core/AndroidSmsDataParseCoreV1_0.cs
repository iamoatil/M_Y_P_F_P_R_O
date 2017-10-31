/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 17:47:57 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓短信数据解析
    /// </summary>
    internal class AndroidSmsDataParseCoreV1_0
    {
        /// <summary>
        /// mmssms.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        private string ContactDbPath { get; set; }

        /// <summary>
        /// 安卓短信数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">mmssms.db文件路径</param>
        /// <param name="contactDbPath">contacts2.db文件路径</param>
        public AndroidSmsDataParseCoreV1_0(string mainDbPath, string contactDbPath)
        {
            MainDbPath = mainDbPath;
            ContactDbPath = contactDbPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public void BuildData(SmsDataSource datasource)
        {
            if (!FileHelper.IsValid(MainDbPath))
            {
                return;
            }

            SqliteContext mainContext = null;

            try
            {
                string charatorPath = MatchCharatorPath();

                var rMainDbFile = SqliteRecoveryHelper.DataRecovery(MainDbPath, charatorPath, "sms", true);
                mainContext = new SqliteContext(rMainDbFile);

                string createSmsSql = string.Empty;
                var smssList = GetSmssList(mainContext, ref createSmsSql);

                if (smssList.IsInvalid())
                {
                    return;
                }

                var contactList = GetContactList();

                bool isExistDataSent = ValidateTableFieldExist(createSmsSql, "date_sent INTEGER");
                bool isExistReceiveDate = ValidateTableFieldExist(createSmsSql, "receive_date INTEGER");

                SMS item;
                foreach (var sms in smssList)
                {
                    try
                    {
                        item = new SMS();

                        item.Content = DynamicConvert.ToSafeString(sms.content);
                        item.Content = FragmentHelper.RemoveNullityDataNew(item.Content);


                        // 验证内容是否为空
                        if (FragmentHelper.IsEmptyString(item.Content))
                        {
                            continue;
                        }

                        item.SmsState = DynamicConvert.ToEnumByValue<EnumSMSState>(sms.type, EnumSMSState.None);
                        item.Number = DynamicConvert.ToSafeString(sms.number);
                        item.Number = DataParseHelper.NumberToStu(item.Number);
                        item.ContactName = DynamicConvert.ToSafeString(sms.name);
                        if (item.ContactName.IsInvalid())
                        {
                            // 关联获取联系人名称
                            item.ContactName = GetContactName(item.Number, contactList);
                        }

                        item.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(sms.XLY_DataType, EnumDataState.Normal);
                        if (DynamicConvert.ToSafeString(sms.deleted) == "1")
                        {
                            item.DataState = EnumDataState.Deleted;
                        }
                        //item.StartDate = DynamicConvert.ToSafeDateTime(sms.date);

                        item.ReadState = DynamicConvert.ToSafeInt(sms.read) == 0 ? EnumReadState.Unread : EnumReadState.Read;

                        // 如果存在receive_date字段，取其作为发送时间，否则取date_sent字段
                        if (isExistReceiveDate)
                        {
                            item.StartDate = DynamicConvert.ToSafeDateTime(sms.receive_date);
                        }
                        else if (isExistDataSent)
                        {
                            item.StartDate = DynamicConvert.ToSafeDateTime(sms.date_sent);
                        }

                        datasource.Items.Add(item);
                    }
                    catch
                    {
                    }
                }
            }
            finally
            {
                mainContext?.Dispose();
                mainContext = null;
            }
        }

        /// <summary>
        /// 根据mmssms.db，获得sms表（短信）集合
        /// </summary>
        /// <param name="context">数据库</param>
        /// <param name="createSmsSql">创建sms表sql语句，用于判断某个字段是否存在</param>
        /// <returns>返回sms表（短信）集合</returns>
        private IEnumerable<dynamic> GetSmssList(SqliteContext context, ref string createSmsSql)
        {
            string charatorPath = MatchCharatorPath();

            createSmsSql = GetCreateTableSql(context, "sms");

            string sqlStr = @"select ifnull(_id,0) as _id,address as number,date{0},read,type,body as content{1}{2}{3} from sms";
            string part1 = ValidateTableFieldExist(createSmsSql, "date_sent INTEGER") ? ",date_sent" : string.Empty;
            string part2 = ValidateTableFieldExist(createSmsSql, "receive_date INTEGER") ? ",receive_date" : string.Empty;
            string part3 = ValidateTableFieldExist(createSmsSql, "deleted INTEGER") ? ",deleted" : string.Empty;
            string part4 = ValidateTableFieldExist(createSmsSql, "XLY_DataType INTEGER") ? ",XLY_DataType" : string.Empty;
            sqlStr = string.Format(sqlStr, part1, part2, part3, part4);

            return context.Find(sqlStr);
        }

        private IEnumerable<dynamic> GetContactList()
        {
            if (!FileHelper.IsValid(ContactDbPath))
            {
                return new List<dynamic>();
            }

            var nfile = SqliteRecoveryHelper.DataRecovery(ContactDbPath, @"chalib\com.android.providers.contacts\contacts2.db.charactor", "data,mimetypes", true);
            var context = new SqliteContext(nfile);
            string sqlStr = "select A.mimetype_id,A.raw_contact_id,A.data1,A.data4,substr(B.[mimetype],25,length(B.[mimetype])-24) as mimetype from data as A left outer join mimetypes as B on A.[mimetype_id] = B.[_id]";
            var res = context.Find(new SQLiteString(sqlStr));

            if (null != context)
            {
                context.Dispose();
            }

            return res;
        }

        /// <summary>
        /// 根据contactid，获取联系人名称
        /// </summary>
        /// <param name="number">联系人号码</param>
        /// <param name="contactList">联系人列表</param>
        /// <returns>返回联系人名称</returns>
        private string GetContactName(string number, IEnumerable<dynamic> contactList)
        {
            if (number.IsInvalid())
            {
                return string.Empty;
            }

            try
            {
                dynamic contactNumber = contactList.FirstOrDefault(c => c.data1 == number);
                if (contactNumber != null)
                {
                    var contactName = contactList.FirstOrDefault(c => c.raw_contact_id == contactNumber.raw_contact_id && c.mimetype.Contains("name"));
                    if (contactName != null)
                    {
                        return DynamicConvert.ToSafeString(contactName.data1);
                    }
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string MatchCharatorPath()
        {
            string charatorPath;
            var context = new SqliteContext(MainDbPath);
            string createSql = GetCreateTableSql(context, "sms");
            if (ValidateTableFieldExist(createSql, "date_sent INTEGER"))
            {
                // 包含date_sent或receive_date字段的特征库
                if (ValidateTableFieldExist(createSql, "receive_date INTEGER"))
                {
                    charatorPath = @"chalib\com.android.providers.telephony\mmssms(date_sent_receive_date).db.charactor";
                }
                else
                {
                    charatorPath = @"chalib\com.android.providers.telephony\mmssms(date_sent).db.charactor";
                }
            }
            else
            {
                // 默认特征库，最少字段
                charatorPath = @"chalib\com.android.providers.telephony\mmssms_normal.db.charactor";
            }

            return charatorPath;
        }

        /// <summary>
        /// 验证数据表中是否包含某字段
        /// </summary>
        /// <param name="createSql">某个表的创建sql语句</param>
        /// <param name="fieldName">字段关键字</param>
        /// <returns>包含：true；不包含：false</returns>
        private bool ValidateTableFieldExist(string createSql, string fieldName)
        {
            return createSql != null && createSql.Contains(fieldName);
        }

        /// <summary>
        /// 查询某个数据库表的创建sql语句，用于判断表中是否包含某个字段
        /// </summary>
        /// <param name="context">数据库操作对象</param>
        /// <param name="tableName">数据表名</param>
        /// <returns>返回创建表的sql语句</returns>
        private string GetCreateTableSql(SqliteContext context, string tableName)
        {
            try
            {
                string selectSql = string.Format(@"select sql from sqlite_master where tbl_name='{0}' and type='table'", tableName);
                var data = context.Find(new SQLiteString(selectSql));
                return DynamicConvert.ToSafeString(data.FirstOrDefault().sql);
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
