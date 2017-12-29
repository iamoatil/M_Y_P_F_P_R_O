using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace XLY.SF.Project.MirrorView
{
    /// <summary>
    /// 镜像文件
    /// </summary>
    public class MirrorFile: INotifyPropertyChanged
    {
        public MirrorFile()
        {
            OpenFileDirCommand= new RelayCommand(OpenFileDir);
        }
        public void Intialize(string dirPath)
        {
            FilePath = dirPath;
            IsIntegrated = false;
        }

        /// <summary>
        /// 镜像文件的状态
        /// </summary>
        private CmdString _state;

        /// <summary>
        /// md5文件的路径
        /// </summary>
        private string md5FilePath;

        /// <summary>
        /// 镜像文件的路径
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }
        private string _fileName;

        /// <summary>
        /// 文件的Md5字符串
        /// </summary>
        public string  Md5
        {
            get { return _md5; }
            set
            {
                _md5 = value;
                OnPropertyChanged();
            }
        }
        private string _md5;

        /// <summary>
        /// 是否是完整的
        /// </summary>
        public bool IsIntegrated
        {
            get { return _isIntegrated; }
            set
            {
                _isIntegrated = value;
                OnPropertyChanged();
            }
        }
        private bool _isIntegrated;

        /// <summary>
        /// 打开文件目录命令
        /// </summary>
        public ICommand OpenFileDirCommand { get; private set; }

        /// <summary>
        /// 打开文件目录
        /// </summary>
        private void OpenFileDir()
        {
            if(File.Exists(FilePath))
            {
                string dir = Path.GetDirectoryName(FilePath);
                Process.Start("explorer.exe",dir);
            }
        }

        internal void SetState(CmdString state)
        {
            _state = state;
            IsIntegrated = CheckIntegrality(state);
            if(IsIntegrated)
            {
                if(File.Exists(md5FilePath))
                {
                    Md5=File.ReadAllText(md5FilePath);
                    FileName = Path.GetFileName(FilePath);
                }
            }
        }

        /// <summary>
        /// 检测完整性
        /// </summary>
        private bool CheckIntegrality(CmdString state)
        {
            if (!File.Exists(FilePath))
            {
                return false;
            }
            if(state == CmdStrings.AllFinishState)
            {
                string md5File = FilePath+".md5";
                if (File.Exists(md5File))
                {
                    md5FilePath = md5File;
                    return true;
                }
            }
            return false;
        }

        #region  INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性更新（不用给propertyName赋值）
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
