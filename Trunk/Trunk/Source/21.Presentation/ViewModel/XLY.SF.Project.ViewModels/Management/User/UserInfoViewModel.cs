using GalaSoft.MvvmLight.Command;
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
    [Export(ExportKeys.ManagementUserInfoViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserInfoViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _cancelProxyCommand;

        private readonly ProxyRelayCommandBase _confirmProxyCommand;

        #endregion

        #region Constructors

        public UserInfoViewModel()
        {
            _cancelProxyCommand = new ProxyRelayCommand(Cancel);
            _confirmProxyCommand = new ProxyRelayCommand<UserInfoEntityModel>(Confirm);
            Item = new UserInfoEntityModel();
        }

        #endregion

        #region Properties

        public ICommand CancelCommand => _cancelProxyCommand.ViewExecuteCmd;

        public ICommand ConfirmCommand => _confirmProxyCommand.ViewExecuteCmd;

        #region UserInfoEntityModel

        private UserInfoEntityModel _item;
        public UserInfoEntityModel Item
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

        #region Private

        private String Cancel()
        {
            throw new NotImplementedException();
        }

        private String Confirm(UserInfoEntityModel item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
