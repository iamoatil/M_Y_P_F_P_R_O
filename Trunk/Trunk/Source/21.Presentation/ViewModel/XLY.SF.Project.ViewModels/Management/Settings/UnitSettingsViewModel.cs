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
    [Export(ExportKeys.SettingsUnitViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UnitSettingsViewModel : ViewModelBase
    {
        #region Fields

        private readonly IRecordContext<WorkUnit> _dbService;

        private readonly ProxyRelayCommandBase _addProxyCommand;

        private readonly ProxyRelayCommandBase _removeProxyCommand;

        #endregion

        #region Contructors

        [ImportingConstructor]
        public UnitSettingsViewModel(IRecordContext<WorkUnit> dbService)
        {
            _dbService = dbService;
            _addProxyCommand = new ProxyRelayCommand(Add, () => Number != String.Empty && UnitName != String.Empty);
            _removeProxyCommand = new ProxyRelayCommand(Remove, () => SelctedItem != null);
            WorkUnit[] cts = dbService.Records.ToArray();
            WorkUnits = new ObservableCollection<WorkUnit>(cts);
        }

        #endregion

        #region Properties

        public ICommand AddCommand => _addProxyCommand.ViewExecuteCmd;

        public ICommand RemoveCommand => _removeProxyCommand.ViewExecuteCmd;

        #region SelctedItem

        private WorkUnit _selectedItem;
        public WorkUnit SelctedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region UnitName

        private String _unitName = String.Empty;
        public String UnitName
        {
            get => _unitName;
            set
            {
                _unitName = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        #endregion

        #region Number

        private String _number = String.Empty;
        public String Number
        {
            get => _number;
            set
            {
                _number = (value ?? String.Empty).Trim();
                OnPropertyChanged();
            }
        }

        #endregion

        #region WorkUnits

        private ObservableCollection<WorkUnit> _workUnits;

        public ObservableCollection<WorkUnit> WorkUnits
        {
            get => _workUnits;
            private set
            {
                _workUnits = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Private

        private String Add()
        {
            if (WorkUnits.Any(x => x.Number == Number))
            {
                return "已存在相同单位";
            }
            WorkUnit ct = new WorkUnit();
            ct.Unit = UnitName;
            ct.Number = Number;
            if (_dbService.Add(ct))
            {
                WorkUnits.Add(ct);
                Number = String.Empty;
                UnitName = String.Empty;
                MessageAggregation.SendGeneralMsg(new GeneralArgs(GeneralKeys.SettingsChangedMsg));
                return "添加单位成功";
            }
            return "添加单位失败";
        }

        private String Remove()
        {
            if (_dbService.Remove(SelctedItem))
            {
                WorkUnits.Remove(SelctedItem);
                SelctedItem = null;
                MessageAggregation.SendGeneralMsg(new GeneralArgs(GeneralKeys.SettingsChangedMsg));
                return "删除单位成功";
            }
            return "删除单位失败";
        }

        #endregion

        #endregion

    }
}
