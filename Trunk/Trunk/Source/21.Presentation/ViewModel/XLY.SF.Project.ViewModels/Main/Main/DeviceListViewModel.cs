using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main
{
    [Export(ExportKeys.DeviceListViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DeviceListViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _selectDeviceCommandProxy;

        private readonly ProxyRelayCommandBase _moveToCommandProxy;

        private readonly ProxyRelayCommandBase _deleteCommandProxy;

        #endregion

        #region Constructors

        public DeviceListViewModel()
        {
            _selectDeviceCommandProxy = new ProxyRelayCommand(SelectDevice);
            CloseCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand<DeviceExtraction>(Close);
            _moveToCommandProxy = new ProxyRelayCommand<DeviceExtraction>(MoveTo);
            _deleteCommandProxy = new ProxyRelayCommand<DeviceExtraction>(Delete);
            PopupCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand<DeviceExtraction>(Popup);
            MessageAggregation.RegisterGeneralMsg<DeviceExtraction>(this, ExportKeys.DeviceWindowClosedMsg, (d) => BackToList(d.Parameters));
        }

        #endregion

        #region Properties

        public ICommand SelectDeviceCommand => _selectDeviceCommandProxy.ViewExecuteCmd;

        public ICommand DeleteCommand => _deleteCommandProxy.ViewExecuteCmd;

        public ICommand CloseCommand { get; }

        public ICommand MoveToCommand => _moveToCommandProxy.ViewExecuteCmd;

        public ICommand PopupCommand { get; }

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        #region Items

        private ObservableCollection<DeviceExtraction> _items;
        public ObservableCollection<DeviceExtraction> Items
        {
            get => _items;
            private set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region SelectedItem

        private DeviceExtraction _selectedItem;
        public DeviceExtraction SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                    if (value == null)
                    {
                        NavigationForMainWindow(ExportKeys.DeviceSelectView, value);
                    }
                    else
                    {
                        NavigationForMainWindow(ExportKeys.DeviceHomeView, value);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Protected

        protected override void LoadCore(object parameters)
        {
            SystemContext.Instance.CaseChanged += Instance_CaseChanged;
            MessageAggregation.RegisterGeneralMsg<DeviceExtraction>(this, ExportKeys.DeviceAddedMsg, AddDevice);
        }

        #endregion

        #region Private

        private void Instance_CaseChanged(Object sender, PropertyChangedEventArgs<Case> e)
        {
            Case @case = e.NewValue;
            if (@case != null)
            {
                Items = new ObservableCollection<DeviceExtraction>(@case.DeviceExtractions);
            }
            else
            {
                Items = null;
            }
        }

        private void AddDevice(GeneralArgs<DeviceExtraction> args)
        {
            DeviceExtraction de = args.Parameters;
            if (!Items.Contains(de))
            {
                Items.Add(de);
            }
            SelectedItem = de;
        }

        private String SelectDevice()
        {
            SelectedItem = null;
            return "选择数据源";
        }

        private void Popup(DeviceExtraction de)
        {
            Items.Remove(de);
            NavigationForNewWindow(ExportKeys.DeviceWindowContentView, de, true);
        }

        private void BackToList(DeviceExtraction de)
        {
            Items.Add(de);
        }

        private void Close(DeviceExtraction de)
        {
            Items.Remove(de);
        }

        private String MoveTo(DeviceExtraction de)
        {
            IPopupWindowService win = IocManagerSingle.Instance.GetPart<IPopupWindowService>();
            RecentCaseEntityModel rc = win.ShowDialogWindow(ExportKeys.CaseSelectionView, de) as RecentCaseEntityModel;
            if (rc == null) return $"取消移动设备[{de["Name"]}]";
            return $"移动设备[{de["Name"]}]到案例[{1}]";
        }

        private String Delete(DeviceExtraction de)
        {
            if (MessageBox.ShowDialogNoticeMsg(""))
            {
                Items.Remove(de);
                de.Delete();
                return $"确认删除设备[{de["Name"]}]";
            }
            return $"取消删除设备[{de["Name"]}]";
        }

        #endregion

        #endregion
    }
}
