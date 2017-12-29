/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 15:45:30 
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
    /// 小米备份联系人数据解析核心类
    /// </summary>
    internal class XiaomiContactsDataParseCoreV1_0
    {
        /// <summary>
        /// addressbook.store文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// miui_bak/_tmp_bak文件路径
        /// </summary>
        private string OtherDbPath { get; set; }

        /// <summary>
        /// 小米备份联系人数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">addressbook.store文件路径</param>
        /// <param name="bakFile">miui_bak/_tmp_bak文件路径</param>
        public XiaomiContactsDataParseCoreV1_0(string mainDbPath, string bakFile)
        {
            MainDbPath = mainDbPath;
            OtherDbPath = bakFile;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public void BuildData(ContactDataSource datasource)
        {
            if (FileHelper.IsValid(MainDbPath))
            {
                foreach (var item in FileParse(MainDbPath))
                {
                    datasource.Items.Add(item);
                }
            }

            if (FileHelper.IsValid(OtherDbPath))
            {
                foreach (var item in FileParse(OtherDbPath))
                {
                    datasource.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 小米联系人备份文件的解析
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<Contact> FileParse(String path)
        {
            List<Contact> list = new List<Contact>();

            var tmp = File.ReadAllBytes(path);
            Contact model = new Contact();
            model.DataState = EnumDataState.Normal;
            for (int i = 3; i < tmp.Length; i++)
            {
                if ((tmp[i].ToString("x02").ToUpper() == "0A") && ((tmp[i - 2].ToString("x02").ToUpper() == "2A") || (tmp[i - 3].ToString("x02").ToUpper() == "2A")))
                {
                    byte[] temp = new byte[Int32.Parse(tmp[i + 1].ToString())];
                    Array.Copy(tmp, i + 2, temp, 0, Int32.Parse(tmp[i + 1].ToString()));
                    i = i + Int32.Parse(tmp[i + 1].ToString()) + 2;
                    string str = System.Text.Encoding.UTF8.GetString(temp.ToArray());
                    if (!String.IsNullOrEmpty(model.Name))
                    {
                        list.Add(model);
                    }
                    model = new Contact();
                    model.DataState = EnumDataState.Normal;
                    model.Name = str;

                    if (tmp[i].ToString("x02").ToUpper() == "12")
                    {
                        if (tmp[i + 1].ToString("x02").ToUpper() != "0A")
                        {
                            temp = new byte[Int32.Parse(tmp[i + 1].ToString())];
                            Array.Copy(tmp, i + 2, temp, 0, Int32.Parse(tmp[i + 1].ToString()));
                            i = i + Int32.Parse(tmp[i + 1].ToString()) + 2;
                            str = System.Text.Encoding.UTF8.GetString(temp.ToArray());
                        }
                        else
                        {
                            i = i + 1;
                        }
                    }

                    if (tmp[i].ToString("x02").ToUpper() == "22")
                    {
                        temp = new byte[Int32.Parse(tmp[i + 1].ToString())];
                        Array.Copy(tmp, i + 2, temp, 0, Int32.Parse(tmp[i + 1].ToString()));
                        i = i + Int32.Parse(tmp[i + 1].ToString()) + 2;
                        str = System.Text.Encoding.UTF8.GetString(temp.ToArray());
                    }
                }
                if ((tmp[i].ToString("x02").ToUpper() == "0A") && (tmp[i - 2].ToString("x02").ToUpper() == "32"))
                {
                    byte[] temp = new byte[Int32.Parse(tmp[i + 1].ToString())];
                    Array.Copy(tmp, i + 2, temp, 0, Int32.Parse(tmp[i + 1].ToString()));
                    i = i + Int32.Parse(tmp[i + 1].ToString()) + 2;
                    string str = System.Text.Encoding.UTF8.GetString(temp.ToArray());
                    if (String.IsNullOrEmpty(model.Number))
                    {
                        model.Number = str;
                    }
                    else
                    {
                        model.Number = model.Number + ";" + str;
                    }
                }
            }

            return list;
        }

    }
}
