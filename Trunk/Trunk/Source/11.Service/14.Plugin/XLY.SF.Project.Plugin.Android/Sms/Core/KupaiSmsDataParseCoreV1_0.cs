/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/31 10:10:50 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Linq;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 酷派手机备份短信解析
    /// </summary>
    public class KupaiSmsDataParseCoreV1_0
    {
        /// <summary>
        /// sms.vmg文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// 酷派手机备份短信解析
        /// </summary>
        /// <param name="mainDbPath">sms.vmg文件路径</param>
        public KupaiSmsDataParseCoreV1_0(string mainDbPath)
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
            var arrData = allText.Split(new string[] { "BEGIN:VMSG", "END:VMSG" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var d in arrData)
            {
                var datas = d.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (datas.IsInvalid())
                {
                    continue;
                }

                SMS sms = new SMS();
                sms.DataState = EnumDataState.Normal;

                //号码
                string temp = datas.FirstOrDefault(s => s.StartsWith("TEL:"));
                if (temp.IsValid() && Regex.IsMatch(temp, @"\+{0,1}\d+"))
                {
                    sms.Number = Regex.Match(temp, @"\+{0,1}\d+").Value;
                }

                //类型
                temp = datas.FirstOrDefault(s => s.StartsWith("RECEVIE_Date:"));
                if (temp.Contains("Thursday, January 1, 1970 8:00:00 AM"))
                {//自己发送的短信，接收时间永远是这个值
                    sms.SmsState = EnumSMSState.SendSMS;
                }
                else
                {
                    sms.SmsState = EnumSMSState.ReceiveSMS;
                }

                //时间和内容
                int beginBody = datas.IndexOf("BEGIN:VBODY");
                int endBody = datas.IndexOf("END:VBODY");
                if (endBody - beginBody < 2)
                {//至少2行，第一行是发送时间，第二行开始是消息内容
                    continue;
                }
                sms.StartDate = GetDateTime(datas[beginBody + 1].TrimStart("Date:"));
                sms.Content = string.Join("\r\n", datas.Skip(beginBody + 2).Take(endBody - beginBody - 2));

                //是否已读
                temp = datas.FirstOrDefault(s => s.StartsWith("CP_READ:"));
                if (temp == "CP_READ:0")
                {
                    sms.ReadState = EnumReadState.Unread;
                }
                else
                {
                    sms.ReadState = EnumReadState.Read;
                }

                datasource.Items.Add(sms);
            }
        }

        private DateTime GetDateTime(string datetime)
        {//Wednesday, November 2, 2016 8:20:57 AM China Standard Time
            var mc = Regex.Match(datetime, @"(\w+), (\w+) (\d+), (\d+) (\d+):(\d+):(\d+) (\w{2})");

            int year, month, day, hour, min, second;

            year = int.Parse(mc.Groups[4].Value);
            day = int.Parse(mc.Groups[3].Value);
            hour = int.Parse(mc.Groups[5].Value);
            if ("PM" == mc.Groups[8].Value && hour < 12)
            {
                hour += 12;
            }
            min = int.Parse(mc.Groups[6].Value);
            second = int.Parse(mc.Groups[7].Value);
            switch (mc.Groups[2].ToString())
            {
                case "January":
                    month = 1;
                    break;
                case "February":
                    month = 2;
                    break;
                case "March":
                    month = 3;
                    break;
                case "April":
                    month = 4;
                    break;
                case "May":
                    month = 5;
                    break;
                case "June":
                    month = 6;
                    break;
                case "July":
                    month = 7;
                    break;
                case "August":
                    month = 8;
                    break;
                case "September":
                    month = 9;
                    break;
                case "October":
                    month = 10;
                    break;
                case "November":
                    month = 11;
                    break;
                //case "December":
                default:
                    month = 12;
                    break;
            }

            return new DateTime(year, month, day, hour, min, second);
        }

    }
}
