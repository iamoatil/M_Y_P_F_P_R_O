using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XLY.SF.Project.Themes.CustromControl.WebrowserEx.Element;

namespace XLY.SF.Project.Themes.CustromControl
{
    /// <summary>
    /// WebBrowserOverlay.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowserOverlay : Window, IDisposable
    {
        private FrameworkElement _placementTarget;

        public WebBrowserOverlay(FrameworkElement placementTarget)
        {
            InitializeComponent();
            _placementTarget = placementTarget;
            Window owner = Window.GetWindow(placementTarget);
            if (owner != null)
            {
                //owner.SizeChanged += delegate { OnSizeLocationChanged(); };
                owner.LocationChanged += delegate { OnSizeLocationChanged(); };
                _placementTarget.SizeChanged += delegate { OnSizeLocationChanged(); };

                if (owner.IsVisible)
                {
                    Owner = owner;
                    Show();
                    OnSizeLocationChanged();
                }
                else
                    owner.IsVisibleChanged += delegate
                    {
                        if (owner.IsVisible)
                        {
                            Owner = owner;
                            Show();
                        }
                    };
            }

            wbContainer?.SuppressScriptErrors(true);
        }

        public void Dispose()
        {
            wbContainer?.Dispose();
        }

        #region Methods

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
                // Delayed call to avoid crash due to Window bug.
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Owner.Close();
                });
        }

        private void OnSizeLocationChanged()
        {
            Point offset = _placementTarget.TranslatePoint(new Point(), Owner);
            Point size = new Point(_placementTarget.ActualWidth, _placementTarget.ActualHeight);
            HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(Owner);
            CompositionTarget ct = hwndSource.CompositionTarget;
            offset = ct.TransformToDevice.Transform(offset);
            size = ct.TransformToDevice.Transform(size);

            Win32.POINT screenLocation = new Win32.POINT(offset);
            Win32.ClientToScreen(hwndSource.Handle, ref screenLocation);
            Win32.POINT screenSize = new Win32.POINT(size);

            Win32.MoveWindow(((HwndSource)HwndSource.FromVisual(this)).Handle, screenLocation.X, screenLocation.Y, screenSize.X, screenSize.Y, true);
        }

        #endregion
    }

    public static class WebBrowserExtensions
    {
        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
    }

}
