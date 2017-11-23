using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Framework.Core.Base.MessageBase.Navigation
{
    /// <summary>
    /// 导航缓存管理，非线程安全
    /// </summary>
    public class NavigationCacheManager<TToken>
    {
        /// <summary>
        /// 当前缓存数据
        /// </summary>
        private Dictionary<TToken, NavigationViewElement> _curCacheItems;

        public NavigationCacheManager()
        {
            _curCacheItems = new Dictionary<TToken, NavigationViewElement>();
        }

        /// <summary>
        /// 添加视图缓存
        /// </summary>
        /// <param name="token">标识</param>
        /// <param name="view">视图</param>
        public void AddViewCache(TToken token, UcViewBase view)
        {
            if (!_curCacheItems.ContainsKey(token))
                _curCacheItems.Add(token, new NavigationViewElement(view));
        }

        /// <summary>
        /// 删除视图缓存
        /// </summary>
        /// <param name="token">标识</param>
        public void RemoveViewCache(TToken token)
        {
            if (_curCacheItems.ContainsKey(token))
                _curCacheItems.Remove(token);
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void Clear()
        {
            _curCacheItems.Clear();
        }

        /// <summary>
        /// 获取指定条件的视图
        /// </summary>
        /// <param name="queryCallback">查询条件</param>
        /// <returns></returns>
        public List<UcViewBase> QueryView(Func<TToken, bool> queryCallback)
        {
            var a = _curCacheItems.Keys.Where(queryCallback);
            List<UcViewBase> result = new List<UcViewBase>();
            foreach (var item in a)
            {
                result.Add(_curCacheItems[item].View);
            }
            return result;
        }

        /// <summary>
        /// 尝试获取指定的视图
        /// </summary>
        /// <param name="token">标识</param>
        /// <param name="cacheView">缓存的视图</param>
        /// <returns></returns>
        public bool TryGetFirstView(TToken token, out UcViewBase cacheView)
        {
            bool hasView = false;
            if (_curCacheItems.ContainsKey(token))
            {
                hasView = true;
                cacheView = _curCacheItems[token].View;
            }
            else
                cacheView = null;

            return hasView;
        }
    }
}
