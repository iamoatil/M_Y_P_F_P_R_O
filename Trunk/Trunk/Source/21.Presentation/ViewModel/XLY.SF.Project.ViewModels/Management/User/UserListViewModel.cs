using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Management.User
{
    [Export(ExportKeys.ManagementUserListViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserListViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _addProxyCommand;

        private readonly ProxyRelayCommandBase _removeProxyCommand;

        private readonly ProxyRelayCommandBase _updateProxyCommand;

        private readonly ProxyRelayCommandBase _searchProxyCommand;

        #endregion

        #region Constructors

        public UserListViewModel()
        {
            _addProxyCommand = new ProxyRelayCommand<UserInfoEntityModel>(Add);
            _removeProxyCommand = new ProxyRelayCommand(Remove, () => CanModify());
            _updateProxyCommand = new ProxyRelayCommand(Update, () => CanModify());
            _searchProxyCommand = new ProxyRelayCommand<String>(Search);
        }

        #endregion

        #region Properties

        public ICommand AddCommand => _addProxyCommand.ViewExecuteCmd;

        public ICommand RemoveCommand => _removeProxyCommand.ViewExecuteCmd;

        public ICommand UpdateCommand => _updateProxyCommand.ViewExecuteCmd;

        public ICommand SearchCommand => _searchProxyCommand.ViewExecuteCmd;

        #region Users

        private ObservableCollection<UserInfoEntityModel> _users;

        public ObservableCollection<UserInfoEntityModel> Users
        {
            get => _users;
            private set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        #endregion

        [Import(typeof(IDatabaseContext))]
        private IDatabaseContext DbService { get; set; }

        public UserInfoEntityModel SelectedItem { get; set; }

        #endregion

        #region Methods

        #region Protected

        protected override void InitLoad(object parameters)
        {
            Search(null);
        }

        #endregion

        #region Private

        private Boolean CanModify()
        {
            return SelectedItem != null;
        }

        private String Add(UserInfoEntityModel item)
        {
            if (DbService.Add(item))
            {
                Users.Add(item);
                return "添加用户成功";
            }
            return "添加用户失败";
        }

        private String Remove()
        {
            if (DbService.Remove(SelectedItem))
            {
                Users.Remove(SelectedItem);
                return "删除用户成功";
            }
            return "删除用户失败";
        }

        private String Update()
        {
            if (DbService.Update(SelectedItem))
            {
                return "更新用户失败";
            }
            return "更新用户失败";
        }

        private String Search(String condition)
        {
            if (String.IsNullOrWhiteSpace(condition))
            {
                var items = DbService.UserInfos.ToModels<UserInfo, UserInfoEntityModel>();
                Users = new ObservableCollection<UserInfoEntityModel>(items);
            }
            else
            {
            }
            return $"搜索关键字[{condition}]";
        }

        #endregion

        #endregion
    }
}
