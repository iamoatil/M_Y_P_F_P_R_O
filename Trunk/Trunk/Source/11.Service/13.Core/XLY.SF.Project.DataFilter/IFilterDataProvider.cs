using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataFilter
{
    /// <summary>
    /// 表示一个数据提供器。
    /// </summary>
    public interface IFilterDataProvider
    {
        /// <summary>
        /// 查询数量。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <returns>集合的大小。</returns>
        Int32 GetCount(Expression expression);

        /// <summary>
        /// 查询。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <returns>数据集合。</returns>
        IEnumerable<T> Query<T>(Expression expression);
    }
}
