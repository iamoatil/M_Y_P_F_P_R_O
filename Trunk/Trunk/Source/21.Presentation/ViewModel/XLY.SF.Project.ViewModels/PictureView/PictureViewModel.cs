using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.PictureView
{
    [Export(ExportKeys.PictureViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PictureViewModel : ViewModelBase
    {
        #region 显示的图片

        private string _viewImage;
        /// <summary>
        /// 显示的图片
        /// </summary>
        public string ViewImage
        {
            get
            {
                return this._viewImage;
            }

            set
            {
                this._viewImage = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 关闭命令

        public ICommand ClosePictureViewCommand { get; private set; }

        #endregion

        public PictureViewModel()
        {
            ClosePictureViewCommand = new RelayCommand(ExecuteClosePictureViewCommand);
        }

        private void ExecuteClosePictureViewCommand()
        {
            base.CloseView();
        }

        protected override void InitLoad(object parameters)
        {
            if (parameters != null)
            {
                ViewImage = parameters.ToString();
            }
        }
    }
}
