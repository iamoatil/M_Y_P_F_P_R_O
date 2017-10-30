/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/19 10:27:13 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS微信视频消息解析
    /// </summary>
    internal static class MessageToVideo
    {
        public static string MessageConvert(dynamic Message, string Location)
        {
            string fileName = string.Format("{0}.mp4", Message.MesLocalID);
            string filepath = Path.Combine(Location, fileName);

            var info = new FileInfo(filepath);
            if (!info.Exists)
            {
                return fileName;
            }
            else
            {
                return info.FullName;
            }
        }
    }
}
