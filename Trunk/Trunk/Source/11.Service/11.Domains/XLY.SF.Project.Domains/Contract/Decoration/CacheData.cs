using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据缓存类，提供超时清除功能
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class CacheData<T1, T2>
    {
        public CacheData(int time)
        {
            _dic = new Dictionary<T1, T2>();
            Timeout = time;
            _date = DateTime.Now;
            _timer = new TimeoutTimer(Timeout, ClearCache);
        }
        private TimeoutTimer _timer;
        private Dictionary<T1, T2> _dic;
        private DateTime _date = DateTime.Now;
        public int Timeout { get; set; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Clear()
        {
            _date = DateTime.Now;
            _dic.Clear();
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(T1 key, T2 value)
        {
            _date = DateTime.Now;
            _dic[key] = value;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Remove(T1 key)
        {
            _date = DateTime.Now;
            _dic.Remove(key);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Contains(T1 key)
        {
            _date = DateTime.Now;
            return _dic.ContainsKey(key);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ClearCache()
        {
            if (_date.AddMilliseconds(Timeout) < DateTime.Now)
            {
                _dic.Clear();
            }
        }
        public T2 this[T1 key]
        {
            get
            {
                _date = DateTime.Now;
                return _dic[key];
            }
            set
            {
                _date = DateTime.Now;
                _dic[key] = value;
            }
        }
    }
}
