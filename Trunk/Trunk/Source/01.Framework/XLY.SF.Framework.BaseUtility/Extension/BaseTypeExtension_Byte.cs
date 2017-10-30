using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/14 13:47:55
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.BaseUtility
{
    public static partial class BaseTypeExtension
    {
        #region GetString： 把byte数组转换为系统默认编码(System.Text.Encoding.Default)类型的字符串
        /// <summary>
        /// 把byte数组转换为系统默认编码(System.Text.Encoding.Default)类型的字符串
        /// </summary>
        /// <param name="bytes">值</param>
        /// <returns></returns>
        public static string GetString(this byte[] bytes)
        {
            return bytes.GetString(System.Text.Encoding.Default);
        }
        #endregion

        #region GetString： 把byte数组转换为制定编码类型的字符串
        /// <summary>
        /// 把byte数组转换为制定编码类型的字符串
        /// </summary>
        /// <param name="bytes">值</param>
        /// <param name="encode">编码类型</param>
        /// <returns></returns>
        public static string GetString(this byte[] bytes, System.Text.Encoding encode)
        {
            if (bytes == null || bytes.Count() <= 0)
            {
                return string.Empty;
            }
            return encode.GetString(bytes);
        }
        #endregion
        
        #region 2进制

        /// <summary>
        /// 转换为2进制字符串
        /// </summary>
        public static string ToBinary(this byte value)
        {
            return Convert.ToString(value, 2);
        }

        #endregion
        
        #region 16进制

        /// <summary>
        /// 转换为16进制字符串（两位）
        /// </summary>
        public static string ToHex(this byte value)
        {
            return value.ToString("X2");
        }

        #endregion

        /// <summary>
        /// 将字节数组转换为16进制的字符串，如"AB1378"
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] array)
        {
            StringBuilder sb = new StringBuilder(array.Length * 2);
            foreach(byte b in array)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将16进制的字符串转为字节数组，如"AB1378"
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string hexString)
        {
            byte[] bs = new byte[hexString.Length / 2];
            for (int i = 0; i < bs.Length; i++)
            {
                bs[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bs;
        }
    }
}
