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
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 小米手机备份短信解析
    /// </summary>
    public class XiaomiSmsDataParseCoreV1_0
    {
        /// <summary>
        /// miui_bak/_tmp_bak文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// sms.store文件路径
        /// </summary>
        private string ContactDbPath { get; set; }

        /// <summary>
        /// 小米手机备份短信解析
        /// </summary>
        /// <param name="mainDbPath">miui_bak/_tmp_bak文件路径</param>
        /// <param name="contactDbPath">sms.store文件路径</param>
        public XiaomiSmsDataParseCoreV1_0(string mainDbPath, string contactDbPath)
        {
            MainDbPath = mainDbPath;
            ContactDbPath = contactDbPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public void BuildData(CallDataSource datasource)
        {
            if (FileHelper.IsValid(MainDbPath))
            {
                var list = FileParse(MainDbPath);
                foreach (var item in list)
                {
                    datasource.Items.Add(item);
                }
            }

            if (FileHelper.IsValid(ContactDbPath))
            {
                var list = FileParse(ContactDbPath);
                foreach (var item in list)
                {
                    datasource.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 小米短信备份文件解析
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<SMS> FileParse(String path)
        {
            List<SMS> list = new List<SMS>();
            SMS model = new SMS();
            model.DataState = EnumDataState.Normal;

            var tmp = File.ReadAllBytes(path);
            int msgLength = 0;
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i].ToString("x02").ToUpper() == "0A")
                {
                    if (tmp[i + 3].ToString("x02").ToUpper() == "12")
                    {
                        msgLength = 2;
                        i = i + 3;
                    }
                    else if (tmp[i + 2].ToString("x02").ToUpper() == "12")
                    {
                        msgLength = 1;
                        i = i + 2;
                    }
                    else
                    {
                        continue;
                    }
                    if (tmp[i].ToString("x02").ToUpper() == "12")
                    {
                        i = i + 2 + Int32.Parse(tmp[i + 1].ToString("x02"));
                    }
                    if (tmp[i].ToString("x02").ToUpper() == "18")
                    {
                        if (tmp[i + 1].ToString("x02") == "01")
                        {
                            model.SmsState = EnumSMSState.ReceiveSMS;
                        }
                        else
                        {
                            model.SmsState = EnumSMSState.SendSMS;
                        }
                        i = i + 2;
                    }
                    if (tmp[i].ToString("x02").ToUpper() == "22")
                    {
                        byte[] temp = new byte[Int32.Parse(tmp[i + 1].ToString())];
                        CopyData(tmp, i + 2, temp, 0, Int32.Parse(tmp[i + 1].ToString()));
                        model.Number = System.Text.Encoding.UTF8.GetString(temp.ToArray());
                        i = i + 2 + Int32.Parse(tmp[i + 1].ToString());
                    }
                }
                if (tmp[i].ToString("x02").ToUpper() == "32")
                {
                    if (msgLength == 1)
                    {
                        byte[] temp = new byte[Int32.Parse(tmp[i + 1].ToString())];
                        CopyData(tmp, i + 2, temp, 0, Int32.Parse(tmp[i + 1].ToString()));
                        model.Content = System.Text.Encoding.UTF8.GetString(temp.ToArray());
                        i = i + 2 + Int32.Parse(tmp[i + 1].ToString());
                    }
                    else
                    {
                        String high;
                        String low;
                        if (IsOdd(Int32.Parse(tmp[i + 2].ToString())))
                        {
                            high = (tmp[i + 2] >> 1).ToString("x02");
                            low = tmp[i + 1].ToString("x02");
                        }
                        else
                        {
                            high = (tmp[i + 2] >> 1).ToString("x02");
                            low = (tmp[i + 1] ^ 128).ToString("x02");
                        }
                        int length = Convert.ToInt32(high + low, 16);
                        if ((i + 3 + length < tmp.Length) && (tmp[i + 3 + length].ToString("x02").ToUpper() == "38"))
                        {
                            byte[] temp = new byte[length];
                            CopyData(tmp, i + 3, temp, 0, length);
                            model.Content = System.Text.Encoding.UTF8.GetString(temp.ToArray());
                            i = i + 3 + length;
                        }
                        else
                        {
                            byte[] temp = new byte[Int32.Parse(tmp[i + 1].ToString())];
                            CopyData(tmp, i + 2, temp, 0, Int32.Parse(tmp[i + 1].ToString()));
                            model.Content = System.Text.Encoding.UTF8.GetString(temp.ToArray());
                            i = i + 2 + Int32.Parse(tmp[i + 1].ToString());
                        }
                    }
                }
                if (i >= tmp.Length)
                {
                    break;
                }
                if (tmp[i].ToString("x02").ToUpper() == "38")
                {
                    i = i + 7;
                }

                //if ((tmp[i].ToString("x02").ToUpper() == "62") && (list.Count() > 1))
                //{
                //    byte[] temp = new byte[Int32.Parse(tmp[i + 1].ToString())];
                //    CopyData(tmp, i + 2, temp, 0, Int32.Parse(tmp[i + 1].ToString()));
                //    i = i + 3 + Int32.Parse(tmp[i + 1].ToString());
                //}

                if (i + 1 >= tmp.Length)
                {
                    list.Add(model);
                    model = new SMS();
                    model.DataState = EnumDataState.Normal;
                    break;
                }
                if ((tmp[i].ToString("x02").ToUpper() == "00") && (tmp[i + 1].ToString("x02").ToUpper() == "0A"))
                {
                    list.Add(model);
                    model = new SMS();
                    model.DataState = EnumDataState.Normal;
                }
            }

            return list;
        }

        private void CopyData(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
        {
            if (sourceIndex + length > sourceArray.Length)
            {
                Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, sourceArray.Length - sourceIndex);
            }
            else
            {
                Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
            }
        }

        /// <summary>
        /// 是否为偶数
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private Boolean IsOdd(int number)
        {
            if (number % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
