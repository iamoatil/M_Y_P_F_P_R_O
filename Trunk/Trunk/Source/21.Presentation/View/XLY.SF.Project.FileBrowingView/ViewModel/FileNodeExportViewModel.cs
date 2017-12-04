/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/28 19:50:21 
 * explain :  
 *
*****************************************************************************/

using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.FileBrowingView.Language;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.FileBrowingView
{
    [Export("ExportKey_ModuleFileNodeExportViewModel", typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FileNodeExportViewModel : ViewModelBase
    {
        public FileNodeExportViewModel()
        {
            CancelOrCloseCommand = new RelayCommand(DoCancelOrCloseCommand);
            OpenPathCommand = new RelayCommand(DoOpenPathCommand);

            AsyncOperation = AsyncOperationManager.CreateOperation(this);
        }

        public RelayCommand CancelOrCloseCommand { get; set; }

        public RelayCommand OpenPathCommand { get; set; }

        private bool _IsExporting = false;

        /// <summary>
        /// 是否正在下载文件
        /// </summary>
        public bool IsExporting
        {
            get => _IsExporting;
            set
            {
                _IsExporting = value;
                OnPropertyChanged();
            }
        }

        private readonly object CountLocker = new object();

        private int _CountSuccess = 0;

        public int CountSuccess
        {
            get => _CountSuccess;
            set
            {
                _CountSuccess = value;
                OnPropertyChanged();
            }
        }

        private int _CountFailure = 0;

        public int CountFailure
        {
            get => _CountFailure;
            set
            {
                _CountFailure = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<FileNodeExportInfo> _LogItems = new ObservableCollection<FileNodeExportInfo>();

        public ObservableCollection<FileNodeExportInfo> LogItems
        {
            get => _LogItems;
            set
            {
                _LogItems = value;
                OnPropertyChanged();
            }
        }

        private void AddLog(string log)
        {
            lock (LogItems)
            {
                LogItems.Add(new FileNodeExportInfo(log));
            }
        }

        private void AddLog(FileBrowingNode fileNode, bool isSuccess)
        {
            lock (LogItems)
            {
                LogItems.Insert(0, new FileNodeExportInfo(fileNode, isSuccess));
            }
        }

        protected override void InitLoad(object parameters)
        {
            base.InitLoad(parameters);

            var param = parameters as FileNodeExportParameter;

            DownloadCts = new CancellationTokenSource();
            DownloadIts = new FileBrowingIAsyncTaskProgress();
            DownloadIts.ExportFileNodeHandle += DownloadIts_ExportFileNodeHandle;

            SavePath = param.SavePath;

            Task.Run(async () =>
            {
                IsExporting = true;
                AddLog(LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_ExportViewModel_Exporting]);

                foreach (var node in param.Nodes)
                {
                    if (!DownloadCts.IsCancellationRequested)
                    {
                        await param.Service.Download(node.Data, SavePath, false, DownloadCts, DownloadIts);
                    }
                }
            }, DownloadCts.Token).ContinueWith((t) =>
            {
                AsyncOperation.Post((tt) =>
                {
                    IsExporting = false;

                    LogItems = new ObservableCollection<FileNodeExportInfo>(LogItems.Where(f => f.IsSuccess == false));
                }, null);
            });
        }

        protected override void Closed()
        {
            AsyncOperation = null;
            DownloadCts = null;
            DownloadIts = null;
            LogItems = null;

            base.Closed();
        }

        /// <summary>
        /// 保存路径
        /// </summary>
        private string SavePath { get; set; }

        /// <summary>
        /// 可以用于异步操作的对象，主要用于非UI线程中替换dispatcher
        /// </summary>
        public AsyncOperation AsyncOperation { get; private set; }

        private CancellationTokenSource DownloadCts { get; set; }

        private FileBrowingIAsyncTaskProgress DownloadIts { get; set; }

        /// <summary>
        /// 文件导出通知
        /// </summary>
        /// <param name="fileNode">导出节点</param>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="localPath">本地保存路径</param>
        private void DownloadIts_ExportFileNodeHandle(FileBrowingNode fileNode, bool isSuccess, string localPath)
        {
            AsyncOperation.Post((t) =>
            {
                lock (CountLocker)
                {
                    if (isSuccess)
                    {
                        CountSuccess++;
                    }
                    else
                    {
                        CountFailure++;
                    }
                }
                AddLog(fileNode, isSuccess);
            }, null);
        }

        private void DoCancelOrCloseCommand()
        {
            if (IsExporting)
            {//取消导出
                DownloadCts?.Cancel();
            }
            else
            {//关闭
                base.CloseView();
            }
        }

        private void DoOpenPathCommand()
        {
            System.Diagnostics.Process.Start("explorer.exe ", SavePath);
        }
    }

    /// <summary>
    /// 文件导出信息
    /// </summary>
    public class FileNodeExportInfo
    {
        public FileNodeExportInfo(string str)
        {
            LogStr = str;
            IsSuccess = null;
            InfoType = FileNodeExportInfoType.StringInfo;
        }

        public FileNodeExportInfo(FileBrowingNode fileNode, bool isSuccess)
        {
            LogStr = fileNode.Name;
            IsSuccess = isSuccess;
            InfoType = FileNodeExportInfoType.ExportInfo;
        }

        public string LogStr { get; set; }

        public bool? IsSuccess { get; set; }

        public FileNodeExportInfoType InfoType { get; set; }

    }

    /// <summary>
    /// 文件导出信息类型
    /// </summary>
    public enum FileNodeExportInfoType
    {
        /// <summary>
        /// 普通字符串
        /// </summary>
        StringInfo,
        /// <summary>
        /// 导出信息
        /// </summary>
        ExportInfo
    }

    /// <summary>
    /// 文件导出参数
    /// </summary>
    internal class FileNodeExportParameter
    {
        /// <summary>
        /// 文件浏览服务
        /// </summary>
        public AbsFileBrowsingService Service { get; set; }

        /// <summary>
        /// 下载列表
        /// </summary>
        public FileBrowingTreeNode[] Nodes { get; set; }

        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; set; }

    }
}
