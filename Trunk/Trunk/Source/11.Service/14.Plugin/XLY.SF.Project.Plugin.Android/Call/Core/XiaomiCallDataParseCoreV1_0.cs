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
    /// 小米手机备份通话记录解析
    /// </summary>
    public class XiaomiCallDataParseCoreV1_0
    {
        /// <summary>
        /// calllog.store文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// miui_bak/_tmp_bak文件路径
        /// </summary>
        private string OtherDbPath { get; set; }

        /// <summary>
        /// 小米手机备份通话记录解析
        /// </summary>
        /// <param name="mainDbPath">calllog.store文件路径</param>
        /// <param name="bakFile">miui_bak/_tmp_bak文件路径</param>
        public XiaomiCallDataParseCoreV1_0(string mainDbPath,string bakFile)
        {
            MainDbPath = mainDbPath;
            OtherDbPath = bakFile;
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

            if (FileHelper.IsValid(OtherDbPath))
            {
                var list = FileParse(OtherDbPath);

                foreach (var item in list)
                {
                    datasource.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 小米通话记录备份文件解析
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<Call> FileParse(String path)
        {
            List<Call> list = new List<Call>();

            var tmp = File.ReadAllBytes(path);

            Call model = new Call();
            model.DataState = EnumDataState.Normal;
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i].ToString("x02").ToUpper() == "0A")
                {
                    if (tmp[i + 2].ToString("x02") == "12")
                    {
                        i = i + 4 + Int32.Parse(tmp[i + 3].ToString());
                    }
                    else if (tmp[i + 3].ToString("x02") == "12")
                    {
                        i = i + 5 + Int32.Parse(tmp[i + 4].ToString());
                    }
                }
                if (tmp[i].ToString("x02").ToUpper() == "1A")
                {
                    byte[] temp = new byte[Int32.Parse(tmp[i + 1].ToString())];
                    Array.Copy(tmp, i + 2, temp, 0, Int32.Parse(tmp[i + 1].ToString()));
                    i = i + 2 + Int32.Parse(tmp[i + 1].ToString());
                    model.Number = System.Text.Encoding.UTF8.GetString(temp.ToArray());
                }

                if (tmp[i].ToString("x02") == "20")
                {
                    i = i + 7;
                }
                if (tmp[i].ToString("x02") == "28")
                {
                    byte[] temp = null;
                    if (tmp[i + 2].ToString("x02") == "30")
                    {
                        temp = new byte[1];
                        Array.Copy(tmp, i + 1, temp, 0, 1);
                        i = i + 2;
                        model.DurationSecond = (int)(temp[0] & 0xFF);
                    }
                    else if (tmp[i + 3].ToString("x02") == "30")
                    {
                        temp = new byte[2];
                        Array.Copy(tmp, i + 1, temp, 0, 2);
                        i = i + 3;
                        model.DurationSecond = BytesToInt(temp);
                    }
                }

                if (tmp[i].ToString("x02") == "30")
                {
                    if (tmp[i + 1].ToString("x02") == "02")
                    {
                        model.Type = EnumCallType.CallOut;
                    }
                    else
                    {
                        model.Type = EnumCallType.CallIn;
                    }
                    i = i + 2;
                }

                if (tmp[i].ToString("x02") == "38")
                {

                    list.Add(model);
                    model = new Call();
                    model.DataState = EnumDataState.Normal;
                }
            }
            return list;
        }
        private int BytesToInt(byte[] ary)
        {
            int value;
            value = (int)((ary[0] & 0xFF)
                     | (ary[1] << 8) & 0xFF00);
            return value;
        }

    }
}
