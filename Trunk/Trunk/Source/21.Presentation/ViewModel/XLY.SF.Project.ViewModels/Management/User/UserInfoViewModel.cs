using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Management.User
{
    [Export(ExportKeys.SettingsUserInfoViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserInfoViewModel : ViewModelBase
    {
        #region Constructors

        public UserInfoViewModel()
        {
            CancelCommand = new RelayCommand(Cancel);
            ConfirmCommand = new RelayCommand(Confirm, CanConfirm);
            Item = new UserInfoModel();
        }

        #endregion

        #region Properties

        public ICommand CancelCommand { get; }

        public ICommand ConfirmCommand { get; }

        #region UserInfoModel

        private UserInfoModel _item;
        public UserInfoModel Item
        {
            get => _item;
            set
            {
                _item = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public

        protected override void InitLoad(Object parameters)
        {
            Item = parameters as UserInfoModel;
        }

        public override Object GetResult()
        {
            return Item;
        }

        #endregion

        #region Private

        private Boolean CanConfirm()
        {
            if (Item.UserName == String.Empty
                || Item.WorkUnit == String.Empty
                || Item.IdNumber == String.Empty
                || Item.PhoneNumber == String.Empty
                || Item.LoginUserName == String.Empty)
            {
                return false;
            }
            if (Item.Password == String.Empty) return false;
            return Item.Password == Item.ConfirmPassword;
        }

        private void Cancel()
        {
            CloseView();
        }

        private void Confirm()
        {
            DialogResult = true;
            CloseView();
        }

        #endregion

        #endregion
    }
}
