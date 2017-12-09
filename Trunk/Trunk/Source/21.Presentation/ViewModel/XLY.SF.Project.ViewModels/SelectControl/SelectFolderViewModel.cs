using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.ViewDomain.Enums;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.Model.SelectControlElement;
using XLY.SF.Project.ViewModels.SelectControl.SelectService;

namespace XLY.SF.Project.ViewModels.SelectControl
{
    [Export(ExportKeys.SelectFolderViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SelectFolderViewModel : ViewModelBase
    {
        #region Propertes

        /// <summary>
        /// 消息框服务
        /// </summary>
        private IMessageBox _msgService;
        /// <summary>
        /// 文件夹树
        /// </summary>
        public ObservableCollection<FolderElement> Folders { get; set; }

        /// <summary>
        /// 文件夹和文件
        /// </summary>
        public ObservableCollection<FolderElement> FolderFileItems { get; set; }

        #region 当前文件夹内的选择

        private FolderElement _curSelectedItemInFolder;
        /// <summary>
        /// 当前选中的文件夹
        /// </summary>
        public FolderElement CurSelectedItemInFolder
        {
            get
            {
                return this._curSelectedItemInFolder;
            }

            private set
            {
                this._curSelectedItemInFolder = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 选择管理器

        /// <summary>
        /// 选择管理器
        /// </summary>
        public SelectManager SelectManager { get; private set; }

        #endregion

        #endregion

        #region ICommands

        /// <summary>
        /// 进入路径
        /// </summary>
        public ICommand InPathCommand { get; private set; }
        /// <summary>
        /// 返回上一级文件夹
        /// </summary>
        public ICommand BackParentFolderCommand { get; private set; }
        /// <summary>
        /// 新建文件夹
        /// </summary>
        public ICommand CreateNewFolderCommand { get; private set; }
        /// <summary>
        /// 加载子文件夹【目前只有第一级目录】
        /// </summary>
        public ICommand LoadFolderCommand { get; set; }
        /// <summary>
        /// 选择了文件夹文件列表中的某一项
        /// </summary>
        public ICommand SelectedItemCommand { get; set; }
        /// <summary>
        /// 进入选择项【双击】
        /// </summary>
        public ICommand InSelectedItemCommand { get; set; }
        /// <summary>
        /// 选择完成
        /// </summary>
        public ICommand SelectedCompleteCommand { get; set; }
        /// <summary>
        /// 取消选择
        /// </summary>
        public ICommand CancelSelectCommand { get; set; }

        #endregion

        [ImportingConstructor]
        public SelectFolderViewModel(IMessageBox msgService)
        {
            _msgService = msgService;

            Folders = new ObservableCollection<FolderElement>();
            FolderFileItems = new ObservableCollection<FolderElement>();


            InPathCommand = new RelayCommand<string>(ExecuteInPathCommand);
            BackParentFolderCommand = new RelayCommand(ExecuteBackParentFolderCommand);
            CreateNewFolderCommand = new RelayCommand(ExecuteCreateNewFolderCommand);
            LoadFolderCommand = new RelayCommand<FolderElement>(ExecuteLoadFolderCommand);
            SelectedItemCommand = new RelayCommand<FolderElement>(ExecuteSelectedItemCommand);
            InSelectedItemCommand = new RelayCommand<FolderElement>(ExecuteInSelectedItemCommand);
            SelectedCompleteCommand = new RelayCommand(ExecuteSelectedCompleteCommand);
            CancelSelectCommand = new RelayCommand(ExecuteCancelSelectCommand);
        }

        protected override void InitLoad(object parameters)
        {
            SelectManager = new SelectManager(SelectControlType.SelectFolder);
            //_curFilter = FilterItems.First().FilterValue;
            var initFolders = SelectManager.InitFolders();
            foreach (var item in initFolders)
            {
                Folders.Add(item);
            }
        }

        public override object GetResult()
        {
            if (DialogResult)
                return Path.Combine(CurSelectedItemInFolder.Parent.FullPath, CurSelectedItemInFolder.Name);
            return null;
        }

        #region ExecuteCommands

        //取消选择
        private void ExecuteCancelSelectCommand()
        {
            CurSelectedItemInFolder = null;
            CloseView();
        }

        //完成选择【确定按钮】
        private void ExecuteSelectedCompleteCommand()
        {
            if (CurSelectedItemInFolder != null)
            {
                var curFullPath = Path.Combine(CurSelectedItemInFolder.Parent.FullPath, CurSelectedItemInFolder.Name);
                if (Directory.Exists(curFullPath))
                {
                    //确定选择的文件夹
                    base.DialogResult = true;
                    CloseView();
                }
            }
        }

        //选中或者进入文件夹【双击】
        private void ExecuteInSelectedItemCommand(FolderElement obj)
        {
            //进入文件夹
            UpdateFolders(SelectManager.InFolderAndUpdateLevel(obj));
        }

        //选择了某一项
        private void ExecuteSelectedItemCommand(FolderElement obj)
        {
            //_curSelectedItem = obj;
            CurSelectedItemInFolder = obj;
        }

        private void ExecuteLoadFolderCommand(FolderElement obj)
        {
            UpdateFolders(SelectManager.InFolderAndUpdateLevel(obj));
        }

        //新建文件夹
        private void ExecuteCreateNewFolderCommand()
        {
            if (SelectManager.CurFolderLevel != null)
            {
                string tmpName = SelectManager.GetNewFolderName(SelectManager.CurFolderLevel.FullPath);
                try
                {
                    Directory.CreateDirectory(tmpName);
                    //新建成功后加入当前文件列表
                    UpdateFolders(SelectManager.InFolderAndUpdateLevel(SelectManager.CurFolderLevel));
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex, string.Format("在【{0}】创建文件夹失败", SelectManager.CurFolderLevel.FullPath));
                }
            }
        }

        //返回上一级文件夹
        private void ExecuteBackParentFolderCommand()
        {
            var parent = SelectManager.CurFolderLevel?.Parent;
            if (parent != null)
                UpdateFolders(SelectManager.InFolderAndUpdateLevel(parent));
        }

        //进入输入的路径
        private void ExecuteInPathCommand(string obj)
        {
            if (Directory.Exists(obj))
            {
                FolderElement tmpFolder = new FolderElement(new DirectoryInfo(obj));
                UpdateFolders(SelectManager.InFolderAndUpdateLevel(tmpFolder));
            }
        }

        #endregion

        #region Tools

        /// <summary>
        /// 更新显示
        /// </summary>
        /// <param name="items"></param>
        private void UpdateFolders(List<FolderElement> items)
        {
            FolderFileItems.Clear();
            foreach (var item in items)
            {
                FolderFileItems.Add(item);
            }
            CurSelectedItemInFolder = null;
        }

        #endregion
    }
}
