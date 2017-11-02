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

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 魅族手机备份通话记录解析
    /// </summary>
    public class MeizuCallDataParseCoreV1_0
    {
        /// <summary>
        /// calllog.vcl文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// 酷派手机备份通话记录解析
        /// </summary>
        /// <param name="mainDbPath">calllog.vcl文件路径</param>
        public MeizuCallDataParseCoreV1_0(string mainDbPath)
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

            string allText = System.IO.File.ReadAllText(MainDbPath);
            var arrData = allText.Split(new string[] { "BEGIN:CALLLOG", "END:CALLLOG" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var d in arrData)
            {
                var datas = d.Trim('\r', '\n').Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (datas.IsInvalid())
                {
                    continue;
                }

                Call call = new Call();
                call.DataState = EnumDataState.Normal;

                call.Number = datas.FirstOrDefault(s => s.StartsWith("X-NUMBER:")).TrimStart("X-NUMBER:");
                call.DurationSecond = int.Parse(datas.FirstOrDefault(s => s.StartsWith("X-DURATION:")).TrimStart("X-DURATION:"));
                call.StartDate = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(UInt64.Parse(datas.FirstOrDefault(s => s.StartsWith("X-DATE:")).TrimStart("X-DATE:")) / 1000).AddHours(8);

                switch (datas.FirstOrDefault(s => s.StartsWith("X-TYPE:")).TrimStart("X-TYPE:"))
                {
                    case "1":
                        call.Type = call.DurationSecond > 0 ? EnumCallType.CallIn : EnumCallType.MissedCallIn;
                        break;
                    case "2":
                        call.Type = call.DurationSecond > 0 ? EnumCallType.CallOut : EnumCallType.MissedCallOut;
                        break;
                    case "3":
                        call.Type = EnumCallType.MissedCallIn;
                        break;
                }

                datasource.Items.Add(call);
            }
        }
    }
}
