using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.DataFilter;

/* ==============================================================================
* Description：Helper  
* Author     ：litao
* Create Date：2017/11/29 11:00:29
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    public static class Helper
    {
        public static IEnumerable<TResult> FilterByCmd<TResult>(this IFilterable source,string cmd)
        {
            return source.Provider.Query<TResult>(Expression.Constant(cmd));
        }
    }
}
