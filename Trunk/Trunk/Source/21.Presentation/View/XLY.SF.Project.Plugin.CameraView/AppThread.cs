
using System;
using System.Windows.Threading;

namespace ProjectExtend.Context
{
    public class AppThread
    {
        #region
        private AppThread() { }
        private static AppThread _appThread = new AppThread();
        public static AppThread Instance { get { return _appThread; } }
        #endregion

        /// <summary>
        /// 主线程的Dispatcher
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// 初始化是否成功
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 初始化.必须在界面线程中调用
        /// </summary>
        public void Initialize()
        {
            IsInitialized = InnerInitialize();
        }

        private bool InnerInitialize()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            return true;
        }

        public void Invoke(Action callback)
        {
            if(!IsInitialized)
            {
                return;
            }
            _dispatcher.Invoke(callback);
        }

        public void BeginInvoke(Action callback)
        {
            if (!IsInitialized)
            {
                return;
            }
            _dispatcher.BeginInvoke(callback);
        }
    }
}
