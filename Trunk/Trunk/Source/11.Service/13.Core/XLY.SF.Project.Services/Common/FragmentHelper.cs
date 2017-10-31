/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 11:00:57 
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
    public static class FragmentHelper
    {
        /// <summary>
        /// 乱码正则匹配(非UTF8)
        /// </summary>
        private static Regex _RegUtf8 = new Regex(@"[^\u0000-\uFFFF]", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        //private static Regex _RegUtf8O = new Regex(@"[\uD800-\uDFFF]", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// 空白字符正则匹配表达式
        /// </summary>
        private static Regex _RegEmptyString = new Regex(@"\s", RegexOptions.Compiled);

        /// <summary>
        /// 验证是否为空白字符
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsEmptyString(string content)
        {
            return _RegEmptyString.Replace(content, string.Empty).IsInvalid();
        }

        /// <summary>
        /// 是否有效碎片数据
        /// </summary>
        /// <returns>true:有效碎片 false:无效碎片</returns>
        public static bool IsValidFragment(string data)
        {
            if (data.IsInvalid())
            {
                return false;
            }

            return _RegUtf8.IsMatch(data);
        }

        /// <summary>
        /// 剔除无效的碎片数据(乱码)
        /// </summary>
        /// <param name="data"></param>
        /// <returns>剔除乱码后的字符串</returns>
        public static string RemoveNullityDataNew(string data)
        {
            return _RegUtf8.Replace(data, string.Empty);
            //return _RegUtf8O.Replace(_RegUtf8.Replace(data, string.Empty), string.Empty);
        }

        /// <summary>
        /// 判断该数据对象是否无效碎片数据.
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <returns>true:无效碎片,false:有效碎片</returns>
        public static bool IsValidFragment(dynamic data)
        {
            return IsValidFragment(data, 0);
        }

        /// <summary>
        /// 判断该数据对象是否无效碎片数据，通过传入的无效条件值.
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="nullityNum">无效标准值 例如：如果值为1则表示如果碎片数据中无效数据大于1则表示该数据对象为无效碎片</param>
        /// <returns>true:无效碎片,false:有效碎片</returns>
        public static bool IsValidFragment(dynamic data, int nullityNum)
        {
            var dataSource = data as DynamicEx;
            var membersCount = dataSource.Members.Count;
            if (membersCount == 0)
                return true;
            var nullityCount = dataSource.Members.Count(dInfo => !IsValidFragment(dInfo.Value.ToSafeString()));
            // 如果无效碎片数据量大于允许存在的数据量则表示该数据无效
            return nullityCount > nullityNum;
        }
    }

}
