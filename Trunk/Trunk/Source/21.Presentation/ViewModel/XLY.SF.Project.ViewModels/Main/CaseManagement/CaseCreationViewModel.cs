using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewModels.Tools;
using System.Linq;
using XLY.SF.Framework.Core.Base.MessageBase;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    [Export(ExportKeys.CaseCreationViewModel, typeof(ViewModelBase))]
    public class CaseCreationViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _confirmCommandProxy;

        private readonly ProxyRelayCommandBase _updateCaseTypeCommandProxy;

        private readonly ProxyRelayCommandBase _skipCommandProxy;

        private readonly IRecordContext<CaseType> _caseTypeService;

        #endregion

        #region Constructors

        [ImportingConstructor]
        public CaseCreationViewModel(IRecordContext<CaseType> caseTypeService)
        {
            _confirmCommandProxy = new ProxyRelayCommand(Confirm, base.ModelName, CanConfirm);
            _updateCaseTypeCommandProxy = new ProxyRelayCommand(UpdateCasetType, base.ModelName);
            _skipCommandProxy = new ProxyRelayCommand(Skip, base.ModelName);
            SelectDirectoryCommand = new RelayCommand(SelectDirectory);
            _caseTypeService = caseTypeService;
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

        public ICommand SelectDirectoryCommand { get; }

        private CaseType[] _caseTypes;
        public CaseType[] CaseTypes
        {
            get => _caseTypes;
            private set
            {
                _caseTypes = value;
                OnPropertyChanged();
            }
        }

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        [Import(typeof(ILogicalDataContext))]
        private ILogicalDataContext DbService
        {
            get;
            set;
        }

        [Import(typeof(IPopupWindowService))]
        private IPopupWindowService PopupService { get; set; }

        #endregion

        #region Methods

        #region Protected

        protected override void InitLoad(object parameters)
        {
            SystemContext.Instance.CaseChanged += Instance_CaseChanged;
            CaseInfo = NewCaseInfo();
            LoadCaseTypes(null);
            MessageAggregation.RegisterGeneralMsg(this, GeneralKeys.SettingsChangedMsg, LoadCaseTypes);
        }

        protected override void Closed()
        {
            base.Closed();
            MessageAggregation.UnRegisterMsg<GeneralArgs>(this, GeneralKeys.SettingsChangedMsg, LoadCaseTypes);
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

        private void SelectDirectory()
        {
            String directory = PopupService.SelectFolderDialog();
            CaseInfo.Path = directory;
            OnPropertyChanged("CaseInfo");
        }

        private void LoadCaseTypes(GeneralArgs args)
        {
            CaseTypes = _caseTypeService.Records.ToArray();
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
                Author = SystemContext.Instance.CurUserInfo.UserName,
                Type = CaseInfo.Type,
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
                MessageBox.ShowDialogWarningMsg("修改成功");
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
