using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace XLY.SF.Project.ScriptEngine
{
    /// <summary>
    /// 数据转换
    /// </summary>
    public class Convert
    {
        /// <summary>
        /// 转换为长整型
        /// </summary>
        public string ToLong(object value)
        {
            return value.ToSafeString().ToSafeInt64().ToString();
        }

        /// <summary>
        /// 转换为整数类型
        /// </summary>
        public string ToInt(object value)
        {
            return value.ToSafeString().ToSafeInt().ToString();
        }

        /// <summary>
        /// 转换为浮点数（带小数的数字）
        /// </summary>
        public string ToDouble(object value)
        {
            return value.ToSafeString().ToSafeDouble().ToString();
        }

        /// <summary>
        /// 转换为安全的字符串值
        /// </summary>
        public string ToString(object value)
        {
            return value.ToSafeString();
        }

        /// <summary>
        /// 转换为数据类型的枚举值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ToDataState(object value)
        {
            var e = DynamicConvert.ToEnumByValue(value, EnumDataState.Normal);
            return e.ToSafeString();
        }

        public enum EnumDataState
        {

            /// <summary>
            /// 未知
            /// </summary>
            None = 0,

            /// <summary>
            /// 正常
            /// </summary>
            Normal = 2,

            /// <summary>
            /// 已删除
            /// </summary>
            Deleted = 1,

            /// <summary>
            /// 碎片
            /// </summary>
            Fragment = 4,
        }

        /// <summary>
        /// xml内容转换为json对象
        /// </summary>
        public string XMLToJSON(object value)
        {
            try
            {
                XmlDocument n = new XmlDocument();
                n.LoadXml(value.ToSafeString());
                return Newtonsoft.Json.JsonConvert.SerializeXmlNode(n);
            }
            catch (Exception ex)
            {
                Console.WriteLine("xml to json occour errors: " + ex.AllMessage());
                return string.Empty;
            }
        }

        #region Encode（字符转码）

        /// <summary>
        /// 字符转码，sourcecode表示原理的编码，targetcode表示转换的目标编码
        /// 基本编码：UnicodeFFFE, UTF-8, UTF-7, UTF-16, UTF-32, UTF-16BE, UTF-16LE, UTF-32BE, ASCII, ISO-8859-1, GB2312, BIG5, GBK, GB18030
        /// 其他编码：quoted-printable，BASE64
        /// </summary>
        public string Encode(object value, string sourcecode, string targetcode)
        {
            try
            {
                sourcecode = sourcecode.ToUpper();
                Encoding tc = Encoding.GetEncoding(targetcode);
                var str = value.ToSafeString();
                switch (sourcecode)
                {
                    case "QUOTED-PRINTABLE":
                        return this.QuotedPrintableEncode(str, tc);
                    case "BASE64":
                        return this.BASE64Encode(str, tc);
                    default:
                        Encoding sc = Encoding.GetEncoding(sourcecode);
                        return str.Encode(sc, tc);
                }
            }
            catch
            {
                return value.ToSafeString();
            }
        }

        private string BASE64Encode(string value, Encoding target)
        {
            var data = System.Convert.FromBase64String(value);
            return data.GetString(target);
        }

        private string QuotedPrintableEncode(string value, Encoding target)// QP编码
        {
            ArrayList vBuffer = new ArrayList();

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '=')
                {
                    i++;
                    if (value[i] != '\r')
                    {
                        byte vByte;
                        if (byte.TryParse(value.Substring(i, 2),
                            NumberStyles.HexNumber, null, out vByte))
                            vBuffer.Add(vByte);
                    }
                    i++;
                }
                else if (value[i] != '\n') vBuffer.Add((byte)value[i]);
            }
            return target.GetString((byte[])vBuffer.ToArray(typeof(byte)));

        }

        #endregion

        #region UrlDecode（URL解码）

        /// <summary>
        /// URL解码
        /// </summary>
        public string UrlDecode(object value)
        {
            return value.ToSafeString().UrlDecode();
        }

        /// <summary>
        /// URL编码
        /// </summary>
        public string UrlEncode(object value)
        {
            return value.ToSafeString().UrlEncode();
        }

        #endregion

        /// <summary>
        /// 转换长整型的linux时间数字值为时间格式
        /// </summary>
        public string LinuxToDateTime(object value)
        {
            var dt = DynamicConvert.ToSafeDateTime(value);
            return dt.IsValid() ? dt.ToDateTimeString() : string.Empty;
        }

        public string ToSinaDateTime(object value)
        {
            if (value != null)
            {
                string time = value.ToSafeString();
                if (time.Length >= 30)
                {
                    string newt = time.Substring(8, 2) + " " + time.Substring(4, 3) + " " + time.Substring(26, 4) + " " + time.Substring(11, 8);
                    var dt = DateTime.Parse(newt);
                    return dt.IsValid() ? dt.ToString("yyyy-MM-dd hh:mm:ss") : "";
                }
            }
            return "";
        }

        /// <summary>
        /// 转换长整型的GoogleChrome时间数字值为时间格式
        /// </summary>
        public string GoogleChromeToDateTime(object value)
        {
            var dt = DynamicConvert.ToSafeDateTimeForGoogleChrome(value);
            return dt.IsValid() ? dt.ToDateTimeString() : string.Empty;
        }

        /// <summary>
        /// 转换字符串为时间格式，fromat为格式，如yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string ToDateTime(object value, string format)
        {
            return value.ToSafeString().ToDateTime(format).ToDateTimeString();
        }

        /// <summary>
        /// 转换字符串为时间格式
        /// </summary>
        public string ToDateTime(object value, string format, string culture)
        {
            return value.ToSafeString().ToDateTime(format, culture).ToDateTimeString();
        }

        /// <summary>
        /// 转换时间戳到当前时间
        /// </summary>
        /// <param name="year">起始年份</param>
        /// <param name="month">起始月份</param>
        /// <param name="day">起始日</param>
        /// <param name="seconds">时间戳(单位:秒)</param>
        /// <returns></returns>
        public string ToDateTime(int year, int month, int day, double seconds)
        {
            return new DateTime(year, month, day, 0, 0, 0, 0).AddSeconds(Math.Round(seconds)).ToLocalTime().ToString();
        }

        /// <summary>
        /// 计算字符串Md5值
        /// </summary>
        /// <param name="content">需要计算Md5字符串</param>
        /// <returns>返回字符串Md5值</returns>
        public string CalculateMd5(object content)
        {
            return System.Utility.Helper.Cryptography.MD5Encrypt(content.ToSafeString());
        }
    }
}
