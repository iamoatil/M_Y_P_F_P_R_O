using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataReport.BcpConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/9/30 10:55:34
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataReport
{
    /// <summary>
    /// BCP导出时的自定义转换方法
    /// </summary>
    public class BcpConverter
    {
        public static BcpConverter Instance => SingleWrapperHelper<BcpConverter>.Instance;
        /// <summary>
        /// 正则匹配形如"张三(1234)"
        /// </summary>
        private Regex _rgCName = new Regex(@"\(.*?\)");
        private Dictionary<string, Regex> _dicReg = new Dictionary<string, Regex>();
        private object TryConverter(object value, Func<object, object> fun)
        {
            try
            {
                return fun(value);
            }
            catch (Exception)
            {
                return value;
            }
        }

        /// <summary>
        /// 数据状态转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyDataState(object value, object[] args)
        {
            return TryConverter(value, v =>
            {
                if (v.ToSafeString() == "删除") return 1;
                if (v.ToSafeString() == "Deleted") return 1;
                if (v.ToSafeString() == "正常") return 0;
                if (v.ToSafeString() == "Normal") return 0;
                return 0;
            });
        }

        /// <summary>
        /// 正则匹配形如"张三(1234)"，将匹配出“张三”
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyName(object value, object[] args)
        {
            string s = value.ToSafeString();
            if (!s.Contains("(") && !s.Contains(")"))
            {
                return "";
            }
            return TryConverter(value, v =>
            {
                return _rgCName.Replace(s, "");
            });
        }

        /// <summary>
        /// 正则匹配形如"张三(1234)"，将匹配出“1234”
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyID(object value, object[] args)
        {
            string s = value.ToSafeString();
            if (!s.Contains("(") && !s.Contains(")"))
            {
                return s;
            }
            return TryConverter(value, v =>
            {
                return _rgCName.Match(s).Value.Trim('(', ')');
            });
        }

        /// <summary>
        /// 判断动作类型，01接收方、02发送方、99其他
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyAction(object value, object[] args)
        {
            if (args.IsValid() && args[0] != null)
            {
                if (args[0].ToSafeString().Contains(value.ToSafeString()))
                {
                    return "02";
                }
                else
                {
                    return "01";
                }
            }
            return "99";
        }

        /// <summary>
        /// 判断短信动作类型，01接收方、02发送方、99其他
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlySMSAction(object value, object[] args)
        {
            if (value.ToSafeString() == "接收")
            {
                return "01";
            }
            else if (value.ToSafeString() == "发送")
            {
                return "02";
            }
            else
            {
                return "99";
            }
        }

        /// <summary>
        /// 判断短信查看状态，0未读，1已读，9其它
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyViewStatus(object value, object[] args)
        {
            if (value.ToSafeString() == "已读")
            {
                return "1";
            }
            else if (value.ToSafeString() == "未读")
            {
                return "0";
            }
            else
            {
                return "9";
            }
        }

        /// <summary>
        /// 判断通话状态，0未接、1接通、9其他
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyCallStatus(object value, object[] args)
        {
            return value.ToSafeString().Contains("未接") ? "0" : "1";
        }

        /// <summary>
        /// 判断通话动作类型，01接收方、02发送方、99其他
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyCallAction(object value, object[] args)
        {
            if (value.ToSafeString().Contains("呼出"))
            {
                return "02";
            }
            else if (value.ToSafeString().Contains("未知"))
            {
                return "99";
            }
            else
            {
                return "01";
            }
        }

        /// <summary>
        /// 判断邮件查看状态，0未读，1已读，9其它
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object XlyMailViewStatus(object value, object[] args)
        {
            return this.TryConverter(value, v =>
            {
                if (value.ToSafeString() == "未读")
                {
                    return "0";
                }
                else if (value.ToSafeString() == "已读")
                {
                    return "1";
                }
                else
                {
                    return "9";
                }
            });
        }

        /// <summary>
        /// 将时间转换为UTC时间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object XlyUTC(object value, object[] args)
        {
            DateTime? time = value.ToSafeString().ToSafeDateTime();
            if (time != null)
            {
                double intResult = 0;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                intResult = ((DateTime)time - startTime).TotalSeconds;
                return intResult;
            }
            return 0;
        }

        /// <summary>
        /// 将时间转换为秒数，例如“1分14秒”转换为“74"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object XlySecond(object value, object[] args)
        {
            return TryConverter(value, v =>
            {
                string content = value.ToSafeString();
                string reg = "";
                if (content.Contains("时"))
                {
                    reg += "(?<时>.*?时)";
                }
                if (content.Contains("分"))
                {
                    reg += "(?<分>.*?分)";
                }
                if (content.Contains("秒"))
                {
                    reg += "(?<秒>.*?秒)";
                }
                int h = 0, m = 0, s = 0;
                if (reg.IsValid())
                {
                    Regex rg1;
                    if (_dicReg.ContainsKey(reg))
                    {
                        rg1 = _dicReg[reg];
                    }
                    else
                    {
                        rg1 = new Regex(reg); //(\w+)时(\w+)分(\w+)秒
                        _dicReg[reg] = rg1;
                    }

                    var m1 = rg1.Match(content);
                    if (m1.Success)
                    {
                        h = !m1.Groups["时"].Success ? 0 : m1.Groups["时"].Value.Replace("时", "").ToSafeInt();
                        m = !m1.Groups["分"].Success ? 0 : m1.Groups["分"].Value.Replace("分", "").ToSafeInt();
                        s = !m1.Groups["秒"].Success ? 0 : m1.Groups["秒"].Value.Replace("秒", "").ToSafeInt();
                    }
                }
                else   //如果没有包含时分秒等文字，默认表示为秒
                {
                    s = value.ToSafeString().ToSafeInt();
                }
                return h * 3600 + m * 60 + s;
            });
        }

        public object XlyCntMember(object value, object[] args)
        {
            string s = value.ToSafeString();
            return TryConverter(value, v =>
            {
                return s.Split(';').Count() - 1;
            });
        }
    }
}
