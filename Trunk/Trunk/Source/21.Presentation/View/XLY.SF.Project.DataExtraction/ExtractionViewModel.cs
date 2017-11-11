using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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

        private TaskProxy _proxy;

        private readonly Dictionary<Object, CheckBox> _headers = new Dictionary<Object, CheckBox>();

        #endregion

        #region Constructors

        public ExtractionViewModel()
        {
            LoadedCommand = new RelayCommand(Loaded);
            StartCommand = new RelayCommand(Start, () => Items != null && Items.Any(x => x.IsChecked));
            SelectGroupCommand = new RelayCommand<IEnumerable<Object>>((o) => SelectGroup(o, true));
            UnselectGroupCommand = new RelayCommand<IEnumerable<Object>>((o) => SelectGroup(o, false));
            SelectItemCommand = new RelayCommand<ExtractionItem>(SelectItem);
            HeaderLoadedCommand = new RelayCommand<RoutedEventArgs>(LoadHeader);
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

        public ICommand LoadedCommand { get; }

        public ICommand SelectGroupCommand { get; }

        public ICommand UnselectGroupCommand { get; }

        public ICommand SelectItemCommand { get; }

        public ICommand HeaderLoadedCommand { get; }

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

        #region Args

        private DataExtractionParams _args;
        public DataExtractionParams Args
        {
            get => _args;
            set
            {
                _args = value;
                if (value != null)
                {
                    Items = PluginAdapter.Instance.GetAllExtractItems(value.Pump).Select(x => new ExtractionItem(x)).ToArray();
                }
                OnPropertyChanged();
            }
        }

        #endregion

        #region IsBusy

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

        protected override void LoadCore(Object parameters)
        {
            Args = parameters as DataExtractionParams;
        }

        #endregion

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
            //PluginAdapter.Instance.Initialization(null);
            //LaunchService();
            Items = new ExtractItem[]
            {
                new ExtractItem{GroupName="设备信息", Name="基本信息"},
                new ExtractItem{GroupName="设备信息", Name="安装应用"},
                new ExtractItem{GroupName="设备信息", Name="SIM卡信息"},
                new ExtractItem{GroupName="设备信息", Name="蓝牙"},
                new ExtractItem{GroupName="设备信息", Name="WIFI信息"},
                new ExtractItem{GroupName="基础数据", Name="联系人"},
                new ExtractItem{GroupName="基础数据", Name="微信"},
                new ExtractItem{GroupName="基础数据", Name="QQ"},
                new ExtractItem{GroupName="基础数据", Name="360浏览器"},
                new ExtractItem{GroupName="基础数据", Name="王者荣耀"},
                new ExtractItem{GroupName="基础数据", Name="绝地求生"},
                new ExtractItem{GroupName="基础数据", Name="腾讯新闻"},
                new ExtractItem{GroupName="特殊", Name="QQ"},
                new ExtractItem{GroupName="特殊", Name="360浏览器"},
                new ExtractItem{GroupName="特殊", Name="王者荣耀"},
                new ExtractItem{GroupName="特殊", Name="绝地求生"},
                new ExtractItem{GroupName="特殊", Name="腾讯新闻"},
            }.Select(x => new ExtractionItem(x)
            {
                Count =1025,
                State = TaskState.Idle,
                Elapsed = TimeSpan.FromHours(17.25345)
            }).ToArray();
        }

        private void Start()
        {
            Message message = new Message((Int32)ExtractionCode.Start);
            message.SetContent(Args);
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            //    foreach (var item in Items)
            //    {
            //        item.State = TaskState.Running;
            //        System.Threading.Thread.Sleep(2000);
            //    }
            //});
            //_proxy.Send(message);
            CanSelect = false;
        }

        private void LaunchService()
        {
            Process process = Process.GetProcessesByName("XLY.SF.Project.DeviceExtractionService").FirstOrDefault();
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(@"F:\Source\Workspaces\SPF-PRO\Trunk\Trunk\Source\22.Tools\XLY.SF.Project.DeviceExtractionService\bin\Debug\XLY.SF.Project.DeviceExtractionService.exe")
                {
                    UseShellExecute = false,
                    //CreateNoWindow = true
                };
                process = Process.Start(info);
            }
            String name = ConfigurationManager.AppSettings["taskProxy"];
            _proxy = new TaskProxy(name)
            {
                DisconnectCallback = Disconnect,
                ReceiveCallback = Receive
            };
            _proxy.TaskOver += _proxy_TaskOver;
            _proxy.Init();
        }

        private void Receive(Message message)
        {
            ExtractionCode code = (ExtractionCode)message.Code;
            switch (code)
            {
                case ExtractionCode.Start:
                    if (message.GetContent<Boolean>())
                    {
                    }
                    else
                    {
                    }
                    break;
                case ExtractionCode.ProgressChanged:
                    break;
                default:
                    break;
            }
        }

        private void Disconnect()
        {
            LaunchService();
        }

        private void _proxy_TaskOver(object sender, TaskOverEventArgs e)
        {
            if (e.IsCompleted)
            {
            }
            else if (e.Exception != null)
            {
            }
            CanSelect = true;
        }

        #endregion

        #endregion
    }
}
