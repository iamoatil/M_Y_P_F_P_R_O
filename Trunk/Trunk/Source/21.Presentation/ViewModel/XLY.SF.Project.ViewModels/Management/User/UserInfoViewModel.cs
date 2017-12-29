using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Management.User
{
    [Export(ExportKeys.SettingsUserInfoViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserInfoViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _cancelProxyCommand;

        private readonly ProxyRelayCommandBase _confirmCommand;

        #endregion

        #region Constructors

        public UserInfoViewModel()
        {
            _cancelProxyCommand = new ProxyRelayCommand(Cancel, base.ModelName);
            _confirmCommand = new ProxyRelayCommand(Confirm, base.ModelName, CanConfirm);
            Item = new UserInfoModel();
        }

        #endregion

        #region Properties

        public ICommand CancelCommand => _cancelProxyCommand.ViewExecuteCmd;

        public ICommand ConfirmCommand => _confirmCommand.ViewExecuteCmd;

        #region IsUpdate

        private Boolean _isUpdate;

        public Boolean IsUpdate
        {
            get => _isUpdate;
            private set
            {
                _isUpdate = value;
                OnPropertyChanged();
            }
        }

        #endregion

        [Import(typeof(IRecordContext<UserInfo>))]
        private IRecordContext<UserInfo> DbService { get; set; }

        public Boolean IncludePassword
        {
            get => Item.IncludePassword;
            set
            {
                Item.IncludePassword = value;
                OnPropertyChanged();
            }
        }

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

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        #endregion

        #region Methods

        #region Public

        protected override void InitLoad(Object parameters)
        {
            UserInfoModel user = parameters as UserInfoModel;
            Item = user ?? new UserInfoModel();
            if (user == null)
            {
                IncludePassword = true;
                IsUpdate = false;
            }
            else
            {
                IsUpdate = true;
            }
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
            if (IncludePassword)
            {
                if (Item.Password == String.Empty) return false;
                Boolean b = false;
                b = Item.Password == Item.ConfirmPassword;
                if (IsUpdate)
                {
                    b = b && (Item.OldPassword != String.Empty);
                }
                return b;
            }
            return true;
        }

        private String Cancel()
        {
            CloseView();
            return IsUpdate ? "取消更新用户" : "取消添加用户";
        }

        private String Confirm()
        {
            String log = null;
            if (IsUpdate)
            {
                if (Update())
                {
                    DialogResult = true;
                    log = "更新用户成功";
                }
                else
                {
                    MessageBox.ShowDialogErrorMsg(SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_User_UpdateErrorPrompt]);
                    log = "更新用户失败";
                }
            }
            else
            {
                if (Add())
                {
                    DialogResult = true;
                    log = "添加用户成功";
                }
                else
                {
                    MessageBox.ShowSuccessMsg(SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_User_AddErrorPrompt]);
                    log = "添加用户失败";
                }
            }
            CloseView();
            return log;
        }

        private Boolean Add()
        {
            Item.LoginPassword = Item.Password;
            return DbService.Add(Item.Entity);
        }

        private Boolean Update()
        {
            if (Item.IncludePassword)
            {
                if (Item.Entity.LoginPassword != UserInfoModel.EncryptPassword(Item.OldPassword))
                {
                    MessageBox.ShowDialogErrorMsg(SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_User_ModifyPasswordErrorPrompt]);
                    return false;
                }
                Item.LoginPassword = Item.Password;
            }
            return DbService.Update(Item.Entity);
        }

        #endregion

        #endregion
    }
}
