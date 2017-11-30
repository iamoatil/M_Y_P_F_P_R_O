using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataPreview.Decoder.TextDecoder.CstEncoding
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/27 15:33:37
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataPreview
{
    /// <summary>
    /// CstEncoding
    /// </summary>
    public class CstEncoding
    {
        private readonly IdentifyEncoding _identifyEncoding = new IdentifyEncoding();

        public string GetString(string encodingName, byte[] bytes, int count)
        {
            if (string.IsNullOrWhiteSpace(encodingName))
            {
                try
                {
                    string autoCheckName = _identifyEncoding.GetEncodingName(IdentifyEncoding.ToSByteArray(bytes));
                    string text = Encoding.GetEncoding(autoCheckName).GetString(bytes, 0, count);
                    return text;
                }
                catch (Exception)
                {
                    encodingName = "Hex";
                }
            }

            if (encodingName == "Hex")
            {
                string text = BytesToHexString(bytes, count);
                return text;
            }
            return string.Empty;
        }

        /// <summary>
        /// Byte数组转化为16进制字符串
        /// </summary>
        /// <returns></returns>
        private string BytesToHexString(byte[] bytes, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                if (i != 0
                    && i % 4 == 0)
                {
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append(Convert.ToString(bytes[i], 16).PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }
    }
}
