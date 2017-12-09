using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Management.Settings
{
    [Export(ExportKeys.SettingsCaseTypeViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CaseTypeSettingsViewModel : ViewModelBase
    {
        #region Fields

        private readonly IRecordContext<CaseType> _dbService;

        private readonly ProxyRelayCommandBase _addProxyCommand;

        private readonly ProxyRelayCommandBase _removeProxyCommand;

        #endregion

        #region Contructors

        [ImportingConstructor]
        public CaseTypeSettingsViewModel(IRecordContext<CaseType> dbService)
        {
            _dbService = dbService;
            _addProxyCommand = new ProxyRelayCommand(Add, base.ModelName, () => CaseTypeName != String.Empty);
            _removeProxyCommand = new ProxyRelayCommand(Remove, base.ModelName, () => SelctedItem != null);
            CaseType[] cts = dbService.Records.ToArray();
            CaseTypes = new ObservableCollection<CaseType>(cts);
        }

        #endregion

        #region Properties

        public ICommand AddCommand => _addProxyCommand.ViewExecuteCmd;

        public ICommand RemoveCommand => _removeProxyCommand.ViewExecuteCmd;

        #region SelctedItem

        private CaseType _selectedItem;
        public CaseType SelctedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region CaseTypeName

        private String _caseTypeName = String.Empty;
        public String CaseTypeName
        {
            get => _caseTypeName;
            set
            {
                _caseTypeName = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        #endregion

        #region CaseTypes

        private ObservableCollection<CaseType> _caseTypes;

        public ObservableCollection<CaseType> CaseTypes
        {
            get => _caseTypes;
            private set
            {
                _caseTypes = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Private

        private String Add()
        {
            if (CaseTypes.Any(x => x.Name == CaseTypeName))
            {
                return "已存在同名的案例类型";
            }
            CaseType ct = new CaseType();
            ct.Name = CaseTypeName;
            if (_dbService.Add(ct))
            {
                CaseTypes.Add(ct);
                CaseTypeName = String.Empty;
                MessageAggregation.SendGeneralMsg(new GeneralArgs(GeneralKeys.SettingsChangedMsg));
                return "添加案例类型成功";
            }
            return "添加案例类型失败";
        }

        private String Remove()
        {
            if (_dbService.Remove(SelctedItem))
            {
                CaseTypes.Remove(SelctedItem);
                SelctedItem = null;
                MessageAggregation.SendGeneralMsg(new GeneralArgs(GeneralKeys.SettingsChangedMsg));
                return "删除案例类型成功";
            }
            return "删除案例类型失败";
        }

        #endregion

        #endregion
    }
}
