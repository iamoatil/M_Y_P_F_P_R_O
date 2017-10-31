/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 13:47:58 
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

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS通话记录数据解析
    /// </summary>
    internal class IOSCallDataParseCoreV2_0
    {
        /// <summary>
        /// HomeDomain\Library\CallHistoryDB\CallHistory.storedata文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// HomeDomain\Library\AddressBook\AddressBook.sqlitedb文件路径
        /// </summary>
        private string AddrDbPath { get; set; }

        /// <summary>
        /// IOS联系人数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">HomeDomain\Library\CallHistoryDB\CallHistory.storedata文件路径</param>
        /// <param name="addrDbPath">HomeDomain\Library\AddressBook\AddressBook.sqlitedb文件路径</param>
        public IOSCallDataParseCoreV2_0(string mainDbPath, string addrDbPath = null)
        {
            MainDbPath = mainDbPath;
            AddrDbPath = addrDbPath;
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

            SqliteContext mainContext = null;

            try
            {
                //主数据库对象
                var mainDbPath = SqliteRecoveryHelper.DataRecovery(MainDbPath, "", "ZCALLRECORD", true);
                mainContext = new SqliteContext(mainDbPath);

                IEnumerable<dynamic> addressDynamicList = null;

                if (FileHelper.IsValid(AddrDbPath))
                {
                    var addressDbPath = SqliteRecoveryHelper.DataRecovery(AddrDbPath, @"chalib\IOS_Contact\AddressBook.sqlitedb.charactor", "ABMultiValue,ABPerson", true);
                    SqliteContext addrContext = new SqliteContext(addressDbPath);

                    addressDynamicList = addrContext.Find("select p.[value],p.[record_id],v.[Last],v.[ROWID] from ABMultiValue p left join ABPerson v on p.[record_id]=v.[ROWID] where p.[property]=3");

                    addrContext.Dispose();
                    addrContext = null;
                }

                mainContext.UsingSafeConnection("select XLY_DataType,ZDURATION,Z_OPT,ZDURATION,ZORIGINATED,cast(ZADDRESS as varchar) as number, cast(ZDATE as DATETIME) as phonetime from ZCALLRECORD", r =>
                    {
                        Call call;
                        dynamic callObj;

                        while (r.Read())
                        {
                            callObj = r.ToDynamic();
                            call = new Call();

                            call.Number = DataParseHelper.NumberToStu(DynamicConvert.ToSafeString(callObj.number));
                            // 号码过滤,验证号码长度
                            if (!DataParseHelper.ValidateNumber(call.Number))
                            {
                                continue;
                            }

                            if (addressDynamicList.IsValid())
                            {
                                var addressname = addressDynamicList.FirstOrDefault(o => DynamicConvert.ToSafeString(o.value).Replace("-", "").Equals(callObj.number));
                                if (addressname != null)
                                {
                                    call.Name = FragmentHelper.RemoveNullityDataNew(DynamicConvert.ToSafeString(addressname.Last));
                                }
                            }

                            call.DataState = DynamicConvert.ToEnumByValue(callObj.XLY_DataType, EnumDataState.Normal);
                            call.DurationSecond = DynamicConvert.ToSafeInt(callObj.ZDURATION);

                            string time = DynamicConvert.ToSafeString(callObj.phonetime).Insert(0, "1");
                            double opertime = time.ToDouble() - 21692848;
                            call.StartDate = DynamicConvert.ToSafeDateTime(opertime);

                            call.Type = GetCallV10Status(DynamicConvert.ToSafeInt(callObj.ZORIGINATED), call.DurationSecond);

                            datasource.Items.Add(call);
                        }
                    });
            }
            finally
            {
                mainContext?.Dispose();
                mainContext = null;
            }
        }

        private EnumCallType GetCallV10Status(int zopt, int durationSecond)
        {
            switch (zopt)
            {
                case 0:
                case 2:
                    if (durationSecond <= 0)
                    {
                        return EnumCallType.MissedCallIn;
                    }
                    return EnumCallType.CallIn;
                case 1:
                    if (durationSecond <= 0)
                    {
                        return EnumCallType.MissedCallOut;
                    }
                    return EnumCallType.CallOut;
                default:
                    return EnumCallType.None;
            }

        }

    }
}

