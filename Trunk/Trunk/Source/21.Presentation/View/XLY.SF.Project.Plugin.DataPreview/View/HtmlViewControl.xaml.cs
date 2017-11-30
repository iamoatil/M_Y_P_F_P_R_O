using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLY.SF.Project.Plugin.DataPreview.View
{
    /// <summary>
    /// HtmlViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class HtmlViewControl : UserControl
    {
        public HtmlViewControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
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
