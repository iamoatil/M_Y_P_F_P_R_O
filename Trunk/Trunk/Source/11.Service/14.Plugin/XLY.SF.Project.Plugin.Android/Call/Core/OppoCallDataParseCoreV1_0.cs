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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// OPPO手机备份通话记录解析
    /// </summary>
    public class OppoCallDataParseCoreV1_0
    {
        /// <summary>
        /// callrecord_backup.xml文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// OPPO手机备份通话记录解析
        /// </summary>
        /// <param name="mainDbPath">callrecord_backup.xml文件路径</param>
        public OppoCallDataParseCoreV1_0(string mainDbPath)
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

            XmlSerializer xml = new XmlSerializer(typeof(CallLog));
            using (FileStream fs = File.OpenRead(DealXml(MainDbPath)))
            {
                var lstCallDatas = xml.Deserialize(fs) as CallLog;
                foreach (var item in lstCallDatas.listDatas)
                {
                    int typeTmp;
                    long dateTmp;
                    int durationTmp;
                    // 号码过滤,验证号码长度
                    if (!DataParseHelper.ValidateNumber(item.number))
                        continue;

                    if (int.TryParse(item.type, out typeTmp) &&
                        long.TryParse(item.date, out dateTmp) &&
                        int.TryParse(item.duration, out durationTmp))
                    {
                        Call callTmp = new Call();
                        callTmp.Number = item.number;
                        callTmp.Name = item.name;
                        callTmp.Type = ConvertToEnumCallType(typeTmp, durationTmp > 0);
                        callTmp.DataState = EnumDataState.Normal;
                        callTmp.StartDate = new DateTime(1970, 1, 1).AddSeconds(dateTmp / 1000).AddHours(8);
                        callTmp.DurationSecond = durationTmp;

                        datasource.Items.Add(callTmp);
                    }
                }
                fs.Close();
            }

        }

        private string DealXml(string file)
        {
            string text = File.ReadAllText(file);
            if (!text.Contains("<CallLog>") && !text.Contains("</CallLog>"))
            {
                if (text.Contains("<callrecord>") && text.Contains("</callrecord>"))
                {
                    text = text.Replace("<callrecord>", "<CallLog>");
                    text = text.Replace("</callrecord>", "</CallLog>");
                }
            }

            File.WriteAllText(file, text);

            return file;
        }

        /// <summary>
        /// 转换通话类型
        /// </summary>
        /// <param name="typeBaseID">通话类型ID</param>
        /// <param name="hasDurationTime">是否有持续时间</param>
        /// <returns></returns>
        private EnumCallType ConvertToEnumCallType(int typeBaseID, bool hasDurationTime)
        {
            EnumCallType result = EnumCallType.None;
            if (Enum.IsDefined(typeof(EnumCallType), typeBaseID))
            {
                result = (EnumCallType)typeBaseID;
                if (result == EnumCallType.CallIn && !hasDurationTime)
                    result = EnumCallType.MissedCallIn;
                else if (result == EnumCallType.CallOut && !hasDurationTime)
                    result = EnumCallType.MissedCallOut;
            }
            return result;
        }

    }
}
