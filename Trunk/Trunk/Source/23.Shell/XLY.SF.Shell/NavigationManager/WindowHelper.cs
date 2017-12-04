using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Shell.NavigationManager
{
    public class WindowHelper : NotifyPropertyBase
    {
        #region 单例

        private static object _objLock = new object();

        private static volatile WindowHelper _instance;

        private WindowHelper()
        {
            WinMaxWidth = SystemParameters.PrimaryScreenWidth;
            WinMaxHeight = SystemParameters.PrimaryScreenHeight;
            _curOpenWindows = new Dictionary<Guid, Shell>();
        }

        public static WindowHelper Instance
        {
            get
            {
                if (_instance == null)
                    lock (_objLock)
                        if (_instance == null)
                            _instance = new WindowHelper();
                return _instance;
            }
        }

        #endregion

        #region 窗体最大尺寸【界面绑定使用】

        private double _winMaxWidth;

        public double WinMaxWidth
        {
            get
            {
                return this._winMaxWidth;
            }

            private set
            {
                this._winMaxWidth = value;
                base.OnPropertyChanged();
            }
        }


        private double _winMaxHeight;

        public double WinMaxHeight
        {
            get
            {
                return this._winMaxHeight;
            }

            private set
            {
                this._winMaxHeight = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 当前创建的所有窗体，除主窗体外

        /// <summary>
        /// 当前打开的所有窗口
        /// </summary>
        private Dictionary<Guid, Shell> _curOpenWindows;

        /// <summary>
        /// 添加已创建的新窗体
        /// </summary>
        /// <param name="newWindow"></param>
        private void AddWindow(Shell newWindow)
        {
            if (!_curOpenWindows.ContainsKey(newWindow.Content.ViewID))
            {
                _curOpenWindows.Add(newWindow.Content.ViewID, newWindow);
            }
        }

        /// <summary>
        /// 删除已打开的窗体
        /// </summary>
        /// <param name="viewModelID">已打开的ViewModelID</param>
        /// <param name="needClose">是否需要关闭操作</param>
        public void RemoveOpenedWindowAndCleanUp(Guid viewModelID, bool needClose)
        {
            if (_curOpenWindows.ContainsKey(viewModelID))
            {
                if (needClose)
                    _curOpenWindows[viewModelID].Close();
                _curOpenWindows[viewModelID].Content.DataSource?.ViewClosedCallback();
                _curOpenWindows.Remove(viewModelID);
            }
        }

        #endregion

        #region 创建窗体

        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="view">显示内容</param>
        /// <param name="owner">父窗体</param>
        /// <returns></returns>
        public Shell CreateShellWindow(UcViewBase view, bool showInTaskBar, Window owner = null)
        {
            Shell newWindow = null;
            if (view != null)
            {
                newWindow = new Shell();
                newWindow.WindowState = view.IsMaxView ? WindowState.Maximized : WindowState.Normal;
                Binding s = new Binding("Title") { Source = view };
                newWindow.SetBinding(Window.TitleProperty, s);
                newWindow.ShowMaxsize = newWindow.ShowMinsize = view.NeedMaxsizeAndMinsize;
                if (view.Height > 0 && view.Width > 0)
                {
                    //TODO
                    //此处由于阴影问题，所以需要给实际展示内容增加80像素
                    newWindow.Width = view.Width + 40 * 2;
                    newWindow.Height = view.Height + 40 * 2;
                }
                else
                    newWindow.SizeToContent = SizeToContent.WidthAndHeight;
                newWindow.Content = view;
                //排除未显示的窗体
                if (owner != null && owner.Visibility != Visibility.Collapsed)
                    newWindow.Owner = owner;
                newWindow.ShowInTaskbar = showInTaskBar;
                this.AddWindow(newWindow);
            }
            return newWindow;
        }

        #endregion
    }
}
