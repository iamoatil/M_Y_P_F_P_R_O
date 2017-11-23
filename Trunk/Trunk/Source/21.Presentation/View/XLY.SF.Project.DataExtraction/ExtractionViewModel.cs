using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.DataExtract;
using XLY.SF.Project.Domains;
using XLY.SF.Project.IsolatedTaskEngine.Common;
using XLY.SF.Project.Plugin.Adapter;

namespace XLY.SF.Project.DataExtraction
{
    public class ExtractionViewModel : ViewModelBase
    {
        #region Fields

        private const String ServerExeName = "XLY.SF.Project.DeviceExtractionService.exe";

        private TaskProxy _proxy;

        private readonly Dictionary<Object, CheckBox> _headers = new Dictionary<Object, CheckBox>();

        private readonly Dictionary<String, ExtractionItem> _executeItems = new Dictionary<String, ExtractionItem>();

        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

        private readonly Timer _timer;

        #endregion

        #region Constructors

        public ExtractionViewModel()
        {
            LoadedCommand = new RelayCommand(Loaded);
            StartCommand = new RelayCommand(Start, () => Items != null && Items.Any(x => x.IsChecked));
            StopCommand = new RelayCommand(Stop);
            SelectGroupCommand = new RelayCommand<IEnumerable<Object>>((o) => SelectGroup(o, true));
            UnselectGroupCommand = new RelayCommand<IEnumerable<Object>>((o) => SelectGroup(o, false));
            SelectItemCommand = new RelayCommand<ExtractionItem>(SelectItem);
            HeaderLoadedCommand = new RelayCommand<RoutedEventArgs>(LoadHeader);
            IsSelfHost = false;
            _timer = new Timer(Interval.TotalMilliseconds);
            _timer.Elapsed += _timer_Elapsed;
        }

        #endregion

        #region Properties

        #region ItemsView

        private IEnumerable<ExtractionItem> _items;
        public IEnumerable<ExtractionItem> Items
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

        public ICommand LoadedCommand { get; }

        public ICommand SelectGroupCommand { get; }

        public ICommand UnselectGroupCommand { get; }

        public ICommand SelectItemCommand { get; }

        public ICommand HeaderLoadedCommand { get; }

