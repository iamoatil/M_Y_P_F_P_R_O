/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/22 13:48:50 
 * explain :  
 *
*****************************************************************************/

using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Contract;
using XLY.SF.Project.FileBrowingView.Language;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.FileBrowingView
{
    [Export("ExportKey_ModuleFileBowingViewModel", typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FileBrowingViewModel : ViewModelBase
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// 文件浏览服务
        /// </summary>
        public AbsFileBrowsingService Service { get; set; }

        /// <summary>
        /// 可以用于异步操作的对象，主要用于非UI线程中替换dispatcher
        /// </summary>
        public AsyncOperation AsyncOperation { get; private set; }

        /// <summary>
        /// 消息服务
        /// </summary>
        private IMessageBox MessageBox { get; set; }

        private IPopupWindowService PopupWindowService { get; set; }

        public FileBrowingViewModel()
        {
            OpenFileNodeCommand = new RelayCommand<FileBrowingTreeNode>(OnOpenFileNode);
            DownFileNodeCommand = new RelayCommand<FileBrowingTreeNode>(OnDownFileNode);
            ClickCheckCommand = new RelayCommand<FileBrowingTreeNode>(OnClickCheckCommand);
            SelecedNodeCommand = new RelayCommand<FileBrowingTreeNode>(OnSelecedNodeCommand);
            DownSelectedFileNodeCommand = new RelayCommand(OnDownSelectedFileNode);
            SearchFileNodeCommand = new RelayCommand(OnSearchFileNode);
            ClearSearchCommand = new RelayCommand(OnClearSearch);
            FilePreviewCommand = new RelayCommand<FileBrowingTreeNode>(FilePreview);

            DataStateSource = new Dictionary<EnumDataState, string>()
            {
                { EnumDataState.None, LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_Search_DataStateAll] },
                { EnumDataState.Normal,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_Search_DataStateNormal] },
                { EnumDataState.Deleted ,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_Search_DataStateDelete] }
            };
            FileTypeSource = new Dictionary<EnumFileType, string>()
            {
                {EnumFileType.All,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_FileType_All] },
                {EnumFileType.Txt,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_FileType_Txt] },
                {EnumFileType.Image,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_FileType_Image] },
                {EnumFileType.Voice,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_FileType_Voice] },
                {EnumFileType.Video,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_FileType_Video] },
                {EnumFileType.Rar,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_FileType_Rar] },
                {EnumFileType.DB,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_FileType_DB] },
                {EnumFileType.Other,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_FileType_Other] }
            };
            KeywordTypeSource = new Dictionary<int, string>()
            {
                {0,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_Search_Keyword] },
                {1,LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_Search_ZhengZe] },
            };

            AsyncOperation = AsyncOperationManager.CreateOperation(this);

            MessageBox = Framework.Core.Base.MefIoc.IocManagerSingle.Instance.GetPart<IMessageBox>();
            PopupWindowService = Framework.Core.Base.MefIoc.IocManagerSingle.Instance.GetPart<IPopupWindowService>();
        }

        protected override void InitLoad(object parameters)
        {
            base.InitLoad(parameters);

            Source = parameters;

            //初始化服务
            LoadingData(async () =>
            {
                Service = FileBrowsingServiceFactory.GetFileBrowsingService(Source);
                FileBrowingNode ss = await Service.GetRootNode();

                Roots = new List<FileBrowingTreeNode>() { new FileBrowingTreeNode(ss) };
                CurFileBrowingTreeNode = Roots[0];
            });
        }

        #region 界面绑定命令

        public RelayCommand<FileBrowingTreeNode> SelecedNodeCommand { get; set; }

        public RelayCommand<FileBrowingTreeNode> OpenFileNodeCommand { get; set; }

        public RelayCommand<FileBrowingTreeNode> DownFileNodeCommand { get; set; }

        public RelayCommand<FileBrowingTreeNode> ClickCheckCommand { get; set; }

        public RelayCommand<FileBrowingTreeNode> FilePreviewCommand { get; set; }

        public RelayCommand DownSelectedFileNodeCommand { get; set; }

        public RelayCommand SearchFileNodeCommand { get; set; }

        public RelayCommand ClearSearchCommand { get; set; }

        #endregion

        #region 界面绑定数据

        private List<FileBrowingTreeNode> _Roots;

        /// <summary>
        /// 文件树根节点
        /// 左边的树控件绑定
        /// </summary>
        public List<FileBrowingTreeNode> Roots
        {
            get => _Roots;
            set
            {
                _Roots = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<FileBrowingTreeNode> _TableItems;

        /// <summary>
        /// 表格数据
        /// 右边的数据列表绑定
        /// </summary>
        public ObservableCollection<FileBrowingTreeNode> TableItems
        {
            get => _TableItems;
            set
            {
                _TableItems = value;
                OnPropertyChanged();
            }
        }

        private FileBrowingTreeNode _CurFileBrowingTreeNode;

        /// <summary>
        /// 当前选择文件夹
        /// </summary>
        public FileBrowingTreeNode CurFileBrowingTreeNode
        {
            get => _CurFileBrowingTreeNode;
            set
            {
                _CurFileBrowingTreeNode = value;
                OnPropertyChanged();
            }
        }

        private bool _IsTableSelectAll;

        /// <summary>
        /// 表格是否全选
        /// </summary>
        public bool IsTableSelectAll
        {
            get => _IsTableSelectAll;
            set
            {
                _IsTableSelectAll = value;
                OnPropertyChanged();

                if (null != TableItems)
                {
                    if (value)
                    {
                        TableItems.ForEach(s => s.IsSelected = true);
                    }
                    else
                    {
                        TableItems.ForEach(s => s.IsSelected = false);
                    }
                }
            }
        }

        private bool _IsLoading = false;

        /// <summary>
        /// 是否正在加载数据
        /// </summary>
        public bool IsLoading
        {
            get => _IsLoading;
            set
            {
                _IsLoading = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region 文件浏览

        /// <summary>
        /// 加载节点数据
        /// </summary>
        /// <param name="doSomething"></param>
        public async void LoadingData(Action doSomething)
        {
            IsLoading = true;
            await Task.Run(doSomething).ContinueWith((t) => IsLoading = false);
        }

        /// <summary>
        /// 打开文件节点
        /// 可以是单个文件夹 也可以是单个文件
        /// 如果是单个文件夹，进入该文件夹
        /// 如果是单个文件，下载该文件
        /// </summary>
        /// <param name="node"></param>
        public void OnOpenFileNode(FileBrowingTreeNode node)
        {
            if (node.Data.IsFile)
            {//文件
                Download(node);
            }
            else
            {//文件夹
                OpenFileFolderNode(node);
            }
        }

        /// <summary>
        /// 标记 表格用事件
        /// </summary>
        /// <param name="node"></param>
        public void OnClickCheckCommand(FileBrowingTreeNode node)
        {
            if (null != TableItems)
            {
                _IsTableSelectAll = TableItems.All(i => i.IsSelected);
                OnPropertyChanged(nameof(IsTableSelectAll));
            }
        }

        /// <summary>
        /// 选中文件夹或者文件
        /// 单击事件  视图界面事件
        /// </summary>
        /// <param name="node"></param>
        public void OnSelecedNodeCommand(FileBrowingTreeNode node)
        {
            node.IsSelected = !node.IsSelected;
            _IsTableSelectAll = TableItems.All(i => i.IsSelected);
            OnPropertyChanged(nameof(IsTableSelectAll));

            FilePreview(node);
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="node"></param>
        private void OpenFileFolderNode(FileBrowingTreeNode node)
        {
            if (null == node || node.Data.IsFile)
            {
                return;
            }

            CurFileBrowingTreeNode = node;

            if (node.AllChildrenNodes.IsInvalid())
            {
                LoadingData(async () =>
                {
                    var ss = await Service.GetChildNodes(node.Data);
                    node.AllChildrenNodes = new List<FileBrowingTreeNode>(ss.Select(f => new FileBrowingTreeNode(f)));
                    node.ChildrenTreeNodes = new List<FileBrowingTreeNode>(ss.Where(f => !f.IsFile).Select(f => new FileBrowingTreeNode(f)));

                    TableItems = new ObservableCollection<FileBrowingTreeNode>(node.AllChildrenNodes);
                    IsTableSelectAll = TableItems.All(s => s.IsSelected);
                });
            }
            else
            {
                TableItems = new ObservableCollection<FileBrowingTreeNode>(node.AllChildrenNodes);
                IsTableSelectAll = TableItems.All(s => s.IsSelected);
            }
        }

        #endregion

        #region 文件预览

        private EnumFilePreviewState _FilePreviewState = EnumFilePreviewState.None;

        /// <summary>
        /// 文件预览状态
        /// </summary>
        public EnumFilePreviewState FilePreviewState
        {
            get => _FilePreviewState;
            set
            {
                if (value == _FilePreviewState)
                {
                    return;
                }

                _FilePreviewState = value;
                OnPropertyChanged();

                HasFilePreview = value == EnumFilePreviewState.Show;
            }
        }

        private string _FilePreviewErrorMsg;

        /// <summary>
        /// 文件预览错误提示消息
        /// </summary>
        public string FilePreviewErrorMsg
        {
            get => _FilePreviewErrorMsg;
            set
            {
                _FilePreviewErrorMsg = value;
                OnPropertyChanged();
            }
        }

        private bool _HasFilePreview = false;

        /// <summary>
        /// 是否显示文件预览
        /// </summary>
        public bool HasFilePreview
        {
            get => _HasFilePreview;
            set
            {
                if (value == _HasFilePreview)
                {
                    return;
                }

                _HasFilePreview = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 当前预览的节点
        /// </summary>
        private FileBrowingTreeNode CurFilePreviewNode = null;

        /// <summary>
        /// 文件预览
        /// </summary>
        /// <param name="node"></param>
        private void FilePreview(FileBrowingTreeNode node)
        {
            if (CurFilePreviewNode == node)
            {
                return;
            }

            CurFilePreviewNode = node;

            if (null == node || !node.Data.IsFile)
            {//文件夹无法预览
                FilePreviewState = EnumFilePreviewState.None;
                return;
            }

            if (node.FileSize == "0")
            {//文件大小为0，无法预览
                FilePreviewState = EnumFilePreviewState.Error;
                FilePreviewErrorMsg = LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_PreView_FileZero];
                return;
            }

            if (node.Data.IsLocalFile)
            {//本地文件，直接预览
                FilePreviewState = EnumFilePreviewState.Show;
                MsgAggregation.Instance.SendGeneralMsg(new GeneralArgs<object>(MessageKeys.PreviewKey) { Parameters = node.Data.LocalFilePath });
            }
            else
            {//先下载，再预览
                Task.Run(async () =>
                {
                    FilePreviewState = EnumFilePreviewState.Downloading;
                    CancellationTokenSource cts = new CancellationTokenSource();
                    FileBrowingIAsyncTaskProgress fts = new FileBrowingIAsyncTaskProgress();
                    fts.ExportFileNodeHandle += (dn, issuccess, localpath) =>
                      {
                          if (issuccess && FileHelper.IsValid(localpath))
                          {
                              FilePreviewState = EnumFilePreviewState.Show;

                              AsyncOperation.Post((t) => MsgAggregation.Instance.SendGeneralMsg(new GeneralArgs<object>(MessageKeys.PreviewKey) { Parameters = localpath }), null);
                          }
                          else
                          {
                              FilePreviewState = EnumFilePreviewState.Error;
                              FilePreviewErrorMsg = LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_PreView_DownloadError];
                          }
                      };

                    await Service.Download(node.Data, System.IO.Path.GetTempPath(), false, cts, fts);
                });
            }
        }

        #endregion

        #region 文件下载

        /// <summary>
        /// 下载文件节点
        /// 可以是单个文件夹 也可以是单个文件
        /// </summary>
        /// <param name="node"></param>
        public void OnDownFileNode(FileBrowingTreeNode node)
        {
            Download(node);
        }

        /// <summary>
        /// 下载选中的所有文件节点
        /// </summary>
        public void OnDownSelectedFileNode()
        {
            if (null == TableItems || !TableItems.Any(f => f.IsSelected))
            {//没有可以下载的文件节点
                MessageBox.ShowDialogErrorMsg(LanguageHelper.LanguageManager[Languagekeys.FileBrowing_Msg_SelectDownloadFileNode]);
            }
            else
            {
                Download(TableItems.Where(f => f.IsSelected).ToArray());
            }
        }

        /// <summary>
        /// 导出文件节点
        /// </summary>
        /// <param name="nodes"></param>
        private void Download(params FileBrowingTreeNode[] nodes)
        {
            if (nodes.IsInvalid())
            {
                return;
            }

            var savePath = PopupWindowService.SelectFolderDialog();

            if (savePath.IsValid())
            {
                FileNodeExportParameter param = new FileNodeExportParameter()
                {
                    Service = Service,
                    Nodes = nodes,
                    SavePath = savePath
                };

                PopupWindowService.ShowDialogWindow("ExportKey_ModuleFileNodeExportView", param);
            }
        }

        #endregion

        #region 文件搜索

        private bool _IsSearching = false;

        /// <summary>
        /// 是否正在搜索文件
        /// </summary>
        public bool IsSearching
        {
            get => _IsSearching;
            set
            {
                _IsSearching = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<EnumDataState, string> _DataStateSource = null;

        /// <summary>
        /// 数据状态列表
        /// </summary>	
        public Dictionary<EnumDataState, string> DataStateSource
        {
            get { return _DataStateSource; }
            set
            {
                _DataStateSource = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<EnumFileType, string> _FileTypeSource = null;

        /// <summary>
        /// 文件类型列表
        /// </summary>	
        public Dictionary<EnumFileType, string> FileTypeSource
        {
            get { return _FileTypeSource; }
            set
            {
                _FileTypeSource = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<int, string> _KeywordTypeSource = null;

        /// <summary>
        /// 关键词\正则列表
        /// </summary>	
        public Dictionary<int, string> KeywordTypeSource
        {
            get { return _KeywordTypeSource; }
            set
            {
                _KeywordTypeSource = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _StartTime = null;

        /// <summary>
        /// 起始时间
        /// </summary>	
        public DateTime? StartTime
        {
            get { return _StartTime; }
            set
            {
                _StartTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _EndTime = null;

        /// <summary>
        /// 结束时间
        /// </summary>	
        public DateTime? EndTime
        {
            get { return _EndTime; }
            set
            {
                _EndTime = value;
                OnPropertyChanged();
            }
        }

        private EnumDataState _DataState = EnumDataState.None;

        /// <summary>
        /// 数据状态
        /// </summary>	
        public EnumDataState DataState
        {
            get { return _DataState; }
            set
            {
                _DataState = value;
                OnPropertyChanged();
            }
        }

        private EnumFileType _FileType = EnumFileType.All;

        /// <summary>
        /// 文件类型
        /// </summary>	
        public EnumFileType FileType
        {
            get { return _FileType; }
            set
            {
                _FileType = value;
                OnPropertyChanged();
            }
        }

        private int _KeywordType = 0;

        /// <summary>
        /// 关键词（0）还是正则表达式（1）
        /// </summary>	
        public int KeywordType
        {
            get { return _KeywordType; }
            set
            {
                _KeywordType = value;
                OnPropertyChanged();

                if (0 == value)
                {
                    KeywordTypeTip = LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_Search_KeywordTip];
                }
                else
                {
                    KeywordTypeTip = LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_Search_ZhengZeTip];
                }
            }
        }

        private string _KeywordTypeTip = LanguageHelper.LanguageManager[Languagekeys.FileBrowing_View_Search_KeywordTip];

        /// <summary>
        /// 关键词提示信息
        /// </summary>
        public string KeywordTypeTip
        {
            get { return _KeywordTypeTip; }
            set
            {
                _KeywordTypeTip = value;
                OnPropertyChanged();
            }
        }

        private string _Keyword = "";

        /// <summary>
        /// 关键词内容
        /// </summary>	
        public string Keyword
        {
            get { return _Keyword; }
            set
            {
                _Keyword = value;
                OnPropertyChanged();
            }
        }

        private CancellationTokenSource SearchCts { get; set; }
        private FileBrowingIAsyncTaskProgress SearchIts { get; set; }

        /// <summary>
        /// 清空搜索条件
        /// </summary>
        public void OnClearSearch()
        {
            StartTime = null;
            EndTime = null;
            DataState = EnumDataState.None;
            FileType = EnumFileType.All;
            KeywordType = 0;
            Keyword = null;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public void OnSearchFileNode()
        {
            if (IsSearching)
            {//取消搜索
                SearchCts?.Cancel();
            }
            else
            {//开始搜索
                List<FilterArgs> list = new List<FilterArgs>();
                if (DataState != EnumDataState.None)
                {
                    list.Add(new FilterByEnumStateArgs() { State = DataState });
                }
                if (StartTime != null || EndTime != null)
                {
                    list.Add(new FilterByDateRangeArgs() { StartTime = StartTime, EndTime = EndTime });
                }
                if (FileType != EnumFileType.All)
                {
                    list.Add(new FilterByEnumFileTypeArgs(FileType));
                }
                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    if (KeywordType == 0)
                    {
                        list.Add(new FilterByStringContainsArgs() { PatternText = Keyword });
                    }
                    else
                    {
                        list.Add(new FilterByRegexArgs() { Regex = new System.Text.RegularExpressions.Regex(Keyword) });
                    }
                }

                if (null == SearchIts)
                {
                    SearchIts = new FileBrowingIAsyncTaskProgress();
                    SearchIts.SearchFileNodeHandle += Its_SearchFileNodeHandle;
                }
                SearchCts = new CancellationTokenSource();

                IsSearching = true;
                TableItems = new ObservableCollection<FileBrowingTreeNode>();

                Task.Run(async () => await Service.Search(CurFileBrowingTreeNode.Data, list, SearchCts, SearchIts), SearchCts.Token).ContinueWith((t) => IsSearching = false);
            }
        }

        private void Its_SearchFileNodeHandle(FileBrowingNode obj)
        {
            AsyncOperation.Post((t) => TableItems.Add(new FileBrowingTreeNode(obj)), null);
        }

        #endregion

    }

    /// <summary>
    /// 文件树节点
    /// </summary>
    public class FileBrowingTreeNode : NotifyPropertyBase
    {
        private static readonly DateTime UnavailableDateTime = new DateTime(1, 1, 1, 0, 0, 0);

        /// <summary>
        /// 文件节点
        /// </summary>
        public FileBrowingNode Data { get; set; }

        private List<FileBrowingTreeNode> _AllChildrenNodes;

        /// <summary>
        /// 文件子节点
        /// </summary>
        public List<FileBrowingTreeNode> AllChildrenNodes
        {
            get => _AllChildrenNodes;
            set
            {
                _AllChildrenNodes = value;
                OnPropertyChanged();
            }
        }

        private List<FileBrowingTreeNode> _ChildrenTreeNodes;

        /// <summary>
        /// 文件树子节点
        /// </summary>
        public List<FileBrowingTreeNode> ChildrenTreeNodes
        {
            get => _ChildrenTreeNodes;
            set
            {
                _ChildrenTreeNodes = value;
                OnPropertyChanged();
            }
        }

        private bool _IsSelected;

        /// <summary>
        /// 是否标记
        /// </summary>
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                _IsSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public EnumDataState DataState => Data.NodeState;

        /// <summary>
        /// 节点名称 
        /// 显示用
        /// </summary>
        public string Text => Data.Name;

        /// <summary>
        /// 类型
        /// </summary>
        public string TypeDesc => Data.IsFile ? "文件" : "文件夹";

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime => Data.CreateTime != null && !Data.CreateTime.Equals(UnavailableDateTime) ? Data.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastWriteTime => Data.LastWriteTime != null && !Data.LastWriteTime.Equals(UnavailableDateTime) ? Data.LastWriteTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public string LastAccessTime => Data.LastAccessTime != null && !Data.LastAccessTime.Equals(UnavailableDateTime) ? Data.LastAccessTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize => Data.IsFile ? FileHelper.GetFileSize((long)Data.FileSize) : "";

        public FileBrowingTreeNode(FileBrowingNode node)
        {
            Data = node;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>
    /// 文件预览状态
    /// </summary>
    public enum EnumFilePreviewState
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 预览错误
        /// </summary>
        Error,
        /// <summary>
        /// 下载文件中
        /// </summary>
        Downloading,
        /// <summary>
        /// 文件预览显示
        /// </summary>
        Show
    }

    /// <summary>
    /// 内部消息
    /// </summary>
    public class MessageKeys
    {
        public const string PreviewKey = "FileBrowingViewPreviewKey";
    }
}
