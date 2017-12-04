using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewModels.Main.CaseManagement;
using System.Linq;
using XLY.SF.Project.ViewDomain.Model.PresentationNavigationElement;
using XLY.SF.Framework.Language;

namespace XLY.SF.Project.ViewModels.Main
{
    /// <summary>
    /// 案例设备列表管理
    /// </summary>
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
            CloseCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand<DeviceExtractionAdorner>(Close);
            _moveToCommandProxy = new ProxyRelayCommand<DeviceExtractionAdorner>(MoveTo);
            _deleteCommandProxy = new ProxyRelayCommand<DeviceExtractionAdorner>(Delete);
            PopupCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand<DeviceExtractionAdorner>(Popup);
            MessageAggregation.RegisterGeneralMsg<object>(this, ExportKeys.DeviceWindowClosedMsg, (d) => BackToList(d.Parameters as DeviceExtractionAdorner));
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

        private ObservableCollection<DeviceExtractionAdorner> _items;
        public ObservableCollection<DeviceExtractionAdorner> Items
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

        private DeviceExtractionAdorner _selectedItem;
        public DeviceExtractionAdorner SelectedItem
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
                        NavigationForMainWindow(ExportKeys.DeviceMainView, value);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Protected

        protected override void InitLoad(object parameters)
        {
            SystemContext.Instance.CaseChanged += Instance_CaseChanged;
            MessageAggregation.UnRegisterMsg<GeneralArgs<DeviceExtractionAdorner>>(this, ExportKeys.DeviceAddedMsg, AddDevice);
            MessageAggregation.RegisterGeneralMsg<DeviceExtractionAdorner>(this, ExportKeys.DeviceAddedMsg, AddDevice);
        }

        #endregion

        #region Private

        private void Instance_CaseChanged(Object sender, PropertyChangedEventArgs<Case> e)
        {
            Case @case = e.NewValue;
            if (@case != null)
            {
                var items = @case.DeviceExtractions.Select(x => new DeviceExtractionAdorner(x)).ToArray();
                Items = new ObservableCollection<DeviceExtractionAdorner>(items);
            }
            else
            {
                Items = null;
            }
        }

        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="args"></param>
        private void AddDevice(GeneralArgs<DeviceExtractionAdorner> args)
        {
            DeviceExtractionAdorner de = args.Parameters;
            if (Items.Any(x => x.Device.ID == de.Device.ID)) return;
            Items.Add(de);
            SelectedItem = de;
        }

        private String SelectDevice()
        {
            SelectedItem = null;
            return "选择数据源";
        }

        /// <summary>
        /// 弹出设备界面
        /// </summary>
        /// <param name="de"></param>
        private void Popup(DeviceExtractionAdorner de)
        {
            Items.Remove(de);
            //NavigationForNewWindow(ExportKeys.DeviceWindowContentView, de, true);

            //TODO
            //由于窗体导航在Shell里面，DeviceExtractionAdorner在ViewModel里面
            //所以此处创建object数组，Shell解析时，固定用此格式
            //
            NavigationForNewWindow(ExportKeys.DeviceMainView, new object[] { de.Device.ID, de });
        }

        /// <summary>
        /// 停靠设备界面到案例
        /// </summary>
        /// <param name="de"></param>
        private void BackToList(DeviceExtractionAdorner de)
        {
            Items.Add(de);
            SelectedItem = de;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <param name="de"></param>
        private void Close(DeviceExtractionAdorner de)
        {
            Items.Remove(de);
            PreCacheToken args = new PreCacheToken(de.Id, ExportKeys.DeviceMainView);
            MessageAggregation.SendGeneralMsg<PreCacheToken>(new GeneralArgs<PreCacheToken>(GeneralKeys.DeleteCacheView) { Parameters = args });
        }

        /// <summary>
        /// 移动设备
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private String MoveTo(DeviceExtractionAdorner de)
        {
            IPopupWindowService win = IocManagerSingle.Instance.GetPart<IPopupWindowService>();
            RecentCaseEntityModel rc = win.ShowDialogWindow(ExportKeys.CaseSelectionView, de.Target.Owner) as RecentCaseEntityModel;
            if (rc == null)
            {
                return $"取消移动设备[{de.Name}]";
            }
            else
            {
                var toCase = Case.Open(rc.CaseProjectFile);
                if (toCase == null)
                {
                    return $"案例[{rc.Name}]不存在";
                }

                Close(de);
                SystemContext.Instance.CurrentCase.Detach(de.Target);
                toCase.Attach(de.Target);

                var log = $"移动设备[{de.Name}]到案例[{toCase.Name}]";

                //移动文件
                Directory.Move(de.Target.Path, Path.Combine(toCase.Path, new DirectoryInfo(de.Target.Path).Name));

                return log;
            }
        }

        /// <summary>
        /// 删除设备 包括相关文件
        /// </summary>
        /// <param name="de"></param>
        /// <returns></returns>
        private String Delete(DeviceExtractionAdorner de)
        {
            if (MessageBox.ShowDialogWarningMsg(SystemContext.LanguageManager[Languagekeys.SourceSelection_DeletePrompt]))
            {
                var log = $"确认删除设备[{de.Name}]";

                Close(de);
                de.Delete();

                return log;
            }
            return $"取消删除设备[{de.Name}]";
        }

        #endregion

        #endregion
    }
}
