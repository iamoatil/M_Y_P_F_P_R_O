using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.ViewDomain.Enums;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.Model.SelectControlElement;
using XLY.SF.Project.ViewDomain.VModel.SelectControl;
using XLY.SF.Project.ViewModels.SelectControl.SelectService;

namespace XLY.SF.Project.ViewModels.SelectControl
{
    [Export(ExportKeys.SelectFileViewViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SelectFileViewModel : ViewModelBase
    {
        #region Propertes

        /// <summary>
        /// 消息框服务
        /// </summary>
        private IMessageBox _msgService;

        /// <summary>
        /// 当前选择项
        /// </summary>
        private FolderElement _curSelectedItem;

        /// <summary>
        /// 当前赛选
        /// </summary>
        private string _curFilter;

        /// <summary>
        /// 文件夹树
        /// </summary>
        public ObservableCollection<FolderElement> Folders { get; set; }

        /// <summary>
        /// 文件夹和文件
        /// </summary>
        public ObservableCollection<FolderElement> FolderFileItems { get; set; }

        /// <summary>
        /// 赛选内容
        /// </summary>
        public ObservableCollection<FilterElement> FilterItems { get; set; }

        #region 当前文件夹内的选择

        private FolderElement _curSelectedItemInFolder;
        /// <summary>
        /// 当前文件夹内的选择
        /// </summary>
        public FolderElement CurSelectedItemInFolder
        {
            get
            {
                return this._curSelectedItemInFolder;
            }

            set
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

        #region Commands
        /// <summary>
        /// 新建文件夹
        /// </summary>
        public ICommand CreateNewFolderCommand { get; private set; }
        /// <summary>
        /// 返回上一级文件夹
        /// </summary>
        public ICommand BackParentFolderCommand { get; private set; }
        /// <summary>
        /// 进入路径
        /// </summary>
        public ICommand InPathCommand { get; private set; }
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
        /// <summary>
        /// 重置赛选项
        /// </summary>
        public ICommand ResetFilterCommand { get; set; }

        #endregion

        [ImportingConstructor]
        public SelectFileViewModel(IMessageBox msgService)
        {
            _msgService = msgService;

            Folders = new ObservableCollection<FolderElement>();
            FolderFileItems = new ObservableCollection<FolderElement>();
            LoadFolderCommand = new RelayCommand<FolderElement>(ExecuteLoadFolderCommand);
            SelectedItemCommand = new RelayCommand<FolderElement>(ExecuteSelectedItemCommand);
            InSelectedItemCommand = new RelayCommand<FolderElement>(ExecuteInSelectedItemCommand);
            SelectedCompleteCommand = new RelayCommand(ExecuteSelectedCompleteCommand);
            CancelSelectCommand = new RelayCommand(ExecuteCancelSelectCommand);
            ResetFilterCommand = new RelayCommand<string>(ExecuteResetFilterCommand);
            InPathCommand = new RelayCommand<string>(ExecuteInPathCommand);
            BackParentFolderCommand = new RelayCommand(ExecuteBackParentFolderCommand);
            CreateNewFolderCommand = new RelayCommand(ExecuteCreateNewFolderCommand);

            //LoadFolders();
        }

        public override object GetResult()
        {
            return _curSelectedItem?.FullPath;
        }

        protected override void InitLoad(object parameters)
        {
            if (parameters != null)
            {
                var filters = parameters.ToString().Split(';');
                FilterItems = new ObservableCollection<FilterElement>()
                {
                    new FilterElement()
                    {
                        FilterName = "全部",
                        FilterValue = "*.*"
                    }
                };
                foreach (var item in filters)
                {
                    if (item.Contains('|'))
                    {
                        FilterItems.Add(new FilterElement()
                        {
                            FilterName = item.Split('|')[0],
                            FilterValue = item.Split('|')[1],
                        });
                    }
                }
                SelectManager = new SelectManager(SelectControlType.SelectFile, FilterItems.First().FilterValue);
                //_curFilter = FilterItems.First().FilterValue;
                var initFolders = SelectManager.InitFolders();
                foreach (var item in initFolders)
                {
                    Folders.Add(item);
                }
            }
        }

        #region ExecuteCommand

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

        //重置赛选
        private void ExecuteResetFilterCommand(string obj)
        {
            _curFilter = obj;
            //刷新当前显示的文件内容
            UpdateFolders(SelectManager.InFolderAndUpdateLevel(SelectManager.CurFolderLevel));
        }

        //取消选择
        private void ExecuteCancelSelectCommand()
        {
            _curSelectedItem = null;
            CloseView();
        }

        //完成选择【确定按钮】
        private void ExecuteSelectedCompleteCommand()
        {
            if (_curSelectedItem != null)
            {
                if (_curSelectedItem.IsFolder)
                {
                    //进入文件夹
                    UpdateFolders(SelectManager.InFolderAndUpdateLevel(_curSelectedItem));
                }
                else
                {
                    if (File.Exists(CurSelectedItemInFolder.FullPath))
                    {
                        base.DialogResult = true;
                        CloseView();
                    }
                    else
                    {
                        //不存在提示
                        _msgService.ShowErrorMsg("打开路径不存在");
                    }
                }
            }
        }

        //选中或者进入文件夹【双击】
        private void ExecuteInSelectedItemCommand(FolderElement obj)
        {
            if (obj.IsFolder)
            {
                //进入文件夹
                UpdateFolders(SelectManager.InFolderAndUpdateLevel(obj));
            }
            else
            {
                base.DialogResult = true;
                _curSelectedItem = obj;
                base.CloseView();
            }
        }

        //选择了某一项
        private void ExecuteSelectedItemCommand(FolderElement obj)
        {
            _curSelectedItem = obj;
            if (!_curSelectedItem.IsFolder)
            {
                CurSelectedItemInFolder = obj;
            }
            else
                CurSelectedItemInFolder = null;
        }

        private void ExecuteLoadFolderCommand(FolderElement obj)
        {
            UpdateFolders(SelectManager.InFolderAndUpdateLevel(obj));
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
        }

        #endregion

        /*///// <summary>
        ///// 进入指定的路径
        ///// </summary>
        ///// <param name="inputPath"></param>
        //private void InFolderByInput(string inputPath)
        //{
        //    if (!string.IsNullOrWhiteSpace(inputPath) && Directory.Exists(inputPath))
        //    {
        //        FolderElement inputFolder = new FolderElement(new DirectoryInfo(inputPath));
        //    }
        //}

        ///// <summary>
        ///// 获取新文件夹的名称
        ///// </summary>
        ///// <param name="curPath">当前新建文件夹的路径</param>
        ///// <returns></returns>
        //private string GetNewFolderName(string curPath)
        //{
        //    string newFolderName = string.Empty;
        //    if (!string.IsNullOrWhiteSpace(curPath))
        //    {
        //        newFolderName = Path.Combine(curPath, "新建文件夹");
        //        int folderIndex = 1;
        //        while (Directory.Exists(newFolderName))
        //        {
        //            newFolderName = Path.Combine(curPath, string.Format("新建文件夹（{0}）", folderIndex));
        //            folderIndex++;
        //        }
        //    }

        //    return newFolderName;
        //}

        //private void LoadFolders()
        //{
        //    //桌面
        //    Folders.Add(new FolderElement(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))));
        //    Folders.Last().LoadSubFolderAndFiles(_curFilter);
        //    if (SystemContext.LanguageManager.Type == Framework.Language.LanguageType.Cn)
        //        Folders.Last().Name = "桌面";
        //    //我的文档
        //    Folders.Add(new FolderElement(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))));
        //    Folders.Last().LoadSubFolderAndFiles(_curFilter);
        //    if (SystemContext.LanguageManager.Type == Framework.Language.LanguageType.Cn)
        //        Folders.Last().Name = "我的文档";

        //    foreach (var item in Environment.GetLogicalDrives())
        //    {
        //        FolderElement tmpEmt = new FolderElement(new System.IO.DirectoryInfo(item));
        //        tmpEmt.LoadSubFolderAndFiles(_curFilter);
        //        Folders.Add(tmpEmt);
        //    }
        //}

        //#region 进入文件夹和刷新文件夹

        ///// <summary>
        ///// 进入文件夹
        ///// </summary>
        ///// <param name="inFolder"></param>
        //private void InFolderAndUpdateLevel(FolderElement inFolder)
        //{
        //    if (inFolder != null && inFolder.IsFolder)
        //    {
        //        //刷新当前目录
        //        inFolder.LoadSubFolderAndFiles(_curFilter);
        //        //刷新子目录
        //        foreach (var item in inFolder.SubFolders)
        //        {
        //            item.LoadSubFolderAndFiles(_curFilter);
        //        }
        //        //记录层级
        //        CurFolderLevel = inFolder;
        //        //更新显示内容
        //        RefreshView(inFolder);
        //    }
        //}

        ///// <summary>
        ///// 刷新界面显示
        ///// </summary>
        ///// <param name="curFolder"></param>
        //private void RefreshView(FolderElement curFolder)
        //{
        //    //清空之前文件信息
        //    FolderFileItems.Clear();
        //    //加载文件夹
        //    foreach (var item in curFolder.SubFolders)
        //    {
        //        FolderFileItems.Add(item);
        //    }
        //    if (CurStatus == SelectControlType.SelectFile)
        //    {
        //        //加载文件
        //        foreach (var item in curFolder.Files)
        //        {
        //            FolderFileItems.Add(item);
        //        }
        //    }
        //}

        //#endregion*/
    }
}
