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

            WorkUnit[] cts = dbService.Records.ToArray();
            WorkUnits = new ObservableCollection<WorkUnit>(cts);
            SelctedWorkUnits = WorkUnits.FirstOrDefault();

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
                _SelectDevice = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<DeviceExtractionAdorner> _items;
        /// <summary>
        /// 当前案例所有设备
        /// </summary>
        public ObservableCollection<DeviceExtractionAdorner> Items
        {
            get {
                if (_items == null)
                {
                    _items= new ObservableCollection<DeviceExtractionAdorner>(SystemContext.Instance.CurrentCase.DeviceExtractions.Select(x => new DeviceExtractionAdorner(x)).ToArray());
                    SelectDevice= _items.FirstOrDefault();
                }
                return _items;       

            }
            private set
            {
                _items = value;
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
        [Import(typeof(IPopupWindowService))]
        private IPopupWindowService PopupService { get; set; }

        private void ExecuteFastReportFormsPathCommand()
        {
            String directory = PopupService.SelectFolderDialog();
            FastReportPath = directory;
            //OnPropertyChanged("CaseInfo.Path");
        }
        private void ExecuteFastBcpPathCommand()
        {
            String directory = PopupService.SelectFolderDialog();
            FastBcpPath = directory;
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
        private void ExecuteFastStartCommand()
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
            var aa = DeviceExternsion.LoadDeviceData(SelectDevice.Target.Path);
            foreach (var item in aa)
            {
                dataSource.Add((IDataSource)item.Data);
            }    

            var destPath = reportPlugins.FirstOrDefault(pl => pl.PluginInfo.Name.Contains("Html报表")).Execute(new DataReportPluginArgument()
            {
                DataPool = dataSource,
                ReportModuleName = "Html模板2(Bootstrap)",
                ReportPath ="",
                CollectionInfo = new ExportCollectionInfo()
                {
                    CaseCode = "1244",
                    CaseName = "杀入按",
                    CaseType = "抢劫",
                    CollectLocation = "环球中心",
                    CollectLocationCode = "610000",
                    CollectorCertificateCode = "CollectorCertificateCode",
                    CollectorName = "by",
                    CollectTime = "2012-3-2"
                },
                DeviceInfo = new ExportDeviceInfo()
                {
                    BloothMac = "29:21:23:42:13:d9",
                    IMEI = "2343353453454",
                    Name = "fsdfi"
                }
            }, null);
        }
        private void ExecuteFastStopCommand()
        {

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
    }

}

