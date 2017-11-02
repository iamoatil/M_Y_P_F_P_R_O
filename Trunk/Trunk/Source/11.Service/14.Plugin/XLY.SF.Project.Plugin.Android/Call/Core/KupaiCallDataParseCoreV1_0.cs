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
using System.Xml;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 酷派手机备份通话记录解析
    /// </summary>
    public class KupaiCallDataParseCoreV1_0
    {
        /// <summary>
        /// calls.xml文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// 酷派手机备份通话记录解析
        /// </summary>
        /// <param name="mainDbPath">calls.xml文件路径</param>
        public KupaiCallDataParseCoreV1_0(string mainDbPath)
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

            XmlDocument doc = new XmlDocument();
            doc.Load(MainDbPath);

            foreach (XmlNode node in doc.SelectSingleNode("calls_parent").SelectNodes("calls_child"))
            {
                Call callTmp = new Call();
                callTmp.DataState = EnumDataState.Normal;
                callTmp.Number = GetXmlNodeAttributeValue(node, "number");
                callTmp.Name = GetXmlNodeAttributeValue(node, "name");
                callTmp.DurationSecond = int.Parse(GetXmlNodeAttributeValue(node, "duration"));

                string date = GetXmlNodeAttributeValue(node, "date");
                callTmp.StartDate = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(UInt64.Parse(date) / 1000).AddHours(8);

                switch (GetXmlNodeAttributeValue(node, "type"))
                {
                    case "1"://呼入
                        callTmp.Type = EnumCallType.CallIn;
                        break;
                    case "2"://呼出
                        if (callTmp.DurationSecond == 0)
                        {
                            callTmp.Type = EnumCallType.MissedCallOut;
                        }
                        else
                        {
                            callTmp.Type = EnumCallType.CallOut;
                        }
                        break;
                    case "3"://未接
                        callTmp.Type = EnumCallType.MissedCallIn;
                        break;
                    default:
                        callTmp.Type = EnumCallType.None;
                        break;
                }

                datasource.Items.Add(callTmp);
            }
        }

        private static string GetXmlNodeAttributeValue(XmlNode node, string key)
        {
            try
            {
                return node.Attributes[key].Value;
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
