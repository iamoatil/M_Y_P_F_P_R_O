using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.ProxyService.LocalFileService
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/23 11:29:49
* ==============================================================================*/

namespace XLY.SF.Project.ProxyService
{
    /// <summary>
    /// 本地文件/文件夹类型判断
    /// </summary>
    public class LocalFileProxy
    {
        /// <summary>
        /// 自动判断选择文件的系统类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public EnumOSType GetOSType(string fileName)
        {
            return XLY.SF.Project.Services.LocalFileService.GetOSType(fileName);
        }
    }
}
