using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataFilter.Views
{
    /// <summary>
    /// 提供一套常用的简单过滤。
    /// </summary>
    public static class Filterable
    {
        #region Public

        /// <summary>
        /// 无条件的筛选。
        /// </summary>
        /// <typeparam name="TResult">筛选结果的类型。</typeparam>
        /// <param name="source">实现了 IFilterable 接口的类型实例。</param>
        /// <returns>筛选结果。</returns>
        public static IEnumerable<TResult> FilterByNone<TResult>(this IFilterable source)
        {
            return source.Provider.Query<TResult>(Expression.Constant(null));
        }

        /// <summary>
        /// 关键字筛选。
        /// </summary>
        /// <typeparam name="TResult">筛选结果的类型。</typeparam>
        /// <param name="source">实现了 IFilterable 接口的类型实例。</param>
        /// <param name="keyWords">属性名与值组成的键值对。</param>
        /// <returns>筛选结果。</returns>
        public static IEnumerable<TResult> FilterByKeywords<TResult>(this IFilterable source, KeyValuePair<String, String>[] keyWords, params String[] fields)
        {
            StringBuilder sb = new StringBuilder();
            if (keyWords != null && keyWords.Length != 0)
            {
                foreach (KeyValuePair<String, String> kv in keyWords)
                {
                    sb.AppendFormat("AND {0} LIKE '%{1}%' ", kv.Key, kv.Value);
                }
                sb = sb.Remove(0, 2);
            }
            return source.Provider.Query<TResult>(Expression.Constant(sb.ToString()));
        }

        /// <summary>
        /// 关键字筛选。
        /// </summary>
        /// <typeparam name="TResult">筛选结果的类型。</typeparam>
        /// <param name="source">实现了 IFilterable 接口的类型实例。</param>
        /// <param name="keyWords">属性名与值组成的键值对。</param>
        /// <returns>筛选结果。</returns>
        public static IEnumerable<TResult> FilterAllTextByKeywords<TResult>(this IFilterable source, String[] keyWords, params String[] fields)
        {
            StringBuilder sb = new StringBuilder();
            if (keyWords != null && keyWords.Length != 0)
            {
                foreach (String keyWord in keyWords)
                {
                    sb.AppendFormat("AND XLYJson LIKE '%{0}%' ", keyWord);
                }
                sb = sb.Remove(0, 2);
            }
            return source.Provider.Query<TResult>(Expression.Constant(sb.ToString()));
        }

        /// <summary>
        /// 正则筛选。
        /// </summary>
        /// <typeparam name="TResult">筛选结果的类型。</typeparam>
        /// <param name="source">实现了 IFilterable 接口的类型实例。</param>
        /// <param name="pattern">正则表达式。</param>
        /// <returns>筛选结果。</returns>
        public static IEnumerable<TResult> FilterByRegex<TResult>(this IFilterable source, KeyValuePair<String, String>[] pattern, params String[] fields)
        {
            StringBuilder sb = new StringBuilder();
            if (pattern != null && pattern.Length != 0)
            {
                foreach (KeyValuePair<String, String> kv in pattern)
                {
                    sb.AppendFormat("AND RegexMatch('${0}','${1}') ", kv.Key, kv.Value.Replace("{", "{{").Replace("}", "}}"));
                }
                sb = sb.Remove(0, 2);
            }
            return source.Provider.Query<TResult>(Expression.Constant(sb.ToString()));
        }

        /// <summary>
        /// 关键字筛选。
        /// </summary>
        /// <typeparam name="TResult">筛选结果的类型。</typeparam>
        /// <param name="source">实现了 IFilterable 接口的类型实例。</param>
        /// <param name="keyWords">属性名与值组成的键值对。</param>
        /// <returns>筛选结果。</returns>
        public static IEnumerable<TResult> FilterAllTextByRegex<TResult>(this IFilterable source, String[] patterns, params String[] fields)
        {
            StringBuilder sb = new StringBuilder();
            if (patterns != null && patterns.Length != 0)
            {
                foreach (String pattern in patterns)
                {
                    sb.AppendFormat("AND RegexMatch('$XLYJson','${0}') ", pattern.Replace("{", "{{").Replace("}", "}}"));
                }
                sb = sb.Remove(0, 2);
            }
            return source.Provider.Query<TResult>(Expression.Constant(sb.ToString()));
        }


        #endregion
    }
}