        public Int32 TotalProgress
        {
            get
            {
                if (_executeItems.Count == 0) return 0;
                Double total = 0;
                foreach (ExtractionItem item in _executeItems.Values)
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

        #region IsSelfHost

        private Boolean _isSelfHost;
        internal Boolean IsSelfHost
        {
            get => _isSelfHost;
            set
            {
                _isSelfHost = value;
                //if (value)
                //{
                //    MessageAggregation.UnRegisterMsg<GeneralArgs<Pump>>(this, "SetDataExtractionParamsMsg", SetPump);
                //}
                //else
                //{
                //    MessageAggregation.RegisterGeneralMsg<Pump>(this, "SetDataExtractionParamsMsg", SetPump);
                //}
            }
        }

        #endregion

        #region IsSelectAll

        private Boolean? _isSelectAll;
        public Boolean? IsSelectAll
        {
            get
            {
                if (_headers.Count == 0) return false;
                if (_headers.Values.All(x => x.IsChecked.HasValue && x.IsChecked.Value))
                {
                    return true;
                }
                else if (_headers.Values.All(x => x.IsChecked.HasValue && !x.IsChecked.Value))
                {
                    return false;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _isSelectAll = value;
                OnPropertyChanged();
                SelectAll(value.Value);
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
                    if (value != null)
                    {
                        Items = PluginAdapter.Instance.GetAllExtractItems(value).Select(x => new ExtractionItem(x)).ToArray();
                    }
                    else
                    {
                        Items = null;
                    }
                }
            }
        }

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

        #region Private

        private void LoadHeader(RoutedEventArgs args)
        {
            CheckBox cb = (CheckBox)args.Source;
            CollectionViewGroup group = (CollectionViewGroup)cb.DataContext;
            if (!_headers.ContainsKey(group.Name))
            {
                _headers.Add(group.Name, cb);
            }
        }

        private void SelectAll(Boolean isChecked)
        {
            foreach (CheckBox cb in _headers.Values)
            {
                cb.IsChecked = isChecked;
            }
        }

        private void SelectGroup(IEnumerable<Object> items, Boolean isChecked)
        {
            Object groupName = null;
            foreach (ExtractionItem item in items.OfType<ExtractionItem>())
            {
                item.IsChecked = isChecked;
                if (groupName == null)
                {
                    groupName = item.Group;
                }
            }
            if (groupName != null)
            {
                _headers[groupName].IsChecked = isChecked;
            }
            OnPropertyChanged("IsSelectAll");
        }

        private void SelectItem(ExtractionItem o)
        {
            CheckBox cb = _headers[o.Group];
            if (cb.IsChecked.HasValue)
            {
                cb.IsChecked = null;
            }
            else
            {
                var temp = Items.Where(x => x.Group == o.Group);
                if (temp.All(x => x.IsChecked))
                {
                    cb.IsChecked = true;
                }
                else if (temp.All(x => !x.IsChecked))
                {
                    cb.IsChecked = false;
                }
            }
            OnPropertyChanged("IsSelectAll");
        }

        private void Loaded()
        {
            LaunchService();
        }

        protected override void InitLoad(object parameters)
        {
            Pump = parameters as Pump;
        }

        //private void SetPump(GeneralArgs<Pump> args)
        //{
        //    Pump = args.Parameters;
        //}

        private void Start()
        {
            ExtractItem[] items = InitStart();
            Message message = new Message((Int32)ExtractionCode.Start);
            DataExtractionParams @params = new DataExtractionParams(Pump, items);
            message.SetContent(@params);
            CanSelect = false;
            _proxy.Send(message);
            _timer.Start();
        }

        private ExtractItem[] InitStart()
        {
            _proxy = CreateProxy();
            _executeItems.Clear();
            OnPropertyChanged("TotalProgress");
            TotalElapsed = TimeSpan.Zero;
            List<ExtractItem> eis = new List<ExtractItem>();
            foreach (ExtractionItem item in Items)
            {
                item.Reset();
                if (!item.IsChecked) continue;
                eis.Add(item.Target);
                _executeItems.Add(item.Target.GUID, item);
            }
            return eis.ToArray();
        }

        private void Stop()
        {
            var items = _executeItems.Values.Where(x => x.State == TaskState.Starting || x.State == TaskState.Running);
            foreach (var item in items)
            {
                item.State = TaskState.Stopping;
            }
            Message message = new Message((Int32)ExtractionCode.Stop);
            _proxy.Send(message);
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
        }

        private void Receive(Message message)
        {
            ExtractionCode code = (ExtractionCode)message.Code;
            switch (code)
            {
                case ExtractionCode.Start:
                    CanSelect = !message.GetContent<Boolean>();
                    break;
                case ExtractionCode.Stop:
                    break;
                case ExtractionCode.ProgressChanged:
                    ChangeProgress(message);
                    break;
                case ExtractionCode.Terminate:
                    Terminate(message);
                    break;
                default:
                    break;
            }
        }

        private void ChangeProgress(Message message)
        {
            TaskProgressEventArgs args = message.GetContent<TaskProgressEventArgs>();
            ExtractionItem item = _executeItems[args.TaskId];
            item.Progress = args.Progress;
            if (args.Progress == 0)
            {
                item.State = TaskState.Starting;
            }
            else
            {
                item.State = TaskState.Running;
            }
            OnPropertyChanged("TotalProgress");
        }

        private void Terminate(Message message)
        {
            TaskTerminateEventArgs args = message.GetContent<TaskTerminateEventArgs>();
            ExtractionItem item = _executeItems[args.TaskId];
            if (args.IsCompleted)
            {
                item.State = TaskState.Completed;
            }
            else if (args.IsFailed)
            {
                item.State = TaskState.Failed;
            }
            else
            {
                item.State = TaskState.Stopped;
            }
        }

        private void Disconnect()
        {
            LaunchService();
        }

        private void _proxy_TaskOver(object sender, TaskOverEventArgs e)
        {
            _timer.Stop();
            CanSelect = true;
            MessageAggregation.SendGeneralMsg<Boolean>(new GeneralArgs<Boolean>("ExtractTaskCompleteMsg")
            {
                Parameters = !e.IsFailed
            });
        }

        private void _proxy_ActivatorError(object sender, ActivatorErrorEventArgs e)
        {
            _timer.Stop();
            CanSelect = true;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Window win = new Window();
                win.Width = 400;
                win.Height = 300;
                win.Content = e.Exception.ToString();
                win.ShowDialog();
            });
        }

        private TaskProxy CreateProxy()
        {
            String name = ConfigurationManager.AppSettings["taskProxy"];
            TaskProxy proxy = new TaskProxy(name)
            {
                DisconnectCallback = Disconnect,
                ReceiveCallback = Receive
            };
            proxy.TaskOver += _proxy_TaskOver;
            proxy.ActivatorError += _proxy_ActivatorError;
            proxy.Init();
            return proxy;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (ExtractionItem item in _executeItems.Values)
            {
                if (item.State == TaskState.Running || item.State == TaskState.Starting)
                {
                    item.Elapsed += Interval;
                }
            }
            TotalElapsed += Interval;
        }

        #endregion

        #endregion
    }
}
