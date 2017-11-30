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
using System.Linq;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Project.ViewModels.Management.User
{
    [Export(ExportKeys.SettingsUserListViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserListViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _addProxyCommand;

        private readonly ProxyRelayCommandBase _removeProxyCommand;

        private readonly ProxyRelayCommandBase _updateProxyCommand;

        private readonly ProxyRelayCommandBase _searchProxyCommand;

        private readonly ProxyRelayCommandBase _removeBatchCommand;

        #endregion

        #region Constructors

        public UserListViewModel()
        {
            _addProxyCommand = new ProxyRelayCommand(Add);
            _removeProxyCommand = new ProxyRelayCommand<UserInfoModel>(Remove);
            _updateProxyCommand = new ProxyRelayCommand<UserInfoModel>(Update);
            _searchProxyCommand = new ProxyRelayCommand<String>(Search);
            SelectAllCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand<Boolean>(SelectAll, (b) => Users != null && Users.Count != 0);
            SelectCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand<Boolean>(Select);
            _removeBatchCommand = new ProxyRelayCommand(RemoveBatch);
        }

        #endregion

        #region Properties

        public ICommand AddCommand => _addProxyCommand.ViewExecuteCmd;

        public ICommand RemoveCommand => _removeProxyCommand.ViewExecuteCmd;

        public ICommand UpdateCommand => _updateProxyCommand.ViewExecuteCmd;

        public ICommand SearchCommand => _searchProxyCommand.ViewExecuteCmd;

        public ICommand RemoveBatchCommand => _removeBatchCommand.ViewExecuteCmd;

        public ICommand SelectAllCommand { get; }

        public ICommand SelectCommand { get; }

        #region IsSelectAll

        private Boolean? _isSelectAll = false;

        public Boolean? IsSelectAll
        {
            get => _isSelectAll;
            private set
            {
                _isSelectAll = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Users

        private ObservableCollection<UserInfoModel> _users;

        public ObservableCollection<UserInfoModel> Users
        {
            get => _users;
            private set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        #endregion

        [Import(typeof(IRecordContext<UserInfo>))]
        private IRecordContext<UserInfo> DbService { get; set; }

        [Import(typeof(IPopupWindowService))]
        private IPopupWindowService PopupService { get; set; }

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        #endregion

        #region Methods

        #region Protected

        protected override void InitLoad(object parameters)
        {
            Search(null);
        }

        #endregion

        #region Private

        private String RemoveBatch()
        {
            if (Users == null) return "没有可删除的用户";
            UserInfoModel[] items = Users.Where(x => x.IsChecked).ToArray();
            if(items.Length ==0) return "没有可删除的用户";
            if (MessageBox.ShowDialogWarningMsg("是否确定删除？"))
            {
                DbService.RemoveRange(items.Select(x => x.Entity).ToArray());
                foreach (var item in items)
                {
                    Users.Remove(item);
                }
                return "批量删除用户";
            }
            return "取消删除用户";
        }

        private void SelectAll(Boolean isSelectAll)
        {
            IsSelectAll = isSelectAll;
            foreach (var item in Users)
            {
                item.IsChecked = isSelectAll;
            }
        }

        private void Select(Boolean isChecked)
        {
            if (Users.All(x => x.IsChecked))
            {
                IsSelectAll = true;
            }
            else if (Users.All(x => !x.IsChecked))
            {
                IsSelectAll = false;
            }
            else 
            {
                IsSelectAll = null;
            }
        }

        private String Add()
        {
            Object result = PopupService.ShowDialogWindow(ExportKeys.SettingsUserInfoView, new UserInfoModel());
            if (result == null) return "取消添加用户";
            UserInfoModel item = (UserInfoModel)result;
            item.LoginPassword = item.Password;
            if (DbService.Add(item.Entity))
            {
                Users.Add(item);
                return "添加用户成功";
            }
            return "添加用户失败";
        }

        private String Remove(UserInfoModel item)
        {
            if (MessageBox.ShowDialogWarningMsg("是否确定删除？"))
            {
                if (DbService.Remove(item.Entity))
                {
                    Users.Remove(item);
                    return "删除用户成功";
                }
                return "删除用户失败";
            }
            return "取消删除用户";
        }

        private String Update(UserInfoModel item)
        {
            Object result = PopupService.ShowDialogWindow(ExportKeys.SettingsUserInfoView, item);
            if (result == null) return "取消更新用户";
            if (DbService.Update(item.Entity))
            {
                return "更新用户失败";
            }
            return "更新用户失败";
        }

        private String Search(String condition)
        {
            UserInfoModel[] items;
            if (String.IsNullOrWhiteSpace(condition))
            {
                items = DbService.Records.ToModels<UserInfo, UserInfoModel>().ToArray();
            }
            else
            {
                var result = DbService.Records.Where(x => x.UserName.Contains(condition)
                || x.LoginUserName.Contains(condition)
                || x.PhoneNumber.Contains(condition)
                || x.WorkUnit.Contains(condition)
                || x.IdNumber.Contains(condition));
                items = result.ToModels<UserInfo, UserInfoModel>().ToArray();
            }
            Users = new ObservableCollection<UserInfoModel>(items);
            return $"搜索关键字[{condition}]";
        }

        #endregion

        #endregion
    }
}
