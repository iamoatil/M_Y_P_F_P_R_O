/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/31 10:10:50 
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
    /// 三星手机通话记录解析
    /// </summary>
    public class SamsungCallDataParseCoreV1_0
    {
        /// <summary>
        /// logs.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// contacts2.db文件路径
        /// </summary>
        private string ContactDbPath { get; set; }

        /// <summary>
        /// 三星手机通话记录解析
        /// </summary>
        /// <param name="mainDbPath">logs.db文件路径</param>
        /// <param name="mainDbPath">contacts2.db文件路径</param>
        public SamsungCallDataParseCoreV1_0(string mainDbPath, string contactDb)
        {
            MainDbPath = mainDbPath;
            ContactDbPath = contactDb;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public void BuildData(CallDataSource datasource)
        {
            List<Call> list = new List<Call>();
            list.AddRange(GetFromLog());
            list.AddRange(GetFromDefault());

            foreach (var item in list)
            {
                datasource.Items.Add(item);
            }
        }

        private List<Call> GetFromLog()
        {
            SqliteContext context = null;
            try
            {
                var items = new List<Call>();

                if (!FileHelper.IsValid(MainDbPath))
                {
                    return items;
                }

                var newFile = SqliteRecoveryHelper.DataRecovery(MainDbPath, @"chalib\com.android.providers.telephony\logs.db.charactor", "logs", true);
                // 处理新文件
                context = new SqliteContext(newFile);
                var calls = context.Find(new SQLiteString("select * from logs where logtype=100 or logtype=256"));
                if (calls.IsInvalid())
                {
                    return items;
                }
                TryParseCall(items, calls);
                return items;
            }
            catch
            {
                return new List<Call>();
            }
            finally
            {
                if (null != context)
                {
                    context.Dispose();
                }
            }
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
                        item.Type = EnumCallType.CallIn;
                        break;
                    case 3:
                    case 5:
                        item.Type = EnumCallType.MissedCallIn;
                        break;
                    case 2:
                        item.Type = item.DurationSecond > 0 ? EnumCallType.CallOut : EnumCallType.MissedCallOut;
                        break;
                    default:
                        item.Type = EnumCallType.None;
                        break;
                }

                if (item.DataState == EnumDataState.Normal)
                {
                    item.Name = DynamicConvert.ToSafeString(v.name);
                }

                item.StartDate = DynamicConvert.ToSafeDateTime(v.date);

                items.Add(item);
            }
        }

        private List<Call> GetFromDefault()
        {
            SqliteContext context = null;
            try
            {
                var items = new List<Call>();

                var file = ContactDbPath;
                if (!FileHelper.IsValid(file))
                {
                    return new List<Call>();
                }
                var newFile = SqliteRecoveryHelper.DataRecovery(file, @"chalib\com.android.providers.contacts\contacts2.db.charactor", "calls", true);
                // 处理新文件
                context = new SqliteContext(newFile);
                var calls = context.FindByName("calls");
                if (calls.IsInvalid())
                {
                    return items;
                }
                TryParseCallDeafult(items, calls);
                return items;
            }
            catch
            {
                return new List<Call>();
            }
            finally
            {
                if (null != context)
                {
                    context.Dispose();
                }
            }
        }

        private void TryParseCallDeafult(List<Call> items, IEnumerable<dynamic> calls)
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
                    default:
                        item.Type = EnumCallType.None;
                        break;
                }
                if (item.DataState == EnumDataState.Normal)
                {
                    item.Name = DynamicConvert.ToSafeString(v.name);
                }
                item.StartDate = DynamicConvert.ToSafeDateTime(v.date);
                items.Add(item);
            }
        }
    }
}
