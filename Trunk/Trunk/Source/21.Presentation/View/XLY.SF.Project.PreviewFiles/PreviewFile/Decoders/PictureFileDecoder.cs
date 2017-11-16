using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace XLY.SF.Project.PreviewFilesView.PreviewFile
{
    class PictureFileDecoder : IFileDecoder
    {
        public FrameworkElement Element
        {
            get
            {
                return _image;
            }
        }

        private readonly Image _image = new Image();

        public void Decode(string path)
        {
            _image.Source = new BitmapImage(new Uri(path));
        }
    }
}
