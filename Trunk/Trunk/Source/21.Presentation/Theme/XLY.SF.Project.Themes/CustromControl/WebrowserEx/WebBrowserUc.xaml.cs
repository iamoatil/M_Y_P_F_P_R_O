using System;
using System.Windows;
using System.Windows.Controls;

namespace XLY.SF.Project.Themes.CustromControl
{
    /// <summary>
    /// WebBrowserUc.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowserUc : UserControl, IDisposable
    {
        private WebBrowserOverlay _webOverlay;

        public WebBrowserUc()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _webOverlay = _webOverlay ?? new WebBrowserOverlay(gdMain);
        }

        /// <summary>
        /// 根据URI导航
        /// </summary>
        /// <param name="source"></param>
        public void NavigationForUri(Uri source)
        {
            _webOverlay = _webOverlay ?? new WebBrowserOverlay(gdMain);
            _webOverlay.wbContainer.Navigate(source);
        }

        /// <summary>
        /// 根据string导航
        /// </summary>
        /// <param name="source"></param>
        public void NavigationForString(string source)
        {
            _webOverlay = _webOverlay ?? new WebBrowserOverlay(gdMain);
            _webOverlay.wbContainer.Navigate(source);
        }

        public void Dispose()
        {
            _webOverlay?.Dispose();
        }

        #region Properties

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(WebBrowserUc), new PropertyMetadata(null, new PropertyChangedCallback(SourceChangedCallback)));

        private static void SourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var winTmp = d as WebBrowserUc;
            winTmp?.NavigationForUri(e.NewValue as Uri);
        }

        #endregion
    }
}
