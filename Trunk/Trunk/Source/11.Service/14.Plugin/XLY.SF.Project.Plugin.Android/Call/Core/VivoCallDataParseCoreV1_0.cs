/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/31 10:10:50 
 * explain :  
 *
*****************************************************************************/

using Newtonsoft.Json.Linq;
using System;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// Vivo手机备份通话记录解析
    /// </summary>
    public class VivoCallDataParseCoreV1_0
    {
        /// <summary>
        /// calllog.json文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// Vivo手机备份通话记录解析
        /// </summary>
        /// <param name="mainDbPath">calllog.json文件路径</param>
        public VivoCallDataParseCoreV1_0(string mainDbPath)
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

            var list = JArray.Parse(System.IO.File.ReadAllText(MainDbPath));
            foreach (JObject call in list)
            {
                var item = new Call();
                item.DataState = EnumDataState.Normal;
                item.Number = call["number"].ToSafeString();
                // 号码过滤,验证号码长度
                if (!DataParseHelper.ValidateNumber(item.Number))
                {
                    continue;
                }

                item.Name = call["name"].ToSafeString();

                int duration = 0, type = 0;
                long date = 0;
                int.TryParse(call["duration"].ToSafeString(), out duration);
                int.TryParse(call["type"].ToSafeString(), out type);
                long.TryParse(call["date"].ToSafeString(), out date);

                item.DurationSecond = duration;
                switch (type)
                {
                    case 1:
                        item.Type = 0 == duration ? EnumCallType.MissedCallIn : EnumCallType.CallIn;
                        break;
                    case 2:
                        item.Type = 0 == duration ? EnumCallType.MissedCallOut : EnumCallType.CallOut;
                        break;
                    case 3:
                        item.Type = EnumCallType.MissedCallIn;
                        item.DurationSecond = 0;
                        break;
                    default:
                        item.Type = EnumCallType.None;
                        break;
                }

                item.StartDate = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(date / 1000).AddHours(8);

                datasource.Items.Add(item);
            }
        }
    }
}
