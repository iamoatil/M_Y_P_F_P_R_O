using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Forms;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;

/* ==============================================================================
* Description：CmdTargetPosition  
* Author     ：litao
* Create Date：2017/11/16 9:50:26
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    /// <summary>
    /// 目标位置
    /// </summary>
    public class CmdTargetPosition : NotifyPropertyBase
    {
        public CmdTargetPosition()
        {
            SetTargetPathCommand = new RelayCommand(new Action(() => Set()));
        }

        /// <summary>
        /// 镜像文件的路径
        /// </summary>
        private string _dirPath = @"C:\XLYSFTasks\";

        public string DirPath
        {
            get { return _dirPath; }
            set
            {
                _dirPath = value;
                OnPropertyChanged();
            }
        }        

        public ICommand SetTargetPathCommand { get; private set; }

        /// <summary>
        /// 设置目标位置
        /// </summary>
        public void Set()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult dr = folderBrowserDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string filePath = folderBrowserDialog.SelectedPath;
                if (!filePath.EndsWith("\\"))
                {
                    filePath += "\\";
                }
                DirPath = filePath;
            }
        }
    }
}
