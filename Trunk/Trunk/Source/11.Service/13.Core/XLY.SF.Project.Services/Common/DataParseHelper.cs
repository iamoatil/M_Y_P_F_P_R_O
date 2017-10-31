/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 11:10:17 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Services
{
    public static class DataParseHelper
    {
        /// <summary>
        /// 检测字符串第一位是否为字符的正则表达式
        /// </summary>
        public static Regex RegChar = new Regex("^[a-zA-Z]", RegexOptions.Compiled);

        /// <summary>
        /// 检测字符串中是否包含连续的3-11个数字，用作电话号码检测
        /// </summary>
        public static Regex RegNumber = new Regex("^[+-]?[0-9]{3,}", RegexOptions.Compiled);

        /// <summary>
        /// 电话号码匹配正则表达式
        /// </summary>
        public static Regex RegMatchPhone = new Regex("\\+?\\d{3,}(?:[- ]\\d{4,})*|\\(0\\d{2,3}\\)[- ]?\\d{7,8}", RegexOptions.Compiled);

        /// <summary>
        /// 把数据库中取得的电话号码转换为标准格式的电话号码格式。
        /// </summary>
        /// <param name="oriNumber">原来的号码字符串。</param>
        /// <returns>返回处理后的电话号码。</returns>
        public static string NumberToStu(string oriNumber)
        {
            oriNumber = RegMatchPhone.Match(oriNumber.Replace("-", string.Empty)).Value;

            if (oriNumber.StartsWith("+86"))
            {
                oriNumber = oriNumber.TrimStart("+86");
            }
            else if (oriNumber.StartsWith("86"))
            {
                oriNumber = oriNumber.TrimStart("86");
            }
            else if (oriNumber.StartsWith("17951"))
            {
                oriNumber = oriNumber.TrimStart("17951");
            }
            else if (oriNumber.StartsWith("12593"))
            {
                oriNumber = oriNumber.TrimStart("12593");
            }

            return oriNumber.Replace("-", string.Empty);
        }

        /// <summary>
        /// 删除状态的电话号码，去掉第一位字母字符（如果有）
        /// </summary>
        /// <param name="number">源号码字符串</param>
        /// <param name="dataType">数据状态</param>
        /// <returns>返回处理后的电话号码字符串</returns>
        public static string RemoveFirstCharForNumber(string number, dynamic dataType)
        {
            if (DynamicConvert.ToSafeInt(dataType) == 1 && RegChar.IsMatch(number))
            {
                return number.Substring(1);
            }
            else
            {
                return number;
            }
        }

        /// <summary>
        /// 验证电话号码的合法性，合法返回True
        /// </summary>
        /// <param name="number">电话号码</param>
        /// <returns>return true or false</returns>
        public static bool ValidateNumber(string number)
        {
            return RegNumber.IsMatch(number);
        }
    }
}
