using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Services.LocalFileService
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/23 10:44:26
* ==============================================================================*/

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 本地文件/文件夹类型判断
    /// </summary>
    public class LocalFileService
    {
        /// <summary>
        /// 自动判断选择文件的系统类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static EnumOSType GetOSType(string fileName)
        {
            byte[] bytes = FileHelper.ReadFileHead(fileName, 4);
            if (bytes.SequenceEqual(new byte[] { 0, 0, 0, 0 }))
            {
                return EnumOSType.Android;
            }
            else if (fileName.EndsWith(".bbb")) // 黑莓备份：根据文件后缀名判断
            {
                return EnumOSType.BlackBerry;
            }
            else if (fileName.EndsWith(".db")) // 单个的db文件：根据文件后缀名判断
            {
                return EnumOSType.DBFile;
            }
            else if (bytes.SequenceEqual(new byte[] { 80, 75, 3, 4 }))
            {
                string strBytes = Encoding.Default.GetString(FileHelper.ReadFileHead(fileName, 200));
                
                if (strBytes.IndexOf(".bbb") != -1 && strBytes.IndexOf("Manifest.xml") != -1)       //黑莓自动备份：根据.bbb和Manifest.xml判定
                {
                    return EnumOSType.BlackBerry;
                }
                else if (strBytes.IndexOf("PalmDatabase.db3SQLite format 3") != -1)     //WebOS采集工具备份：根据PalmDatabase.db3SQLite format 3判定
                {
                    return EnumOSType.WebOS;
                }
                else
                {
                    return EnumOSType.IOS;
                }
            }
            else
            {
                var databytes = FileHelper.ReadFileHead(fileName, 0x202);
                if (databytes[0x200] == 0x1F && databytes[0x201] == 0x8B)   //YunOS
                {
                    return EnumOSType.Android;
                }
                else
                {
                    return EnumOSType.None;
                }
            }
        }
    }
}
