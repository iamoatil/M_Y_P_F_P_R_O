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
    /// 华为手机备份通话记录解析
    /// </summary>
    public class HuaweiCallDataParseCoreV1_0
    {
        /// <summary>
        /// calllog.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// contact.db文件路径
        /// </summary>
        private string ContactDbPath { get; set; }

        /// <summary>
        /// 华为手机备份通话记录解析
        /// </summary>
        /// <param name="mainDbPath">calllog.db文件路径</param>
        /// <param name="contactDbPath">contact.db文件路径</param>
        public HuaweiCallDataParseCoreV1_0(string mainDbPath, string contactDbPath)
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

            SqliteContext context = null;
            SqliteContext contactcontext = null;

            try
            {
                context = new SqliteContext(MainDbPath);
                if (FileHelper.IsValid(ContactDbPath))
                {
                    contactcontext = new SqliteContext(ContactDbPath);
                }

                var calllogList = context.Find(new SQLiteString("SELECT number,date,duration,type,ring_times FROM calls_tb"));

                foreach (var calllogdata in calllogList)
                {
                    Call callTmp = new Call();
                    callTmp.Number = DynamicConvert.ToSafeString(calllogdata.number);
                    callTmp.Name = FindName(contactcontext, callTmp.Number);
                    callTmp.Type = ConvertToEnumCallType(DynamicConvert.ToSafeString(calllogdata.type), DynamicConvert.ToSafeString(calllogdata.duration));
                    callTmp.DataState = EnumDataState.Normal;
                    callTmp.StartDate = new DateTime(1970, 1, 1).AddSeconds(DynamicConvert.ToSafeLong(calllogdata.date) / 1000).AddHours(8);
                    callTmp.DurationSecond = DynamicConvert.ToSafeInt(calllogdata.duration);

                    datasource.Items.Add(callTmp);
                }
            }
            catch
            {

            }
            finally
            {
                context?.Dispose();
                contactcontext?.Dispose();
                context = null;
                contactcontext = null;
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
                return DynamicConvert.ToSafeString(res.FirstOrDefault().data1);
            }

            return string.Empty;
        }


        private EnumCallType ConvertToEnumCallType(string type, string duration)
        {
            switch (type)
            {
                case "1":
                case "3":
                    return "0" == duration ? EnumCallType.MissedCallIn : EnumCallType.CallIn;
                case "2":
                    return "0" == duration ? EnumCallType.MissedCallOut : EnumCallType.CallOut;
            }

            return EnumCallType.None;
        }

    }
}
