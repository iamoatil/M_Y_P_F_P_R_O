/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/31 10:10:50 
 * explain :  
 *
*****************************************************************************/

using System;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 中兴手机备份通话记录解析
    /// </summary>
    public class ZhongxingCallDataParseCoreV1_0
    {
        /// <summary>
        /// CallHistory.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// 中兴手机备份通话记录解析
        /// </summary>
        /// <param name="mainDbPath">CallHistory.db文件路径</param>
        public ZhongxingCallDataParseCoreV1_0(string mainDbPath)
        {
            MainDbPath = mainDbPath;
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
                var dataList = context.Find("SELECT duration,duration_type,type,number,name,date from calls ORDER BY _id");
                foreach (var calllogdata in dataList)
                {
                    Call callTemp = new Call();
                    callTemp.DataState = EnumDataState.Normal;
                    callTemp.DurationSecond = DynamicConvert.ToSafeInt(calllogdata.duration);
                    callTemp.Number = DynamicConvert.ToSafeString(calllogdata.number);
                    callTemp.Name = DynamicConvert.ToSafeString(calllogdata.name);
                    callTemp.StartDate = new DateTime(1970, 1, 1).AddSeconds(DynamicConvert.ToSafeLong(calllogdata.date) / 1000).AddHours(8);

                    switch ((int)DynamicConvert.ToSafeInt(calllogdata.type))
                    {
                        case 2:
                            callTemp.Type = 0 == callTemp.DurationSecond ? EnumCallType.MissedCallOut : EnumCallType.CallOut;
                            break;
                        default:
                            callTemp.Type = 0 == callTemp.DurationSecond ? EnumCallType.MissedCallIn : EnumCallType.CallIn;
                            break;
                    }

                    datasource.Items.Add(callTemp);
                }
            }
        }
    }
}
