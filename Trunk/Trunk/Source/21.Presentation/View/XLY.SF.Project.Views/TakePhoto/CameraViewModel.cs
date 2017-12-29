using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace XLY.SF.Project.Views.TakePhoto
{
    class CameraViewModel : INotifyPropertyChanged
    {
        public CameraViewModel()
        {
            TakePhotoCommand = new RelayCommand<CameraPlayer>(TakePhoto);
            CloseWindowCommand = new RelayCommand<Object>(CloseWindow);
            DevLooksManager = new DeviceLooksManager();
            DevLooksManager.Initialize(Path.GetFullPath("DeviceLooks"));
        }
        public DeviceLooksManager DevLooksManager { get; private set; }

        /// <summary>
        /// 拍照命令
        /// </summary>
        public ICommand TakePhotoCommand { get; private set; }

        /// <summary>
        /// 拍关闭窗口
        /// </summary>
        public ICommand CloseWindowCommand { get; private set; } 

        /// <summary>
        /// 拍照
        /// </summary>
        private void TakePhoto(CameraPlayer player)
        {
            DeviceLooks devLook = DevLooksManager.SelectedItem;
            if(devLook != null)
            {
                string path = devLook.ImagePath;
                player.TakePhoto(path);
                devLook.CheckValidate();
            }
        }

        private void CloseWindow(Object win)
        {
            Window window = win as Window;
            if(window != null)
            {
                window.Close();
            }
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
