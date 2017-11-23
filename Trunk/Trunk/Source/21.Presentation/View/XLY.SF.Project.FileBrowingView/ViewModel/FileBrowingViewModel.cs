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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.FileBrowingView
{
    public class FileBrowingViewModel : ViewModelBase
    {
        public FileBrowingViewModel()
        {
            SelecedNodeChanged = new RelayCommand<FileBrowingTreeNode>(OnSelecedNodeChanged);
            OpenFileNodeCommand = new RelayCommand<FileBrowingTreeNode>(OnOpenFileNode);
            DownFileNodeCommand = new RelayCommand<FileBrowingTreeNode>(OnDownFileNode);
            ClickCheckCommand = new RelayCommand<FileBrowingTreeNode>(OnClickCheckCommand);

            Source = @"G:\镜像\2016-0107.bin";

            //初始化服务
            LoadingData(async () =>
            {
                Service = FileBrowsingServiceFactory.GetFileBrowsingService(Source);
                FileBrowingNode ss = await Service.GetRootNode();

                Roots = new List<FileBrowingTreeNode>() { new FileBrowingTreeNode(ss) };
            });
        }

        public RelayCommand<FileBrowingTreeNode> SelecedNodeChanged { get; set; }

        public RelayCommand<FileBrowingTreeNode> OpenFileNodeCommand { get; set; }

        public RelayCommand<FileBrowingTreeNode> DownFileNodeCommand { get; set; }

        public RelayCommand<FileBrowingTreeNode> ClickCheckCommand { get; set; }


        /// <summary>
        /// 数据源
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// 文件浏览服务
        /// </summary>
        public AbsFileBrowsingService Service { get; set; }

        private List<FileBrowingTreeNode> _Roots;

        /// <summary>
        /// 文件树根节点
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

        private List<FileBrowingTreeNode> _TableItems;

        /// <summary>
        /// 表格数据
        /// </summary>
        public List<FileBrowingTreeNode> TableItems
        {
            get => _TableItems;
            set
            {
                _TableItems = value;
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

        private bool _IsLoading;

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

        /// <summary>
        /// 加载节点数据
        /// </summary>
        /// <param name="doSomething"></param>
        public async void LoadingData(Action doSomething)
        {
            IsLoading = true;
            await Task.Run(doSomething).ContinueWith((t) => IsLoading = false);
        }

        public void OnSelecedNodeChanged(FileBrowingTreeNode node)
        {
            if (node.AllChildrenNodes == null)
            {
                LoadingData(() =>
                {
                    var ss = Service.GetChildNodes(node.Data).Result;
                    node.AllChildrenNodes = new List<FileBrowingTreeNode>(ss.Select(f => new FileBrowingTreeNode(f)));
                    node.ChildrenTreeNodes = new List<FileBrowingTreeNode>(ss.Where(f => !f.IsFile).Select(f => new FileBrowingTreeNode(f)));

                    TableItems = node.AllChildrenNodes;
                    IsTableSelectAll = TableItems.All(s => s.IsSelected);
                });
            }
            else
            {
                TableItems = node.AllChildrenNodes;
                IsTableSelectAll = TableItems.All(s => s.IsSelected);
            }
        }

        /// <summary>
        /// 下载文件节点
        /// 可以是单个文件夹 也可以是单个文件
        /// </summary>
        /// <param name="node"></param>
        public void OnDownFileNode(FileBrowingTreeNode node)
        {

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

        }

        /// <summary>
        /// 标记
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
    }

    /// <summary>
    /// 文件树节点
    /// </summary>
    public class FileBrowingTreeNode : NotifyPropertyBase
    {
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
        public string CreateTime => Data.CreateTime != null ? Data.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastWriteTime => Data.LastWriteTime != null ? Data.LastWriteTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public string LastAccessTime => Data.LastAccessTime != null ? Data.LastAccessTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize => Data.IsFile ? Data.FileSize.ToString() : "";

        public FileBrowingTreeNode(FileBrowingNode node)
        {
            Data = node;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
