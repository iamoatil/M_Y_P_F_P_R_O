using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace XLY.SF.Project.CameraView
{
    public class DeviceLooks : INotifyPropertyChanged
    {
        public DeviceLooks(string name, string imagePath)
        {
            this.Name = name;
            this.ImagePath = imagePath;
            DeletePhotoCommand = new RelayCommand(DeletePhoto);
            CheckValidate();
        }

        /// <summary>
        /// 删除照片命令
        /// </summary>
        public ICommand DeletePhotoCommand { get; private set; }

        /// <summary>
        /// 样貌的名字
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 图片的路径
        /// </summary>
        public string ImagePath
        {
            get;private set;
        }

        /// <summary>
        /// 检查有效性
        /// </summary>
        public void CheckValidate()
        {
            IsImagePathInvalidate = !File.Exists(ImagePath);
            OnPropertyChanged("ImagePath");

        }
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

        /// <summary>
        /// 删除其图片
        /// </summary>
        public void DeletePhoto()
        {
            if(File.Exists(ImagePath))
            {
                try
                {
                    File.Delete(ImagePath);
                }
                catch (Exception ex)
                {
                }
            }
            CheckValidate();
        }

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
