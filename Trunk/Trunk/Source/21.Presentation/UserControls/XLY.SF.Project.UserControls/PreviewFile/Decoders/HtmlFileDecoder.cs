using System;
using System.Windows;
using System.Windows.Controls;

namespace XLY.SF.Project.UserControls.PreviewFile.Decoders
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
