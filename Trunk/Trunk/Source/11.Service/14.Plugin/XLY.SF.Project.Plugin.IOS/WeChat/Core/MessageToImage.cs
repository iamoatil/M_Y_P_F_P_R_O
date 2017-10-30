/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/19 10:26:58 
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
    /// IOS微信图片消息解析
    /// </summary>
    internal static class MessageToImage
    {
        public static string MessageConvert(dynamic Message, string Location)
        {
            string filepath1 = Path.Combine(Location, string.Format("{0}.pic", Message.MesLocalID));
            string filepath2 = Path.Combine(Location, string.Format("{0}.pic_hd", Message.MesLocalID));
            string filepath3 = Path.Combine(Location, string.Format("{0}.pic_thum", Message.MesLocalID));

            FileInfo info = null;
            info = new FileInfo(filepath1);
            if (info.Exists)
            {
                return info.FullName;
            }

            info = new FileInfo(filepath2);
            if (info.Exists)
            {
                return info.FullName;
            }

            info = new FileInfo(filepath3);
            if (info.Exists)
            {
                return info.FullName;
            }

            return string.Format("{0}.pic", Message.MesLocalID);
        }
    }
}
