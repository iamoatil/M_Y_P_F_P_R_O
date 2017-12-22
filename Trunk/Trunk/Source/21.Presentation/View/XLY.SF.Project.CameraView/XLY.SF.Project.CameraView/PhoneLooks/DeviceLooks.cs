using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace XLY.SF.Project.CameraView
{
    class DeviceLooks : INotifyPropertyChanged
    {
        public DeviceLooks(string name, string imagePath)
        {
            this.Name = name;
            if(File.Exists(imagePath))
            {
                this.ImagePath = imagePath;
            }
        }

        /// <summary>
        /// 样貌的名字
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 图片的路径
        /// </summary>
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                IsImagePathInvalidate=!File.Exists(_imagePath);
                OnPropertyChanged();
            }
        }
        private string _imagePath;

        /// <summary>
        /// 图片路径是否无效
        /// </summary>
        public bool IsImagePathInvalidate
        {
            get { return _isImagePathInvalidate; }
            set
            {
                _isImagePathInvalidate = value;
                OnPropertyChanged();
            }
        }
        private bool _isImagePathInvalidate;

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string name = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
