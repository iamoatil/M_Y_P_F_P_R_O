/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/31 10:10:50 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// Vivo手机备份短信解析
    /// </summary>
    public class VivoSmsDataParseCoreV1_0
    {
        /// <summary>
        /// sms.json文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// contact.vcf文件路径
        /// </summary>
        private string ContactDbPath { get; set; }

        /// <summary>
        /// Vivo手机备份短信解析
        /// </summary>
        /// <param name="mainDbPath">sms.json文件路径</param>
        /// <param name="contactDbPath">contact.vcf文件路径</param>
        public VivoSmsDataParseCoreV1_0(string mainDbPath, string contactDbPath)
        {
            MainDbPath = mainDbPath;
            ContactDbPath = contactDbPath;
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

            var names = GetNumberName(ContactDbPath);

            DataContractJsonSerializer dcJs = new DataContractJsonSerializer(typeof(List<VivoSMSModel>));
            using (FileStream fs = new FileStream(MainDbPath, FileMode.Open))
            {
                var readDatas = dcJs.ReadObject(fs) as List<VivoSMSModel>;
                foreach (var item in readDatas)
                {
                    if (ValidateEntityItem(item))
                    {
                        SMS tmpSMS = new SMS();
                        tmpSMS.DataState = EnumDataState.Normal;
                        tmpSMS.Number = item.address;
                        tmpSMS.SmsState = int.Parse(item.type) == 1 ? EnumSMSState.ReceiveSMS : EnumSMSState.SendSMS;

                        if (names.Keys.Contains(tmpSMS.Number))
                        {
                            tmpSMS.ContactName = names[tmpSMS.Number];
                        }
                        else if (names.Keys.Contains(tmpSMS.Number.TrimStart("+86")))
                        {
                            tmpSMS.ContactName = names[tmpSMS.Number.TrimStart("+86")];
                        }
                        else if (names.Keys.Contains("+86" + tmpSMS.Number))
                        {
                            tmpSMS.ContactName = names["+86" + tmpSMS.Number];
                        }

                        double dateTmp = double.Parse(item.date) / 1000;
                        tmpSMS.StartDate = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1).AddSeconds(dateTmp));

                        tmpSMS.ReadState = int.Parse(item.status) == -1 ? EnumReadState.Read : EnumReadState.Unread;

                        tmpSMS.Content = item.body;
                        datasource.Items.Add(tmpSMS);
                    }
                }
            }
        }

        /// <summary>
        /// 验证序列化结果
        /// </summary>
        /// <param name="entityItem">序列化后的实体</param>
        /// <returns></returns>
        private bool ValidateEntityItem(VivoSMSModel entityItem)
        {
            bool result = false;
            int sendState = -1;
            double dateValue;
            result = DataParseHelper.ValidateNumber(entityItem.address);
            result = int.TryParse(entityItem.type, out sendState);
            result = double.TryParse(entityItem.date, out dateValue);
            result = int.TryParse(entityItem.status, out sendState);
            return result;
        }

        private Dictionary<string, string> GetNumberName(string path)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (!File.Exists(path))
            {
                return dic;
            }

            try
            {
                string allText = System.IO.File.ReadAllText(path);
                var arrData = allText.Split(new string[] { "BEGIN:VCARD", "END:VCARD" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var d in arrData)
                {
                    var datas = d.Trim('\r', '\n').Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (datas.IsInvalid())
                    {
                        continue;
                    }

                    string Name = "", Number = "";

                    string temp = datas.FirstOrDefault(s => s.StartsWith("TEL;TYPE=CELL:"));
                    if (temp.IsValid())
                    {
                        Number = temp.TrimStart("TEL;TYPE=CELL:").Replace("-", "").Trim();
                    }

                    temp = datas.FirstOrDefault(s => s.StartsWith("FN:"));
                    if (temp.IsValid())
                    {
                        Name = temp.TrimStart("FN:").Trim();
                    }

                    if (Name.IsValid() || Number.IsValid())
                    {
                        if (!dic.Keys.Contains(Number))
                        {
                            dic.Add(Number, Name);
                        }
                    }
                }
            }
            catch
            {
            }

            return dic;
        }
    }
}
