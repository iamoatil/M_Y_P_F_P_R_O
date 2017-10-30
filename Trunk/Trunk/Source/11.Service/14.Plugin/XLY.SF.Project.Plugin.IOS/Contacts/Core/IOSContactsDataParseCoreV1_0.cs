/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 9:55:00 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS联系人解析
    /// </summary>
    public class IOSContactsDataParseCoreV1_0
    {
        /// <summary>
        /// AddressBook.sqlitedb文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// IOS联系人数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">AddressBook.sqlitedb文件路径</param>
        public IOSContactsDataParseCoreV1_0(string mainDbPath)
        {
            MainDbPath = mainDbPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <returns></returns>
        public void BuildTree()
        {

        }
    }
}
