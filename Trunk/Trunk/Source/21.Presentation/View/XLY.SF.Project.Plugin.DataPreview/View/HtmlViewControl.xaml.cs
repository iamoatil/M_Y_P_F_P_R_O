using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace XLY.SF.Project.Plugin.DataPreview.View
{
    /// <summary>
    /// HtmlViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class HtmlViewControl : UserControl, IDataPreviewRelease
    {
        public HtmlViewControl()
        {
            InitializeComponent();
        }

        public void Release()
        {
            web.Dispose();
        }

        private bool IsUserControl_Loaded = false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsUserControl_Loaded)
            {//只加载一次
                IsUserControl_Loaded = true;

                DataPreviewPluginArgument arg = this.DataContext as DataPreviewPluginArgument;
                if (arg != null && arg.CurrentData is string fileName)
                {
                    if (File.Exists(fileName))
                    {
                        try
                        {
                            WebBrowserExtensions.SuppressScriptErrors(web, true);
                            web.Navigate(new Uri(fileName, UriKind.RelativeOrAbsolute));
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }
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
