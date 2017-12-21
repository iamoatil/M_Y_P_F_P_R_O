
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace XLY.SF.Project.CameraView
{
    class ImageManager
    {
        /// <summary>
        /// 默认图片
        /// </summary>
        private Image _defaultImage;

        public bool IsInitialized2 { get; private set; }

        protected void OnInitialized()
        {
            IsInitialized2 = InnerInitialize();
        }

        private bool InnerInitialize()
        {
            _defaultImage = new Image();
            _defaultImage.Source = new BitmapImage(new Uri(@"/Resource/no-camera.png", UriKind.Relative));

            return true;
        }

    }
}
