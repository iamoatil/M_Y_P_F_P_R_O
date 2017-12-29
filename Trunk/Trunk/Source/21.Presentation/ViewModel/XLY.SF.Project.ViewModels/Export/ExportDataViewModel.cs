using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;
using System.Linq;
using XLY.SF.Framework.Core.Base.CoreInterface;
using ProjectExtend.Context;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.ViewModels.Main.CaseManagement;
using XLY.SF.Project.Plugin.Adapter;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.DataReport;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;


namespace XLY.SF.Project.ViewModels.Export
{
    [Export(ExportKeys.ExportDataViewViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public class ExportDataViewModel : ViewModelBase
    {

        //private readonly IRecordContext<WorkUnit> _dbService;
        [ImportingConstructor]
        public ExportDataViewModel(IRecordContext<WorkUnit> dbService)
        {
            ReportFormsPathCommand = new RelayCommand(ExecuteReportFormsPathCommand);
            BcpPathCommand = new RelayCommand(ExecuteBcpPathCommand);

            StartCommand = new RelayCommand(ExecuteStartCommand, ControlBtn);
            StopCommand = new RelayCommand(ExecuteStopCommand);

            WorkUnit[] cts = dbService.Records.ToArray();
            WorkUnits = new ObservableCollection<WorkUnit>(cts);
            SelctedWorkUnits = WorkUnits.FirstOrDefault();

            _timer = new System.Timers.Timer(Interval.TotalMilliseconds);
            _timer.Elapsed += _timer_Elapsed;

        }
        private bool ControlBtn()
        {
            if (NotDeleteDataChecked == false && DeleteDataChecked == false)
            {
                return false;
            }
            if (UploadCheck == false && ReportCheck == false)
            {
                return false;
            }
            if (ReportCheck && ReportPath == "")
            {
                return false;
            }
            if (UploadCheck && BcpPath == "")
            {
                return false;
            }
            return true;
        }
        //protected override void InitLoad(object parameters)
        //{
        //    DataList = DeviceExternsion.LoadDeviceData(devicePath);
        //}
        private readonly System.Timers.Timer _timer;

        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

        private WorkUnit _SelctedWorkUnits;
        public WorkUnit SelctedWorkUnits
        {
            get => _SelctedWorkUnits;
            set
            {
                _SelctedWorkUnits = value;
                OnPropertyChanged();
            }
        }

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
        private ObservableCollection<DeviceExtractionAdorner> _DevicesItems;
        /// <summary>
        /// 当前案例所有设备
        /// </summary>
        public ObservableCollection<DeviceExtractionAdorner> DevicesItems
        {
            get
            {
                if (_DevicesItems == null)
                {
                    _DevicesItems = new ObservableCollection<DeviceExtractionAdorner>(SystemContext.Instance.CurrentCase.DeviceExtractions.Select(x => new DeviceExtractionAdorner(x)).ToArray());
                    foreach (var item in _DevicesItems)
                    {
                        item.IsChecked = true;
                    }
                }
                return _DevicesItems;

            }
            private set
            {
                _DevicesItems = value;
                OnPropertyChanged();
            }
        }
        private string _SelectReportType;

        public string SelectReportType
        {
            get { return _SelectReportType ?? "HTML"; }
            set
            {
                _SelectReportType = value;
                base.OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _ReportType;
        /// <summary>
        /// 导出报表类型
        /// </summary>
        public ObservableCollection<string> ReportType
        {
            get
            {
                if (_ReportType == null)
                {
                    _ReportType = new ObservableCollection<string> { "HTML", "PDF" };
                }
                return _ReportType;
            }
            set
            {
                _ReportType = value;
                base.OnPropertyChanged();
            }
        }
        private string _SelectUploadType;

        public string SelectUploadType
        {
            get { return _SelectUploadType ?? "蛛网平台"; }
            set
            {
                _SelectUploadType = value;
                base.OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _UploadType;
        /// <summary>
        /// 上传包类型
        /// </summary>
        public ObservableCollection<string> UploadType
        {
            get
            {
                if (_UploadType == null)
                {
                    _UploadType = new ObservableCollection<string> { "蛛网平台", "SIS平台", "109云平台", "高检云平台" };
                }
                return _UploadType;
            }
            set
            {
                _UploadType = value;
                base.OnPropertyChanged();
            }
        }

        private string _ReportPath = "";
        /// <summary>
        /// 报表路径
        /// </summary>
        public string ReportPath
        {
            get { return _ReportPath; }
            set
            {
                _ReportPath = value;
                base.OnPropertyChanged();
            }
        }
        private string _BcpPath = "";
        /// <summary>
        /// bcp路径
        /// </summary>
        public string BcpPath
        {
            get { return _BcpPath; }
            set
            {
                _BcpPath = value;
                base.OnPropertyChanged();
            }
        }


        private bool _ReportCheck = true;
        /// <summary>
        /// 报表选中
        /// </summary>
        public bool ReportCheck
        {
            get { return _ReportCheck; }
            set
            {
                _ReportCheck = value;
                base.OnPropertyChanged();
            }
        }

        private bool _UploadCheck = true;
        /// <summary>
        /// 上传包选中
        /// </summary>
        public bool UploadCheck
        {
            get { return _UploadCheck; }
            set
            {
                _UploadCheck = value;
                base.OnPropertyChanged();
            }
        }
        private bool _DeleteDataChecked = true;
        /// <summary>
        /// 删除数据
        /// </summary>
        public bool DeleteDataChecked
        {
            get { return _DeleteDataChecked; }
            set
            {
                _DeleteDataChecked = value;
                base.OnPropertyChanged();
            }
        }
        private bool _NotDeleteDataChecked = true;
        /// <summary>
        /// 未删除数据
        /// </summary>
        public bool NotDeleteDataChecked
        {
            get { return _NotDeleteDataChecked; }
            set
            {
                _NotDeleteDataChecked = value;
                base.OnPropertyChanged();
            }
        }

        private bool _AllDataChecked = true;
        /// <summary>
        /// 全部数据
        /// </summary>
        public bool AllDataChecked
        {
            get { return _AllDataChecked; }
            set
            {
                _AllDataChecked = value;
                base.OnPropertyChanged();
            }
        }
        private bool _CheckDataChecked;
        /// <summary>
        /// 勾选数据
        /// </summary>
        public bool CheckDataChecked
        {
            get { return _CheckDataChecked; }
            set
            {
                _CheckDataChecked = value;
                base.OnPropertyChanged();
            }
        }
        private bool _MarkingDataChecked;
        /// <summary>
        /// 标记数据
        /// </summary>
        public bool MarkingDataChecked
        {
            get { return _MarkingDataChecked; }
            set
            {
                _MarkingDataChecked = value;
                base.OnPropertyChanged();
            }
        }
        private ObservableCollection<DataExtactionItem> _DeviceListSource = null;
        /// <summary>
        /// 设备列表
        /// </summary>	
        public ObservableCollection<DataExtactionItem> DeviceListSource
        {
            get { return _DeviceListSource; }
            set
            {
                _DeviceListSource = value;
                OnPropertyChanged();
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



        #region Progress

        private Double _progress;
        /// <summary>
        /// 进度
        /// </summary>
        public Double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        private ObservableCollection<DataExtactionItem> _dataList;

        /// <summary>
        /// 数据列表
        /// </summary>	
        public ObservableCollection<DataExtactionItem> DataList
        {
            get { return _dataList; }
            set
            {
                _dataList = value;
                OnPropertyChanged();
            }
        }


        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        [Import(typeof(IPopupWindowService))]
        private IPopupWindowService PopupService { get; set; }
        /// <summary>
        /// 根据时间创建导出文件名,列入：任务-2017-11-30-17-54-41_0
        /// </summary>
        /// <returns></returns>
        private string CreateExportPath(string name)
        {
            return name + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        }
        private void ExecuteReportFormsPathCommand()
        {
            String directory = PopupService.SelectFolderDialog();
            ReportPath = directory != "" ? Path.Combine(directory, CreateExportPath("报表—")) : "";
        }
        private void ExecuteBcpPathCommand()
        {
            String directory = PopupService.SelectFolderDialog();
            BcpPath = directory != "" ? Path.Combine(directory, CreateExportPath("数据包—")) : "";
        }

        private void GetExportData(IList<IDataSource> data, ObservableCollection<DataExtactionItem> treeNodes)
        {
            foreach (var item in treeNodes)
            {
                if (item.TreeNodes.Count() > 0)
                {
                    GetExportData(data, item.TreeNodes);
                }
                if (item.Data != null)
                {
                    data.Add((IDataSource)item.Data);
                }
            }
        }

        private void CreateItemJson(IDataItems items, Type itemType = null)
        {
            if (items == null)
            {
                return;
            }
            foreach (var c in items.GetView(0, -1))
            {

                foreach (var columnVal in DisplayAttributeHelper.FindDisplayAttributes(itemType))
                {
                    string val = string.Empty;
                    var value = columnVal.GetValue(c);
                }
            }
        }
        private void ExecuteStopCommand()
        {
            _timer.Stop();
        }
        /// <summary>
        /// 快速导出
        /// </summary>
        private void ExecuteStartCommand()
        {
            EnumExportState state;
            if (NotDeleteDataChecked && DeleteDataChecked)
                state = EnumExportState.All;
            else
                state = DeleteDataChecked ? EnumExportState.Delete : EnumExportState.NotDelete;

            _timer.Start();
            TotalElapsed = TimeSpan.Zero;
            Progress = 0;
            Task.Factory.StartNew(() =>
            {
                var pluginModules = PluginAdapter.Instance.GetPluginsByType<DataReportModulePluginInfo>(PluginType.SpfReportModule).ToList().ConvertAll(p => (AbstractDataReportModulePlugin)p.Value)
               .ConvertAll(m => m.PluginInfo as DataReportModulePluginInfo).OrderBy(m => m.OrderIndex);
                var reportPlugins = PluginAdapter.Instance.GetPluginsByType<DataReportPluginInfo>(PluginType.SpfReport).ToList().ConvertAll(p => (AbstractDataReportPlugin)p.Value);
                foreach (var p in reportPlugins)   //添加报表模板信息
                {
                    if (p.PluginInfo is DataReportPluginInfo rp)
                    {
                        rp.Modules = pluginModules.Where(m => m != null && m.ReportId == rp.Guid).ToList();
                    }
                }
                Progress = 35;

                List<AbstractDataReportPlugin> stencils = new List<AbstractDataReportPlugin>();
                if (ReportCheck)//选择html模版
                {
                    stencils.Add(reportPlugins.FirstOrDefault(pl => pl.PluginInfo.Name.Contains("Html")));
                }
                if (UploadCheck)//选择bcp模版
                {
                    stencils.Add(reportPlugins.FirstOrDefault(pl => pl.PluginInfo.Name.Contains("BCP")));
                }
                //设备数据源
                IList<IDataSource> dataSource = new List<IDataSource>();
                IList<string> listPath = new List<string>();
                foreach (DeviceExtractionAdorner device in DevicesItems.Where(p => p.IsChecked))
                {
                    dataSource.Clear();
                    foreach (var Externsion in DeviceExternsion.LoadDeviceData(device.Target.Path))
                    {
                        GetExportData(dataSource, Externsion.TreeNodes);
                    }
                    foreach (var st in stencils)
                    {
                        string path = st.PluginInfo.Name.Contains("Html") ? Path.Combine(ReportPath, device.Name + "报表") : Path.Combine(BcpPath, device.Name + "数据包");
                        listPath.Add(path);
                        var destPath = st.Execute(new DataReportPluginArgument()
                        {
                            CurrentPath = device.Target.Path,
                            DataPool = dataSource,
                            ReportModuleName = "Html模板2(Bootstrap)",
                            ReportPath = path,
                            ExportState = state,
                            CollectionInfo = new ExportCollectionInfo()
                            {
                                CaseCode = device.Id,
                                CaseName = device.Name,
                                CaseType = device.Type,
                                //CollectLocation = device.Device.CollectionInfo.CollectLocation,
                                //CollectLocationCode = device.Device.CollectionInfo.CollectLocationCode,
                                //CollectorCertificateCode = device.Device.CollectionInfo.CollectorCertificateCode,
                                //CollectorName = device.Device.CollectionInfo.CollectorName,
                                //CollectTime = device.Device.CollectionInfo.CollectTime

                            },
                            DeviceInfo = new ExportDeviceInfo()
                            {
                                BloothMac = device.Device.DeviceType == EnumDeviceType.Phone ? (device.Device as Domains.Device).BMac : "",
                                IMEI = device.Device.DeviceType == EnumDeviceType.Phone ? (device.Device as Domains.Device).IMEI : "",
                                Name = device.Name
                            }
                        }, null);
                    }
                }
                Progress = 100;
                _timer.Stop();
                foreach (var item in stencils)
                {
                    SystemContext.Instance.AsyncOperation.Post(p =>
                    {
                        if (MessageBox.ShowDialogSuccessMsg("导出成功，是否打开导出文件？"))
                        {
                            foreach (var path in listPath)
                            {
                                System.Diagnostics.Process.Start(path);
                            }
                        }
                        base.CloseView();
                    }, null);
                }
            });
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TotalElapsed += Interval;
        }
        /// <summary>
        /// 选择报表路径
        /// </summary>
        public ICommand ReportFormsPathCommand { get; set; }
        /// <summary>
        /// 快速导出-选择bcp路径
        /// </summary>
        public ICommand BcpPathCommand { get; set; }
        /// <summary>
        /// 导出按钮
        /// </summary>
        public ICommand StartCommand { get; set; }

        /// <summary>
        /// 停止按钮
        /// </summary>
        public ICommand StopCommand { get; set; }

    }

}

