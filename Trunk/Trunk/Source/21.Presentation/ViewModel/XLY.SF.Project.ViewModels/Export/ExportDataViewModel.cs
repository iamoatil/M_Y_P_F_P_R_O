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

    public class ExportDataViewModel: ViewModelBase
    {

        //private readonly IRecordContext<WorkUnit> _dbService;
        [ImportingConstructor]
        public ExportDataViewModel(IRecordContext<WorkUnit> dbService) {
            FastReportFormsPathCommand = new RelayCommand(ExecuteFastReportFormsPathCommand);
            FastBcpPathCommand = new RelayCommand(ExecuteFastBcpPathCommand);
            SeniorReportFormsPathCommand = new RelayCommand(ExecuteSeniorReportFormsPathCommand);
            SeniorBcpPathCommand = new RelayCommand(ExecuteSeniorBcpPathCommand);

            FastStartCommand = new RelayCommand(ExecuteFastStartCommand);
            FastStopCommand=new RelayCommand(ExecuteFastStopCommand);

            FastSeniorCommand = new RelayCommand<bool>(ExecuteFastSeniorCommand);
            ; 
            WorkUnit[] cts = dbService.Records.ToArray();
            WorkUnits = new ObservableCollection<WorkUnit>(cts);
            SelctedWorkUnits = WorkUnits.FirstOrDefault();

            _timer = new System.Timers.Timer(Interval.TotalMilliseconds);
            _timer.Elapsed += _timer_Elapsed;

        }
        private void ExecuteFastSeniorCommand(bool obj)
        {
            if (obj)
            {
                DeviceExportReport tmp = null;
                //DataList = DeviceExternsion.LoadDeviceData(SelectDevice.Target.Path);
                foreach (var item in DevicesItems)
                {
                    tmp = new DeviceExportReport();
                    tmp.Path=item.Target.Path;
                    tmp.Name = item.Name;
                    tmp.IsFirstStyle = true;
                    foreach (var ExtractItem in item.Target.ExtractItems)
                    {
                        tmp.TreeNodes.Add(new DeviceExportReport() { Name= ExtractItem .Mode});
                    }
                    //item.Target.ExtractItems
                }
            }
            //var tmp = obj  as Boolean;      
            //var tmp = obj as System.Windows.Controls.CheckBox;
            //if (tmp.Checked)
            //{
            //    DataList = DeviceExternsion.LoadDeviceData(SelectDevice.Target.Path);
            //}
        }
        //protected override void InitLoad(object parameters)
        //{
        //    DataList = DeviceExternsion.LoadDeviceData(devicePath);
        //}
        private readonly System.Timers.Timer _timer;

        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);


        private ObservableCollection<DeviceExportReport> _DeviceList;
        /// <summary>
        /// 高级导出——设备列表
        /// </summary>	
        public ObservableCollection<DeviceExportReport> DeviceList
        {
            get { return _DeviceList ?? new ObservableCollection<DeviceExportReport>(); }
            set
            {
                _DeviceList = value;
                OnPropertyChanged();
            }
        }

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
        private DeviceExtractionAdorner _SelectDevice;

        public DeviceExtractionAdorner SelectDevice
        {
            get { return _SelectDevice; }
            set {
                //var dataList = DeviceExternsion.LoadDeviceData(devicePath);
                _SelectDevice = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<DeviceExtractionAdorner> _DevicesItems;
        /// <summary>
        /// 当前案例所有设备
        /// </summary>
        public ObservableCollection<DeviceExtractionAdorner> DevicesItems
        {
            get {
                if (_DevicesItems == null)
                {
                    _DevicesItems = new ObservableCollection<DeviceExtractionAdorner>(SystemContext.Instance.CurrentCase.DeviceExtractions.Select(x => new DeviceExtractionAdorner(x)).ToArray());
                    SelectDevice= _DevicesItems.FirstOrDefault();
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
            get { return _SelectReportType??"HTML"; }
            set {
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
            get {
                if (_ReportType == null)
                {
                    _ReportType = new ObservableCollection<string> { "HTML","PDF"};
                }
                return _ReportType;
            }
            set {
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

        private string _FastReportPath;
        /// <summary>
        /// 快速报表路径
        /// </summary>
        public string FastReportPath
        {
            get { return _FastReportPath; }
            set {
                _FastReportPath = value;
                base.OnPropertyChanged();
            }
        }
        private string _FastBcpPath;
        /// <summary>
        /// 快速bcp路径
        /// </summary>
        public string FastBcpPath
        {
            get { return _FastBcpPath; }
            set
            {
                _FastBcpPath = value;
                base.OnPropertyChanged();
            }
        }
        private string _SeniorReportPath;
        /// <summary>
        /// 高级报表路径
        /// </summary>
        public string SeniorReportPath
        {
            get { return _SeniorReportPath; }
            set
            {
                _SeniorReportPath = value;
                base.OnPropertyChanged();
            }
        }
        private string _SeniorBcpPath;
        /// <summary>
        /// 高级报表路径
        /// </summary>
        public string SeniorBcpPath
        {
            get { return _SeniorBcpPath; }
            set
            {
                _SeniorBcpPath = value;
                base.OnPropertyChanged();
            }
        }

        private bool _FastReportCheck=true;
        /// <summary>
        /// 快速导出-报表选中
        /// </summary>
        public bool FastReportCheck
        {
            get { return _FastReportCheck; }
            set {
                _FastReportCheck = value;
                base.OnPropertyChanged();
            }
        }

        private bool _FastUploadCheck=false;
        /// <summary>
        /// 快速导出-上传包选中
        /// </summary>
        public bool FastUploadCheck
        {
            get { return _FastUploadCheck; }
            set
            {
                _FastUploadCheck = value;
                base.OnPropertyChanged();
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



   
        //private Boolean? _isSelectAll;
        //public Boolean? IsSelectAll
        //{
        //    get
        //    {
        //        if (_headers.Count == 0) return false;
        //        if (_headers.Values.All(x => x.IsChecked.HasValue && x.IsChecked.Value))
        //        {
        //            return true;
        //        }
        //        else if (_headers.Values.All(x => x.IsChecked.HasValue && !x.IsChecked.Value))
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    set
        //    {
        //        _isSelectAll = value;
        //        OnPropertyChanged();
        //        SelectAll(value.Value);
        //    }
        //}

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
        private string CreateExportPath() {
            return "任务-"+DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        }
        private void ExecuteFastReportFormsPathCommand()
        {
            String directory = PopupService.SelectFolderDialog();
            FastReportPath = Path.Combine(directory, CreateExportPath());
            //OnPropertyChanged("CaseInfo.Path");
        }
        private void ExecuteFastBcpPathCommand()
        {
            String directory = PopupService.SelectFolderDialog();
            FastBcpPath = Path.Combine(directory, CreateExportPath()); 
        }
        private void ExecuteSeniorReportFormsPathCommand()
        {
            String directory = PopupService.SelectFolderDialog();
            SeniorReportPath = directory;
        }
        private void ExecuteSeniorBcpPathCommand()
        {
            String directory = PopupService.SelectFolderDialog();
            SeniorBcpPath = directory;
        }
        private void GetExportData(IList<IDataSource> data, ObservableCollection<DataExtactionItem> treeNodes) {
            foreach (var item in treeNodes)
            {
                if (item.TreeNodes.Count()>0)
                {
                    GetExportData(data, item.TreeNodes);
                }
                if (item.Data!=null)
                {
                    data.Add((IDataSource)item.Data);
                }
       
            }
        }
        /// <summary>
        /// 快速导出
        /// </summary>
        private void ExecuteFastStartCommand()
        {

            if (FastReportCheck == false && FastUploadCheck == false)
            {
                MessageBox.ShowWarningMsg("请勾选至少一项需要导出的数据！");
                return;
            } else if (FastReportCheck) {
                if (FastReportPath=="")
                {
                    MessageBox.ShowWarningMsg("请选择报表导出的路径！");
                    return;
                }
            } else if (FastUploadCheck) {
                if (FastBcpPath == "")
                {
                    MessageBox.ShowWarningMsg("请选择上传包导出的路径！");
                    return;
                }
            }
            _timer.Start();
            TotalElapsed = TimeSpan.Zero;
            Progress = 100;
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
                //设备数据源
                IList<IDataSource> dataSource = new List<IDataSource>();
                foreach (var item in DeviceExternsion.LoadDeviceData(SelectDevice.Target.Path))
                {
                    GetExportData(dataSource, item.TreeNodes);
                }
                List<AbstractDataReportPlugin> items = new List<AbstractDataReportPlugin>();
                if (FastReportCheck)//选择html模版
                {
                    items.Add(reportPlugins.FirstOrDefault(pl => pl.PluginInfo.Name.Contains("Html")));
                }
                if (FastUploadCheck)//选择bcp模版
                {
                    items.Add(reportPlugins.FirstOrDefault(pl => pl.PluginInfo.Name.Contains("BCP")));
                }
                foreach (var item in items)
                {
                    //执行导出操作
                    var destPath = item.Execute(new DataReportPluginArgument()
                    {
                        DataPool = dataSource,
                        ReportModuleName = "Html模板2(Bootstrap)",
                        ReportPath = item.PluginInfo.Name.Contains("Html") ? FastReportPath : FastBcpPath,
                        CollectionInfo = new ExportCollectionInfo()
                        {
                            CaseCode = SelectDevice.Id,
                            CaseName = SelectDevice.Name,
                            CaseType = SelectDevice.Type,
                            //CollectLocation = (SelectDevice.Device as Domains.Device).CollectionInfo.CollectLocation,
                            //CollectLocationCode = (SelectDevice.Device as Domains.Device).CollectionInfo.CollectLocationCode,
                            //CollectorCertificateCode = (SelectDevice.Device as Domains.Device).CollectionInfo.CollectorCertificateCode,
                            //CollectorName = (SelectDevice.Device as Domains.Device).CollectionInfo.CollectorName,
                            //CollectTime = (SelectDevice.Device as Domains.Device).CollectionInfo.CollectTime

                        },
                        DeviceInfo = new ExportDeviceInfo()
                        {
                            BloothMac = (SelectDevice.Device as Domains.Device).BMac,
                            IMEI = (SelectDevice.Device as Domains.Device).IMEI,
                            Name = (SelectDevice.Device as Domains.Device).Name
                        }
                    }, null);
                    
                }
                _timer.Stop();
                
                //MessageBox.ShowSuccessMsg("导出成功");
            });
        }
        private void ExecuteFastStopCommand()
        {
            _timer.Stop();
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TotalElapsed += Interval;
        }
        /// <summary>
        /// 快速导出-选择报表路径
        /// </summary>
        public ICommand FastReportFormsPathCommand { get; set; }
        /// <summary>
        /// 快速导出-选择bcp路径
        /// </summary>
        public ICommand FastBcpPathCommand { get; set; }
        /// <summary>
        /// 该户导出-选择报表路径
        /// </summary>
        public ICommand SeniorReportFormsPathCommand { get; set; }
        /// <summary>
        /// 快速导出-选择报表路径
        /// </summary>
        public ICommand SeniorBcpPathCommand { get; set; }

        /// <summary>
        /// 快速开始导出
        /// </summary>
        public ICommand FastStartCommand { get; set; }
        /// <summary>
        /// 快速停止导出
        /// </summary>
        public ICommand FastStopCommand { get; set; }

        /// <summary>
        /// 快速高级切换
        /// </summary>
        public ICommand FastSeniorCommand { get; set; }
    }

}

