using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.IPage
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/12/8 14:32:45
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 支持分页读取的数据源集合
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// 分页大小
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 当前数据集的起始位置
        /// </summary>
        int Cursor { get; set; }
        /// <summary>
        /// 数据总数
        /// </summary>
        int Total { get; }
        /// <summary>
        /// 读取下一页数据
        /// </summary>
        /// <returns></returns>
        bool NextPage();
        /// <summary>
        /// 读取上一页数据
        /// </summary>
        /// <returns></returns>
        bool PrePage();
        /// <summary>
        /// 获取当前的数据集
        /// </summary>
        IEnumerable View { get; }
    }
}
