using System;
using System.Windows;
using System.Windows.Controls;

namespace XLY.SF.Project.PreviewFilesView.PreviewFile
{
    class HtmlFileDecoder : IFileDecoder
    {
        public FrameworkElement Element
        {
            get { return webBrowser; }
        }

        readonly WebBrowser webBrowser = new WebBrowser();
        public void Decode(string path)
        {
            webBrowser.Source = new Uri(path);
        }
    }
}
