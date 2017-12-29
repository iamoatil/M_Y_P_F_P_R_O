using System.Collections;

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
        /// 数据总数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 删除数据总数
        /// </summary>
        int DeleteCount { get; }

        /// <summary>
        /// 获取当前页码的数据集，如果pageSize&lt;=0，则获取之后的全部数据
        /// </summary>
        IEnumerable GetView(int cursor = 0, int pageSize = -1);
    }
}
