/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/31 10:10:50 
 * explain :  
 *
*****************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// OPPO手机备份短信解析
    /// </summary>
    public class OppoSmsDataParseCoreV1_0
    {
        /// <summary>
        /// sms.vmsg文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// OPPO手机备份短信解析
        /// </summary>
        /// <param name="mainDbPath">sms.vmsg文件路径</param>
        public OppoSmsDataParseCoreV1_0(string mainDbPath)
        {
            MainDbPath = mainDbPath;
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

            var lstDatas = File.ReadLines(MainDbPath);
            SMS smsTmp = new SMS();
            foreach (var item in lstDatas)
            {
                if (item.Equals("END:VMSG"))
                {
                    datasource.Items.Add(smsTmp);

                    smsTmp = new SMS();
                    smsTmp.DataState = EnumDataState.Normal;
                }
                CreateSMS(item, ref smsTmp);
            }
        }

        private void CreateSMS(string data, ref SMS createItem)
        {
            var tmpValue1 = data.Split(':');
            var tmpValue2 = data.Split(';');

            //电话
            if (tmpValue1[0].Equals("TEL") && DataParseHelper.ValidateNumber(tmpValue1[1]))
            {
                createItem.Number = tmpValue1[1];
            }
            //消息类型
            else if (tmpValue1[0].Equals("X-BOX"))
            {
                createItem.SmsState = tmpValue1[1] == "INBOX" ? EnumSMSState.ReceiveSMS : EnumSMSState.SendSMS;
            }
            //日期
            else if (tmpValue1[0].Equals("Date"))
            {
                DateTime dateTmp;
                if (DateTime.TryParse(data.Remove(0, 5), out dateTmp))
                {
                    createItem.StartDate = dateTmp;
                }
            }
            string dx = "";
            if (tmpValue2[0].Equals("Subject") || tmpValue2[0].StartsWith("Subject"))
            {
                dx = tmpValue1[1];
            }
            else if (data.StartsWith("=") && createItem.Content != "")
            {
                dx = data;
            }

            if ("" != dx)
            {
                createItem.Content = createItem.Content + dx;
                if (!dx.EndsWith("="))
                {
                    if (Regex.IsMatch(createItem.Content, @"(=[0-9A-F]{2})+"))
                    {
                        var datas = createItem.Content.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                        var bDatas = datas.ToList().ConvertAll((t) =>
                        {
                            return Convert.ToByte(t, 16);
                        });
                        createItem.Content = Encoding.UTF8.GetString(bDatas.ToArray());
                    }
                }
            }

        }

    }
}
