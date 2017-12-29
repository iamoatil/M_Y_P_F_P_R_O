/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/12/14 13:48:10 
 * explain :  
 *
*****************************************************************************/

using SevenZip;
using System;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// 7Z解压辅助类
    /// </summary>
    public static class SevenZipHelper
    {
        static SevenZipHelper()
        {
            SevenZipBase.SetLibraryPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));
        }

        /// <summary>
        /// 解压文件
        /// 注意，这种解压方式无法解决长路径问题。如有需要，可以使用WinRARCSharp的UnRAR方法
        /// </summary>
        /// <param name="sourceFile">压缩文件路径</param>
        /// <param name="destPath">解压保存路径</param>
        public static void ExtractArchive(string sourceFile, string destPath)
        {
            try
            {
                using (var sz = new SevenZipExtractor(sourceFile))
                {
                    sz.ExtractArchive(destPath);
                }
            }
            catch (Exception ex)
            {
                Log4NetService.LoggerManagerSingle.Instance.Error(ex, $"解压文件{sourceFile}出错！");
            }
        }
    }
}
