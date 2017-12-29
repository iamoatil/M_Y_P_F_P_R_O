using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
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
        private IPopupWindowService PopupWindowService { get; set; }

        public CmdTargetPosition()
        {
            SetTargetPathCommand = new RelayCommand(new Action(() => Set()));

            PopupWindowService = Framework.Core.Base.MefIoc.IocManagerSingle.Instance.GetPart<IPopupWindowService>();
        }

        /// <summary>
        /// 镜像文件的路径
        /// </summary>
        private string _dirPath = @"C:\SPFMirror\";

        public string DirPath
        {
            get { return _dirPath; }
            set
            {
                _dirPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 镜像文件路径
        /// </summary>
        public string TargetMirrorFile { get; set; }

        public ICommand SetTargetPathCommand { get; private set; }

        /// <summary>
        /// 设置目标位置
        /// </summary>
        public void Set()
        {
            string filePath = PopupWindowService.SelectFolderDialog();

            if (!String.IsNullOrEmpty(filePath))
            {
                if (!filePath.EndsWith("\\"))
                {
                    filePath += "\\";
                }

                DirPath = filePath;
            }
        }
    }
}
