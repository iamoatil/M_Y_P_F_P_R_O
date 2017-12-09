/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 17:13:24 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓通话记录解析类
    /// </summary>
    internal class AndroidCallDataParseCoreV1_0
    {
        /// <summary>
        /// contacts2.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// calls.db文件路径
        /// </summary>
        private string CallsDbPath { get; set; }

        /// <summary>
        /// 安卓通话记录数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">contacts2.db文件路径</param>
        /// <param name="callsDbPath">calls.db文件路径</param>
        public AndroidCallDataParseCoreV1_0(string mainDbPath, string callsDbPath)
        {
            MainDbPath = mainDbPath;
            CallsDbPath = callsDbPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public List<Call> BuildData()
        {
            var items = new List<Call>();

            SqliteContext mainContext = null;
            SqliteContext callsContext = null;

            try
            {
                List<dynamic> list = new List<dynamic>();

                if (FileHelper.IsValid(MainDbPath))
                {
                    var rMainDbFile = SqliteRecoveryHelper.DataRecovery(MainDbPath, @"chalib\com.android.providers.contacts\contacts2.db.charactor", "calls", true);
                    mainContext = new SqliteContext(rMainDbFile);
                    var ls = mainContext.Find("SELECT * FROM calls");
                    if (ls.IsValid())
                    {
                        list.AddRange(ls);
                    }
                }

                if (FileHelper.IsValid(CallsDbPath))
                {
                    var rMainDbFile = SqliteRecoveryHelper.DataRecovery(CallsDbPath, @"chalib\com.android.providers.contacts\contacts2.db.charactor", "calls", true);
                    callsContext = new SqliteContext(rMainDbFile);
                    var ls = callsContext.Find("SELECT * FROM calls");
                    if (ls.IsValid())
                    {
                        list.AddRange(ls);
                    }
                }

                TryParseCall(items, list);
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "AndroidCallDataParseCoreV1_0 BuildData Error!");
            }
            finally
            {
                mainContext?.Dispose();
                callsContext?.Dispose();
                mainContext = null;
                callsContext = null;
            }

            return items;
        }

        private void TryParseCall(List<Call> items, IEnumerable<dynamic> calls)
        {
            foreach (var v in calls)
            {
                var item = new Call();
                item.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(v.XLY_DataType, EnumDataState.Normal);
                item.Number = DataParseHelper.NumberToStu(v.number);
                // 号码过滤,验证号码长度
                if (!DataParseHelper.ValidateNumber(v.number))
                {
                    continue;
                }
                item.DurationSecond = DynamicConvert.ToSafeInt(v.duration);
                int type = DynamicConvert.ToSafeInt(v.type);
                switch (type)
                {
                    case 1:
                    case 3:
                        item.Type = type.ToEnumByValue<EnumCallType>();
                        break;
                    case 2:
                        item.Type = item.DurationSecond > 0 ? EnumCallType.CallOut : EnumCallType.MissedCallOut;
                        break;
                    case 10://新建联系人事件
                        continue;
                    default:
                        item.Type = EnumCallType.None;
                        break;
                }
                if (item.DataState == EnumDataState.Normal)
                {
                    item.Name = DynamicConvert.ToSafeString(v.name);
                }

                if ("1" == DynamicConvert.ToSafeString(v.mark_deleted) || "1" == DynamicConvert.ToSafeString(v.deleted))
                {//红米
                    item.DataState = EnumDataState.Deleted;
                }

                item.StartDate = DynamicConvert.ToSafeDateTime(v.date);
                items.Add(item);
            }
        }
    }
}
