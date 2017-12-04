using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.DataFilter;
using XLY.SF.Project.DataFilter.Views;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Domains
{
    public class AggregationFilterView<TResult> : FilterView<TResult>
        where TResult : AbstractDataItem
    {
        #region Fields

        private String[] _dateTimeColumnsName;

        private Int32 _cursor;

        private Boolean _isLocked;

        #endregion

        #region Constructors

        public AggregationFilterView(IFilterable source, string key)
            : base(source)
        {
            Key = key;
        }

        #endregion

        #region Properties

        #region Args

        private FilterArgs[] _args;

        public FilterArgs[] Args
        {
            get { return _args; }
            set
            {
                CheckValidation();
                _args = value;
            }
        }

        #endregion

        #region PageSize

        private Int32 _pageSize = 50;

        public Int32 PageSize
        {
            get { return _pageSize; }
            set
            {
                CheckValidation();
                if (value <= 0) throw new ArgumentException("Must be positive integer.");
                _pageSize = value;
            }
        }

        #endregion

        public Int32 PageCount { get; private set; }

        /// <summary>
        /// 指示该行数据属于哪个节点
        /// </summary>
        public String Key { get; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 开始一次新的过滤。 执行该方法后，可以调用NextPage获取下一页的视图数据。
        /// </summary>
        public override void Initialize()
        {
            _isLocked = true;
            Expression = CreateExpression(false, Args ?? throw new ArgumentNullException("Args"));
            base.Initialize();
            PageCount = (Count + PageSize - 1) / PageSize;
        }

        /// <summary>
        /// 执行Filter之后，执行该方法可以获取下一页的视图数据。
        /// </summary>
        /// <returns>如果存在下一页数据就返回true；否则返回false。</returns>
        public virtual Boolean NextPage()
        {
            if (!_isLocked) return false;
            Expression = CreateExpression(true, Args);
            base.Filter();
            if (_cursor >= Count)
                return false;
            _cursor += PageSize;
            return true;
        }

        /// <summary>
        /// 执行Filter之后，执行该方法可以获取下前一页的视图数据。
        /// </summary>
        /// <returns>如果存在前一页数据就返回true；否则返回false。</returns>
        public virtual Boolean PreviousPage()
        {
            if (!_isLocked) return false;
            if (Count == 0) return false;
            _cursor -= PageSize;
            Expression = CreateExpression(true, Args);
            base.Filter();
            if (_cursor <= 0)
                return false;
            return true;
        }

        /// <summary>
        /// 重置过滤器。若当前正在执行过滤，在开始下一次新的过滤之前，需要执行该方法。
        /// </summary>
        public void Reset()
        {
            _cursor = 0;
            PageCount = 0;
            _isLocked = false;
        }

        /// <summary>
        /// 重置光标
        /// </summary>
        public void ResetCursor()
        {
            _cursor = 0;
        }

        #endregion

        #region Private

        private Expression CreateExpression(Boolean paging, params FilterArgs[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("a.XLYKey = '{0}' ", Key);
            if (args.Length == 0)       //全部查询
            {
                if (paging)
                {
                    sb.AppendFormat("LIMIT {0} OFFSET {1} ", PageSize, _cursor);
                }
                return Expression.Constant(sb.ToString());
            }
            bool isAttach = false;
            bool isUnion = false;
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case FilterByStringContainsArgs keywordArg:
                        sb.AppendFormat("AND a.XLYJson LIKE '%{0}%' ", keywordArg.PatternText.Replace("'","''"));
                        break;
                    case FilterByRegexArgs regexArg:
                        String temp = regexArg.Regex.ToString();
                        sb.AppendFormat("AND RegexMatch(a.XLYJson,'{0}') ", temp.Replace("'", "''").Replace("{", "{{").Replace("}", "}}"));
                        break;
                    case FilterByDateRangeArgs dateRangeArg:
                        SetDateTimeRangeSql(dateRangeArg.StartTime, dateRangeArg.EndTime, sb);
                        break;
                    case FilterByBookmarkArgs bookMarkArg:
                        if(bookMarkArg.BookmarkId < 0)
                        {
                            isUnion = true;
                        }
                        else
                        {
                            sb.AppendFormat("AND b.BookMarkId = {0} ", bookMarkArg.BookmarkId);
                        }
                        isAttach = true;
                        break;
                    case FilterByEnumStateArgs stateArg:
                        sb.AppendFormat("AND a.DataState = '{0}' ", stateArg.State.ToString());
                        break;
                    case FilterBySensitiveArgs senstiveArg:
                        sb.AppendFormat("AND a.SensitiveId = {0} ", senstiveArg.SensitiveId);
                        break;
                    default:
                        break;
                }
            }

            if (isUnion)
            {
                sb.Insert(0, "#");
            }
            else if (isAttach)
            {
                sb.Insert(0, "$");
            }

            if (paging)
            {
                sb.AppendFormat("LIMIT {0} OFFSET {1} ", PageSize, _cursor);
            }
           
            return Expression.Constant(sb.ToString());
        }

        private String[] GetDateTimeColumnsName()
        {
            if (_dateTimeColumnsName != null) return _dateTimeColumnsName;
            var displays = DisplayAttributeHelper.FindDisplayAttributes(typeof(TResult));
            PropertyInfo[] proppertyInfos = typeof(TResult).GetProperties();
            String[] columnsName = proppertyInfos.Where(x =>
            {
                DisplayAttribute atrribute = x.GetCustomAttribute<DisplayAttribute>();
                return atrribute != null &&
                    (atrribute.ColumnType == EnumColumnType.DateTime ||
                    x.PropertyType == typeof(DateTime?) ||
                    x.PropertyType == typeof(DateTime));
            }).Select(y =>
            {
                //DisplayAttribute atrribute = y.GetCustomAttribute<DisplayAttribute>();
                //return atrribute.PropertyName;// String.IsNullOrWhiteSpace(atrribute.Text) ? y.Name : atrribute.Text;
                return displays.FirstOrDefault(d => d.Owner == y)?.PropertyName;
            }).ToArray();
            _dateTimeColumnsName = columnsName;
            return columnsName;
        }

        private void SetDateTimeRangeSql(DateTime? start, DateTime? end, StringBuilder sb)
        {
            String[] dateColumnsName = GetDateTimeColumnsName();
            if (dateColumnsName != null && dateColumnsName.Length != 0)
            {
                if (start == null && end == null)
                {
                }
                else if (start == null && end != null)
                {
                    foreach (String str in dateColumnsName)
                    {
                        sb.AppendFormat($"AND REPLACE(a.{str},'/','-') < '{end.Value.ToString("yyyy-MM-dd HH:mm:ss")}' ");
                    }
                }
                else if (start != null && end == null)
                {
                    foreach (String str in dateColumnsName)
                    {
                        sb.AppendFormat($"AND REPLACE(a.{str},'/','-') >= '{start.Value.ToString("yyyy-MM-dd HH:mm:ss")}' ");
                    }
                }
                else
                {
                    foreach (String str in dateColumnsName)
                    {
                        sb.AppendFormat($"AND REPLACE(a.{str},'/','-') >= '{start.Value.ToString("yyyy-MM-dd HH:mm:ss")}'AND REPLACE(a.{str},'/','-') < '{end.Value.ToString("yyyy-MM-dd HH:mm:ss")}' ");
                    }
                }
            }
        }

        private void CheckValidation()
        {
            if (_isLocked)
            {
                throw new InvalidOperationException("Can't change value,because filter is runing,please call Reset first.");
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// 支持书签过滤的数据集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataAggregationFilterView<T> : AggregationFilterView<T> 
        where T : AbstractDataItem
    {
        public DataAggregationFilterView(IFilterable source, string key) 
            : base(source, key)
        { }

        public override bool NextPage()
        {
            var rt = base.NextPage();
            AssociatedBookmark();
            return rt;
        }

        public override bool PreviousPage()
        {
            var rt = base.PreviousPage();
            AssociatedBookmark();
            return rt;
        }

        public event Action OnAssociatedBookmark;

        /// <summary>
        /// 关联书签
        /// </summary>
        private void AssociatedBookmark()
        {
            OnAssociatedBookmark?.Invoke();
        }
    }
}
