/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/19 10:26:19 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS微信语音消息解析
    /// </summary>
    internal static class MessageToVoice
    {
        public static string MessageConvert(dynamic Message, string Location)
        {
            string filepath = Path.Combine(Location, string.Format("{0}.aud", Message.MesLocalID));

            try
            {
                if (!File.Exists(filepath))
                {
                    return string.Format("{0}.aud", Message.MesLocalID);
                }

                return AudioDecodeHelper.Decode(filepath);
            }
            catch
            {
                return string.Format("{0}.aud", Message.MesLocalID);
            }
        }
    }
}
