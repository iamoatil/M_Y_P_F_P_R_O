using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewModels.Tools;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    [Export(ExportKeys.CaseCreationViewModel, typeof(ViewModelBase))]
    public class CaseCreationViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _confirmCommandProxy;

        private readonly ProxyRelayCommandBase _updateCaseTypeCommandProxy;

        private readonly ProxyRelayCommandBase _skipCommandProxy;

        #endregion

        #region Constructors

        public CaseCreationViewModel()
        {
            _confirmCommandProxy = new ProxyRelayCommand(Confirm, CanConfirm);
            _updateCaseTypeCommandProxy = new ProxyRelayCommand(UpdateCasetType);
            _skipCommandProxy = new ProxyRelayCommand(Skip);
        }

        #endregion

        #region Properties

        #region CaseInfo

        private CaseInfo _caseInfo;

        public CaseInfo CaseInfo
        {
            get { return _caseInfo; }
            set
            {
                _caseInfo = value;
                OnPropertyChanged();
                OnPropertyChanged("IsEnable");
            }
        }

        #endregion

        public Boolean IsEnable => !(CaseInfo is RestrictedCaseInfo);

        public ICommand ConfirmConmmand => _confirmCommandProxy.ViewExecuteCmd;

        public ICommand SkipConmmand => _skipCommandProxy.ViewExecuteCmd;

        public ICommand UpdateCaseTypeCommand => _updateCaseTypeCommandProxy.ViewExecuteCmd;

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        [Import(typeof(IDatabaseContext))]
        private IDatabaseContext DbService
        {
            get;
            set;
        }

        #endregion

        #region Methods

        #region Protected

        protected override void InitLoad(object parameters)
        {
            SystemContext.Instance.CaseChanged += Instance_CaseChanged;
            CaseInfo = NewCaseInfo();
        }

        #endregion

        #region Private

        private void Instance_CaseChanged(object sender, PropertyChangedEventArgs<Case> e)
        {
            if (e.NewValue == null)
            {
                CaseInfo = NewCaseInfo();
            }
            else
            {
                CaseInfo = e.NewValue.CaseInfo;
            }
        }

        private String Confirm()
        {
            if (IsEnable)
            {
                return CreateCase();
            }
            return UpdateCase();
        }

        private String Skip()
        {
            if (IsEnable)
            {
                return CreateCase();
            }
            ((RestrictedCaseInfo)CaseInfo).Reset();
            OnPropertyChanged("CaseInfo");
            return "取消修改案例信息";
        }

        private String CreateCase()
        {
            Case newCase = Case.New(CaseInfo);
            if (newCase == null) return string.Empty;
            SystemContext.Instance.CurrentCase = newCase;
            RecentCaseEntityModel model = new RecentCaseEntityModel
            {
                CaseID = CaseInfo.Id,
                Name = CaseInfo.Name,
                Timestamp = DateTime.Parse(CaseInfo.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")),
                CaseProjectFile = newCase.ProjectFile,
                Number = newCase.CaseInfo.Number,
                LastOpenTime = DateTime.Now,
            };
            if (!DbService.Add(model))
            {
                MessageBox.ShowDialogErrorMsg(SystemContext.LanguageManager[Languagekeys.ViewLanguage_View_UpdateRecentError]);
            }
            else
            {
                CaseInfo = newCase.CaseInfo;

                //收起子页面
                EditCaseNavigationHelper.SetEditCaseViewStatus(false);
                NavigationForMainWindow(ExportKeys.DeviceSelectView);
                return $"创建案例{newCase.Name}成功";
            }
            return $"创建案例{newCase.Name}失败";
        }

        private String UpdateCase()
        {
            if (SystemContext.Instance.CurrentCase.Update())
            {
                MessageBox.ShowDialogNoticeMsg("修改成功");
                return $"更新案例信息{SystemContext.Instance.CurrentCase.Name}成功";
            }
            return $"更新案例信息{SystemContext.Instance.CurrentCase.Name}失败";
        }

        private Boolean CanConfirm()
        {
            return !(String.IsNullOrWhiteSpace(CaseInfo.Name)
                || String.IsNullOrWhiteSpace(CaseInfo.Number)
                || String.IsNullOrWhiteSpace(CaseInfo.Author)
                || String.IsNullOrWhiteSpace(CaseInfo.Type));
        }

        private CaseInfo NewCaseInfo()
        {
            return new CaseInfo()
            {
                Name = "默认案例",
                Number = DateTime.Now.ToString("yyyyMMddhhmmss"),
                Author = SystemContext.Instance.CurUserInfo.UserName,
                Path = SystemContext.Instance.CaseSaveFullPath,
                Type = "临时的",
            };
        }

        private String UpdateCasetType()
        {
            return string.Empty;
        }

        #endregion

        #endregion
    }
}
