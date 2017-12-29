using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Themes.Behavior.ScrollPageBehavior
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/12/8 14:07:42
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 滚动分页行为，在滚动到顶部或底部时自动翻页
    /// </summary>
    public class ScrollPageBehavior : Behavior<ScrollViewer>
    {
        #region Field

        /// <summary>
        /// 上一次滚动的方向，true为向上滚动，false为向下滚动
        /// </summary>
        private bool? _lastScrollDirection = null;

        /// <summary>
        /// 原始的可分页的数据源
        /// </summary>
        private PageSource _page;

        /// <summary>
        /// 当前缓存的数据集
        /// </summary>
        private ScrollBufferCollection _scrollBuffer;
        #endregion

        #region Event

        protected override void OnAttached()
        {
            base.OnAttached();
            
            #region 初始化时加载两页数据
            try
            {
                var dg = AttachHelper.GetParent<ItemsControl>(AssociatedObject);
                if (dg == null)
                    return;
                if(dg.ItemsSource == null)
                {
                    dg.Loaded += (s,e)=> 
                    {
                        if (dg.ItemsSource == null)
                            return;
                        InitPage(dg);
                    };
                }
                else
                {
                    InitPage(dg);
                }
            }
            catch  //如果是非分页数据源集合，则直接返回
            {
                return;
            }
            #endregion

            var dpd = DependencyPropertyDescriptor.FromProperty(ScrollViewer.VerticalOffsetProperty, AssociatedType);
            dpd.AddValueChanged(AssociatedObject, (sender, args) => { OnScroll(); });   //加载滚动事件监听
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        /// <summary>
        /// 初始化时加载两页数据
        /// </summary>
        /// <param name="container"></param>
        private void InitPage(ItemsControl container)
        {
            _page = new PageSource(container.ItemsSource);
            _page.PageSize = (int)container.GetValue(AttachHelper.ScrollPageSizeProperty);
            _scrollBuffer = new ScrollBufferCollection();
            _scrollBuffer.Transaction(() =>
            {
                _scrollBuffer.AddRange(_page.View);
                if (_page.NextPage())
                {
                    _scrollBuffer.AddRange(_page.View);
                }
            });
            container.ItemsSource = _scrollBuffer;
        }

        private void OnScroll()
        {
            if (AssociatedObject.ScrollableHeight == 0)
                return;
            #region 如果滚动到底部，则向后翻页
            if (AssociatedObject.VerticalOffset >= AssociatedObject.ScrollableHeight)
            {
                _lastScrollDirection = false;
                #region 如果上一次滚动是往下滚
                if (_lastScrollDirection == null || _lastScrollDirection == false)
                {
                    if (_page.Cursor >= _page.Count - _page.PageSize)  //只需要滚动到倒数第二页，缓冲区是2页数据
                        return;
                    _scrollBuffer.Transaction(() =>
                    {
                        _scrollBuffer.Clear();
                        _scrollBuffer.AddRange(_page.View);
                        if (_page.NextPage())
                        {
                            _scrollBuffer.AddRange(_page.View);
                        }
                    });

                    AssociatedObject.ScrollToVerticalOffset(Math.Max(0, AssociatedObject.VerticalOffset - _page.PageSize));
                }
                #endregion
                #region 如果上一次滚动是往上滚
                else
                {
                    if (_page.Cursor >= _page.Count)
                        return;
                    _scrollBuffer.Transaction(() =>
                    {
                        _scrollBuffer.Clear();
                        if (_page.NextPage())
                        {
                            _scrollBuffer.AddRange(_page.View);
                        }
                        if (_page.NextPage())
                        {
                            _scrollBuffer.AddRange(_page.View);
                        }
                    });

                    AssociatedObject.ScrollToVerticalOffset(Math.Max(1, AssociatedObject.VerticalOffset - _page.PageSize));
                }
                #endregion
                
                //AssociatedObject.ScrollToVerticalOffset(Math.Max(0, AssociatedObject.VerticalOffset - _page.PageSize));
            }
            #endregion
            #region 如果滚动到顶部，则向前翻页
            else if (AssociatedObject.VerticalOffset <= 0)
            {
                if (_page.Cursor <= 0)
                    return;
                #region 如果上一次滚动是往上滚
                if (_lastScrollDirection == null || _lastScrollDirection == true)
                {
                    _scrollBuffer.Transaction(() =>
                    {
                        _scrollBuffer.Clear();
                        _scrollBuffer.AddRange(_page.View);
                        if (_page.PrePage())
                        {
                            _scrollBuffer.InsertRange(0, _page.View);
                        }
                    });
                }
                #endregion
                #region 如果上一次滚动是往下滚
                else
                {
                    if (_page.Cursor <= _page.PageSize)
                        return;
                    _scrollBuffer.Transaction(() =>
                    {
                        _scrollBuffer.Clear();
                        if (_page.PrePage())
                        {
                            _scrollBuffer.InsertRange(0, _page.View);
                        }
                        if (_page.PrePage())
                        {
                            _scrollBuffer.InsertRange(0, _page.View);
                        }
                    });
                }
                #endregion
                _lastScrollDirection = true;
                AssociatedObject.ScrollToVerticalOffset(AssociatedObject.VerticalOffset + _page.PageSize);
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// 滚动时的缓存数据集合，一般存储两页数据
        /// </summary>
        class ScrollBufferCollection : INotifyCollectionChanged, IEnumerable
        {
            /// <summary>
            /// 数据集合改变数据
            /// </summary>
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            private List<object> _source = new List<object>();

            private bool _isBeginUpdate = false;

            private bool IsBeginUpdate
            {
                get { return _isBeginUpdate; }
                set
                {
                    _isBeginUpdate = value;
                    if (!value)
                        Update();
                }
            }

            private void Update()
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            /// <summary>
            /// 清空数据数据
            /// </summary>
            public void Clear()
            {
                _source.Clear();
                if (!IsBeginUpdate)
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            /// <summary>
            /// 加入单条数据到集合末尾
            /// </summary>
            /// <param name="item"></param>
            public void Add(object item)
            {
                _source.Add(item);
                if (!IsBeginUpdate)
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }

            /// <summary>
            /// 加入多条数据到集合末尾
            /// </summary>
            /// <param name="item"></param>
            public void AddRange(IEnumerable item)
            {
                foreach (var i in item)
                {
                    _source.Add(i);
                }

                if (!IsBeginUpdate)
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            /// <summary>
            /// 插入单条数据
            /// </summary>
            /// <param name="index"></param>
            /// <param name="item"></param>
            public void Insert(int index, object item)
            {
                _source.Insert(index, item);
                if (!IsBeginUpdate)
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }

            /// <summary>
            /// 插入多条数据
            /// </summary>
            /// <param name="index"></param>
            /// <param name="item"></param>
            public void InsertRange(int index, IEnumerable item)
            {
                foreach (var i in item)
                {
                    _source.Insert(index++, i);
                }
                if (!IsBeginUpdate)
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            public IEnumerator GetEnumerator()
            {
                return _source.GetEnumerator();
            }

            /// <summary>
            /// 批量操作数据，只会在结束时发送集合改变事件命令
            /// </summary>
            /// <param name="action"></param>
            public void Transaction(Action action)
            {
                IsBeginUpdate = true;
                action();
                IsBeginUpdate = false;
            }
        }

        /// <summary>
        /// 可分页的数据源
        /// </summary>
        class PageSource
        {
            public PageSource(object obj)
            {
                _owner = obj;
                PageSize = 50;
            }

            private dynamic _owner;

            public int PageSize { get; set; }
            public int Cursor { get; set; }

            public int Count => _owner.Count;

            public IEnumerable View => _owner.GetView(Cursor, PageSize);

            public bool NextPage()
            {
                if (Cursor + PageSize >= Count)
                    return false;
                Cursor += PageSize;
                return true;
            }

            public bool PrePage()
            {
                if (Cursor <= 0)
                    return false;
                Cursor -= PageSize;
                return true;
            }
        }
    }
}
