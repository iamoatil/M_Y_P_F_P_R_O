using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace XLY.SF.Project.CameraView
{
    class CameraViewModel : INotifyPropertyChanged
    {
        public CameraViewModel()
        {
            TakePhotoCommand = new RelayCommand<CameraPlayer>(TakePhoto);
           
            DevLooksManager = new DeviceLooksManager();
            DevLooksManager.Initialize(Path.GetFullPath("DeviceLooks"));
        }

        public DeviceLooksManager DevLooksManager { get; private set; }

        /// <summary>
        /// 拍照命令
        /// </summary>
        public ICommand TakePhotoCommand { get; private set; }

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
