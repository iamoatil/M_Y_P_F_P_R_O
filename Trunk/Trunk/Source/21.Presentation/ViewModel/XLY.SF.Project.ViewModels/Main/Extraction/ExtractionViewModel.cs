using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.IsolatedTaskEngine.Common;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Extraction
{
    [Export(ExportKeys.ExtractionViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ExtractionViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _addPlanCommandProxy;

        private readonly ProxyRelayCommandBase _removePlanCommandProxy;

        private const String ServerExeName = "XLY.SF.Project.DeviceExtractionService.exe";

        private TaskProxy _proxy;

        private readonly Dictionary<String, ExtractItemAdorner> _executeItems = new Dictionary<String, ExtractItemAdorner>();

        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

        private readonly Timer _timer;

        #endregion

        #region Constructors

        public ExtractionViewModel()
        {
            _addPlanCommandProxy = new ProxyRelayCommand(Add, base.ModelName, () => PlanName != String.Empty);
            _removePlanCommandProxy = new ProxyRelayCommand<ExtractionPlanModel>(Remove, base.ModelName);
            StartCommand = new RelayCommand(Start, () => Items != null && Items.Any(x => x.IsChecked));
            StopCommand = new RelayCommand(Stop);
            _timer = new Timer(Interval.TotalMilliseconds);
            _timer.Elapsed += _timer_Elapsed;
        }

        #endregion

        #region Properties

        public ICommand AddPlanCommand => _addPlanCommandProxy.ViewExecuteCmd;

        public ICommand RemovePlanCommand => _removePlanCommandProxy.ViewExecuteCmd;

        [Import(typeof(IRecordContext<ExtractionPlan>))]
        private IRecordContext<ExtractionPlan> DbService { get; set; }

        #region Plans

        private ObservableCollection<ExtractionPlanModel> _plans;
        public ObservableCollection<ExtractionPlanModel> Plans
        {
            get => _plans;
            private set
            {
                _plans = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region PlanName

        private String _planName = String.Empty;
        public String PlanName
        {
            get => _planName;
            set
            {
                if (value == null)
                {
                    _planName = String.Empty;
                }
                _planName = value.Trim();
                OnPropertyChanged();
            }
        }

        #endregion

        #region SelectedPlan

        private ExtractionPlanModel _selectedPlan;
        public ExtractionPlanModel SelectedPlan
        {
            get => _selectedPlan;
            set
            {
                if (_selectedPlan == value) return;
                _selectedPlan = value;
                if (value != null)
                {
                    SetSelectedItems(value.ExtractItemTokens.ToArray());
                }
                OnPropertyChanged();
            }
        }

        #endregion

        #region ItemsView

        private IEnumerable<ExtractItemAdorner> _items;
        public IEnumerable<ExtractItemAdorner> Items
        {
            get => _items;
            private set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public ICommand StartCommand { get; }

        public ICommand StopCommand { get; }

        public Int32 TotalProgress
        {
            get
            {
                if (_executeItems.Count == 0) return 0;
                Double total = 0;
                foreach (ExtractItemAdorner item in _executeItems.Values)
                {
                    total += item.Progress;
                }
                total = total / _executeItems.Count * 100;
                return (Int32)total;
            }
        }

        #region TotalElapsed

        private TimeSpan _totalElapsed = TimeSpan.Zero;
        public TimeSpan TotalElapsed
        {
            get => _totalElapsed;
            private set
            {
                _totalElapsed = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Pump

        private Pump _pump;
        public Pump Pump
        {
            get => _pump;
            set
            {
                if (_pump != value)
                {
                    _pump = value;
                }
            }
        }

        public String ExtractionId => Pump?.Id;

        #endregion

        #region CanSelect

        private Boolean _canSelect = true;
        public Boolean CanSelect
        {
            get => _canSelect;
            set
            {
                _canSelect = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Protected

        protected override void Closed()
        {
            _proxy.Disconnected -= Proxy_Disconnect;
            _proxy.Disconnect();
        }

        protected override void InitLoad(object parameters)
        {
            //先启动后台服务在创建代理
            LaunchService();

            var items = DbService.Records.ToModels<ExtractionPlan, ExtractionPlanModel>().ToArray();
            Plans = new ObservableCollection<ExtractionPlanModel>(items);

            Pump = parameters as Pump;
            _proxy = CreateProxy();
            Message message = new Message((Int32)ExtractionCode.Init, Pump);
            SendMessage(message);
        }

        #endregion

        #region Private

        #region Extraction

        private void Start()
        {
            ExtractItem[] items = InitStart();
            Message message = new Message((Int32)ExtractionCode.Start, items);
            CanSelect = false;
            SendMessage(message);
        }

        private ExtractItem[] InitStart()
        {
            Reset();
            foreach (ExtractItemAdorner item in Items)
            {
                if (!item.IsChecked) continue;
                _executeItems.Add(item.Target.GUID, item);
            }
            return _executeItems.Values.Select(x=>x.Target).ToArray();
        }

        private void Stop()
        {
            var items = _executeItems.Values.Where(x => x.State == TaskState.Starting || x.State == TaskState.Running);
            foreach (var item in items)
            {
                item.State = TaskState.IsCancellationRequest;
            }
            Message message = new Message((Int32)ExtractionCode.Stop);
            SendMessage(message);
        }

        private String GetServicePath()
        {
            String file = ConfigurationManager.AppSettings["extractionService"];
            if (String.IsNullOrWhiteSpace(file))
            {
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Services\Extraction", ServerExeName);
            }
            else if (!Path.IsPathRooted(file))
            {
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
            }
            return file;
        }

        private void LaunchService()
        {
            String exeFile = GetServicePath();
            ProcessStartInfo info = new ProcessStartInfo(exeFile)
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(info);
            System.Threading.Thread.Sleep(200);
        }

        private void StartCallback(Message e)
        {
            Boolean b = e.GetContent<Boolean>();
            CanSelect = !b;
            if (b)
            {
                _timer.Start();
                var m = new GeneralArgs<KeyValuePair<String, Boolean>>(GeneralKeys.ExtractDeviceStateMsg)
                {
                    Parameters = new KeyValuePair<String, Boolean>(Pump.Id, false)
                };
                MessageAggregation.SendGeneralMsg(m);
            }
        }

        private void ChangeProgress(Message message)
        {
            TaskProgressChangedEventArgs args = message.GetContent<TaskProgressChangedEventArgs>();
            ExtractItemAdorner item = _executeItems[args.TaskId];
            item.State = TaskState.Running;
            item.Progress = args.Progress;
            OnPropertyChanged("TotalProgress");
        }

        private void ChangeState(Message message)
        {
            TaskStateChangedEventArgs args = message.GetContent<TaskStateChangedEventArgs>();
            ExtractItemAdorner item = _executeItems[args.TaskId];
            item.State = args.NewState;
        }

        private void TerminateItem(Message message)
        {
            Framework.Core.Base.ViewModel.TaskTerminateEventArgs args = message.GetContent<Framework.Core.Base.ViewModel.TaskTerminateEventArgs>();
            ExtractItemAdorner item = _executeItems[args.TaskId];
            if (args.IsCompleted)
            {
                item.State = TaskState.Completed;
                Int32.TryParse(args.Message, out Int32 count);
                item.Count = count;
                item.Progress = 1;
            }
            else if (args.IsFailed)
            {
                item.State = TaskState.Failed;
            }
            else
            {
                item.State = TaskState.Cancelled;
            }
        }

        private void Reset()
        {
            _timer.Stop();
            foreach (ExtractItemAdorner item in _executeItems.Values)
            {
                item.Reset();
            }
            _executeItems.Clear();
            TotalElapsed = TimeSpan.Zero;
            OnPropertyChanged("TotalProgress");
        }

        private void SendMessage(Message message)
        {
            if (!_proxy.IsConnected)
            {
                _proxy.Connect();
            }
            _proxy.Send(message);
        }

        private void Proxy_Disconnect(Object sender, EventArgs args)
        {
            Reset();
            CanSelect = true;
        }

        private void Proxy_Received(object sender, Message e)
        {
            ExtractionCode code = (ExtractionCode)e.Code;
            switch (code)
            {
                case ExtractionCode.Init:
                    ExtractItem[] items = e.GetContent<ExtractItem[]>();
                    Items = items.Select(x => new ExtractItemAdorner(x)).ToArray();
                    break;
                case ExtractionCode.Start:
                    StartCallback(e);
                    break;
                case ExtractionCode.ProgressChanged:
                    ChangeProgress(e);
                    break;
                case ExtractionCode.StateChanged:
                    ChangeState(e);
                    break;
                case ExtractionCode.ItemTerminate:
                    TerminateItem(e);
                    break;
                default:
                    break;
            }
        }

        private void _proxy_TaskTernimate(object sender, IsolatedTaskEngine.Common.TaskTerminateEventArgs e)
        {
            _timer.Stop();
            CanSelect = true;
            OnPropertyChanged("TotalProgress");
            var m = new GeneralArgs<KeyValuePair<String, bool>>(GeneralKeys.ExtractDeviceStateMsg)
            {
                Parameters = new KeyValuePair<String, Boolean>(Pump.Id, true)
            };
            MessageAggregation.SendGeneralMsg(m);
            MessageAggregation.SendGeneralMsgToUI<String>(new GeneralArgs<String>("GeneralKeys_TaskCompleteMsg")
            {
                Parameters = ExtractionId
            });
        }

        private void _proxy_ActivatorError(object sender, TaskEnginErrorEventArgs e)
        {
            _timer.Stop();
            CanSelect = true;
        }

        private TaskProxy CreateProxy()
        {
            String name = ConfigurationManager.AppSettings["taskProxy"];
            TaskProxy proxy = new TaskProxy(name);
            proxy.Disconnected += Proxy_Disconnect;
            proxy.Received += Proxy_Received;
            proxy.TaskTerminated += _proxy_TaskTernimate;
            proxy.TaskEngineError += _proxy_ActivatorError;
            return proxy;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (ExtractItemAdorner item in _executeItems.Values)
            {
                if (item.State == TaskState.Running || item.State == TaskState.Starting)
                {
                    item.Elapsed += Interval;
                }
            }
            TotalElapsed += Interval;
        }

        #endregion

        #region Solution

        private void SetSelectedItems(params String[] ids)
        {
            if (Items.Count() == 0) return;
            ClearSelectedItems();
            for (Int32 j = 0; j < ids.Length; j++)
            {
                foreach (ExtractItemAdorner item in Items)
                {
                    if (item.Target.Token == ids[j])
                    {
                        item.IsChecked = true;
                        break;
                    }
                }
            }
        }

        private void ClearSelectedItems()
        {
            foreach (ExtractItemAdorner item in Items)
            {
                item.IsChecked = false;
            }
        }

        public String[] GetSelectedItems()
        {
            return Items?.Where(x => x.IsChecked).Select(x => x.Target.Token).ToArray();
        }

        private String Add()
        {
            String[] tokens = GetSelectedItems();
            if (tokens.Length == 0) return "没有可添加的方案";
            ExtractionPlanModel plan = new ExtractionPlanModel();
            plan.Name = PlanName;
            plan.ExtractItemTokens = tokens;
            if (DbService.Add(plan.Entity))
            {
                Plans.Add(plan);
                SelectedPlan = plan;
                return "添加方案成功";
            }
            return "添加方案失败";
        }

        private String Remove(ExtractionPlanModel item)
        {
            if (DbService.Remove(item.Entity))
            {
                if (SelectedPlan == item)
                {
                    SetSelectedItems(new String[0]);
                }
                Plans.Remove(item);
                return "删除方案成功";
            }
            return "删除方案失败";
        }

        #endregion

        #endregion

        #endregion
    }
}
