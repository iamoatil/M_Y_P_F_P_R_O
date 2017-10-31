/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 15:45:30 
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
    /// 安卓酷派备份联系人数据解析
    /// </summary>
    internal class KupaiContactsDataParseCoreV1_0
    {
        /// <summary>
        /// contacts.vcf文件路径
        /// </summary>
        private string MainDataFilePath { get; set; }

        /// <summary>
        /// 安卓酷派备份联系人数据解析
        /// </summary>
        /// <param name="mainDbPath">contacts.vcf文件路径</param>
        public KupaiContactsDataParseCoreV1_0(string mainDataFilePath)
        {
            MainDataFilePath = mainDataFilePath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public void BuildData(ContactDataSource datasource)
        {
            if (!FileHelper.IsValid(MainDataFilePath))
            {
                return;
            }

            try
            {
                string allText = System.IO.File.ReadAllText(MainDataFilePath);
                var arrData = allText.Split(new string[] { "BEGIN:VCARD", "END:VCARD" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var d in arrData)
                {
                    var datas = d.Trim('\r', '\n').Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (datas.IsInvalid())
                    {
                        continue;
                    }

                    Contact contact = new Contact();
                    contact.DataState = EnumDataState.Normal;

                    string temp = datas.FirstOrDefault(s => s.StartsWith("TEL;"));
                    if (temp.IsValid() && Regex.IsMatch(temp, @"\+{0,1}\d+"))
                    {
                        contact.Number = Regex.Match(temp, @"\+{0,1}\d+").Value;
                    }

                    temp = datas.FirstOrDefault(s => s.StartsWith("FN;"));
                    if (temp.IsValid() && Regex.IsMatch(temp, @"(=[0-9A-F]{2})+={0,1}"))
                    {
                        string codestr = Regex.Match(temp, @"(=[0-9A-F]{2})+={0,1}").Value;
                        if (codestr.EndsWith("="))
                        {
                            int startIndex = datas.IndexOf(temp) + 1;
                            while (startIndex < datas.Count)
                            {
                                if (!Regex.IsMatch(datas[startIndex], @"(=[0-9A-F]{2})+={0,1}"))
                                {
                                    break;
                                }

                                if (!codestr.EndsWith("="))
                                {
                                    break;
                                }

                                codestr += Regex.Match(datas[startIndex], @"(=[0-9A-F]{2})+={0,1}").Value.TrimStart("=");
                                if (!codestr.EndsWith("="))
                                {
                                    break;
                                }

                                startIndex++;
                            }
                        }

                        contact.Name = DecodeDP(codestr);
                    }
                    else
                    {
                        temp = datas.FirstOrDefault(s => s.StartsWith("N;"));
                        if (temp.IsValid() && Regex.IsMatch(temp, @"(=[0-9A-F]{2})+={0,1}"))
                        {
                            string codestr = Regex.Match(temp, @"(=[0-9A-F]{2})+={0,1}").Value;
                            if (codestr.EndsWith("="))
                            {
                                int startIndex = datas.IndexOf(temp) + 1;
                                while (startIndex < datas.Count)
                                {
                                    if (!Regex.IsMatch(datas[startIndex], @"(=[0-9A-F]{2})+={0,1}"))
                                    {
                                        break;
                                    }

                                    if (!codestr.EndsWith("="))
                                    {
                                        break;
                                    }

                                    codestr += Regex.Match(datas[startIndex], @"(=[0-9A-F]{2})+={0,1}").Value.TrimStart("=");
                                    if (!codestr.EndsWith("="))
                                    {
                                        break;
                                    }

                                    startIndex++;
                                }
                            }

                            contact.Name = DecodeDP(codestr);
                        }
                        else
                        {
                            temp = datas.FirstOrDefault(s => s.StartsWith("FN:"));
                            if (temp.IsValid())
                            {
                                contact.Name = temp.Substring(3).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            }
                            else
                            {
                                temp = datas.FirstOrDefault(s => s.StartsWith("N:"));
                                if (temp.IsValid())
                                {
                                    contact.Name = temp.Substring(2).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                }
                            }
                        }
                    }

                    datasource.Items.Add(contact);
                }
            }
            catch
            {

            }
        }


        //Quoted-Printable 解码  
        public static string DecodeDP(string _ToDecode)
        {
            char[] chars = _ToDecode.ToCharArray();
            byte[] bytes = new byte[chars.Length];
            int bytesCount = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '=')
                {
                    bytes[bytesCount++] = Convert.ToByte(int.Parse(chars[i + 1].ToString() + chars[i + 2].ToString(), System.Globalization.NumberStyles.HexNumber));
                    i += 2;
                }
                else
                {
                    bytes[bytesCount++] = Convert.ToByte(chars[i]);
                }
            }

            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytesCount);
        }

    }
}
