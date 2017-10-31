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
    internal class IOSCallDataParseCoreV1_0
    {
        /// <summary>
        /// WirelessDomain\Library\CallHistory\call_history.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// IOS联系人数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">WirelessDomain\Library\CallHistory\call_history.db文件路径</param>
        public IOSCallDataParseCoreV1_0(string mainDbPath)
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

            SqliteContext mainContext = null;

            try
            {
                //主数据库对象
                var mainDbPath = SqliteRecoveryHelper.DataRecovery(MainDbPath, @"chalib\IOS_Call\call_history.db.charactor", "call", true);
                mainContext = new SqliteContext(mainDbPath);

                mainContext.UsingSafeConnection("SELECT * FROM call", r =>
                 {
                     Call call;
                     dynamic callObj;

                     while (r.Read())
                     {
                         callObj = r.ToDynamic();
                         call = new Call();

                         call.DataState = DynamicConvert.ToEnumByValue(callObj.XLY_DataType, EnumDataState.Normal);
                         call.Number = DataParseHelper.NumberToStu(DynamicConvert.ToSafeString(callObj.address));
                         // 号码过滤,验证号码长度
                         if (!DataParseHelper.ValidateNumber(call.Number))
                         {
                             continue;
                         }

                         call.DurationSecond = DynamicConvert.ToSafeInt(callObj.duration);
                         call.StartDate = DynamicConvert.ToSafeDateTime(callObj.date);

                         call.Type = GetCallStatus(DynamicConvert.ToSafeInt(callObj.flags), call.DurationSecond);

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

        private EnumCallType GetCallStatus(int flag, int durationSecond)
        {
            switch (flag)
            {
                case 0:
                case 16:
                case 65536:
                    {
                        if (durationSecond <= 0)
                        {
                            return EnumCallType.MissedCallIn;
                        }

                        return EnumCallType.CallIn;

                    }
                case 1:
                    if (durationSecond <= 0)
                    {
                        return EnumCallType.MissedCallOut;
                    }
                    return EnumCallType.CallOut;
                case 2:
                    if (durationSecond <= 0)
                    {
                        return EnumCallType.MissedCallIn;
                    }

                    return EnumCallType.CallIn;
                case 9:
                case 65545:
                case 17:
                case 1769481:
                case 4521993:
                case 1769472:
                    {
                        if (durationSecond <= 0)
                        {
                            return EnumCallType.MissedCallOut;
                        }

                        return EnumCallType.CallOut;
                    }
                case 4:
                    {
                        if (durationSecond > 0)
                        {
                            return EnumCallType.CallIn;
                        }
                        return EnumCallType.MissedCallIn;
                    }
                case 5:
                    {
                        if (durationSecond > 0)
                        {
                            return EnumCallType.CallOut;
                        }
                        return EnumCallType.MissedCallOut;
                    }
                case 1114117:
                case 65541:
                case 1507333:
                    return EnumCallType.MissedCallOut;
                case 1507332:
                case 1769476:
                case 65540:
                    return EnumCallType.MissedCallIn;
                default:
                    return EnumCallType.None;
            }

        }
    }
}

