using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.ScriptEngine.Helper.BaseUtility
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/12/15 15:53:29
* ==============================================================================*/

namespace XLY.SF.Project.ScriptEngine
{
    /// <summary>
    /// BaseUtility
    /// </summary>
    public static class BaseUtility
    {
        public static bool IsValid(string file)
        {
            if (String.IsNullOrEmpty(file))
                return false;
            if (!System.IO.File.Exists(file))
                return false;
            FileInfo info = new FileInfo(file);
            if (info.Length <= 0)
                return false;
            return true;
        }
    }

    /// <summary>
    /// 字符编码方式
    /// </summary>
    public enum EnumEncodingType
    {
        /// <summary>
        /// UTF_8
        /// </summary>
        [Description("utf-8")]
        UTF_8 = 0x1,

        /// <summary>
        /// GB2312
        /// </summary>
        [Description("gb2312")]
        GB2312 = 0x2,

        /// <summary>
        /// GBK
        /// </summary>
        [Description("GBK")]
        GBK = 3,

        /// <summary>
        /// GB18030
        /// </summary>
        [Description("GB18030")]
        GB18030 = 4
    }
}
