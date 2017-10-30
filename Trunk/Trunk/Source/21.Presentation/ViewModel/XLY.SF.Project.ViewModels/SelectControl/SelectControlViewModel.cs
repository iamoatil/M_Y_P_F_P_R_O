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
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.Model.SelectControlElement;
using XLY.SF.Project.ViewDomain.VModel.SelectControl;

namespace XLY.SF.Project.ViewModels.SelectControl
{
    [Export(ExportKeys.SelectControlViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SelectControlViewModel : ViewModelBase
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

        #region 当前路径【文件夹层级】

        private FolderElement _curFolderLevel;
        /// <summary>
        /// 当前文件夹层级
        /// </summary>
        public FolderElement CurFolderLevel
        {
            get
            {
                return this._curFolderLevel;
            }

            set
            {
                this._curFolderLevel = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// 加载子文件夹
        /// </summary>
        public ICommand LoadFolderCommand { get; set; }
        /// <summary>
        /// 进入文件夹
        /// </summary>
        public ICommand SelectedFolderCommand { get; set; }
        /// <summary>
        /// 选择了文件夹文件列表中的某一项
        /// </summary>
        public ICommand SelectedItemCommand { get; set; }
        /// <summary>
        /// 进入选择项【如果是文件就直接选中】
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
        public SelectControlViewModel(IMessageBox msgService)
        {
            _msgService = msgService;

            Folders = new ObservableCollection<FolderElement>();
            FolderFileItems = new ObservableCollection<FolderElement>();
            LoadFolderCommand = new RelayCommand<FolderElement>(ExecuteLoadFolderCommand);
            SelectedFolderCommand = new RelayCommand<FolderElement>(ExecuteSelectedFolderCommand);
            SelectedItemCommand = new RelayCommand<FolderElement>(ExecuteSelectedItemCommand);
            InSelectedItemCommand = new RelayCommand<FolderElement>(ExecuteInSelectedItemCommand);
            SelectedCompleteCommand = new RelayCommand(ExecuteSelectedCompleteCommand);
            CancelSelectCommand = new RelayCommand(ExecuteCancelSelectCommand);
            ResetFilterCommand = new RelayCommand<string>(ExecuteResetFilterCommand);
            FilterItems = new ObservableCollection<FilterElement>()
            {
                new FilterElement()
                {
                    FilterName = "全部",
                    FilterValue = "*.*"
                }
            };
            _curFilter = FilterItems.First().FilterValue;
            LoadFolders();
        }

        public override object GetResult()
        {
            return _curSelectedItem?.FullPath;
        }

        protected override void LoadCore(object parameters)
        {
            if (parameters != null)
            {
                var filters = parameters.ToString().Split(';');
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
            }
        }

        #region ExecuteCommand

        //重置赛选
        private void ExecuteResetFilterCommand(string obj)
        {
            _curFilter = obj;
            //刷新当前显示的文件内容
            InFolderAndUpdateLevel(CurFolderLevel);
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
                    InFolderAndUpdateLevel(_curSelectedItem);
                }
                else
                {
                    if (File.Exists(CurSelectedItemInFolder.FullPath))
                        CloseView();
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
                InFolderAndUpdateLevel(obj);
            }
            else
            {
                //选中文件
                _curSelectedItem = obj;
                base.CloseView();
                //CloseView(true);
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

        private void ExecuteSelectedFolderCommand(FolderElement obj)
        {
            //刷新子文件夹
            InFolderAndUpdateLevel(obj);
        }

        private void ExecuteLoadFolderCommand(FolderElement obj)
        {
            //加载子文件夹
            foreach (var item in obj.SubFolders)
            {
                item.LoadSubFolderAndFiles(_curFilter);
            }
        }

        #endregion

        #region Tools

        /// <summary>
        /// 关闭选择界面
        /// </summary>
        /// <param name="isComplete">是否选择完成</param>
        private void CloseView(bool isComplete)
        {
            //GeneralArgs<bool> closeSelectViewArgs = new GeneralArgs<bool>(GeneralKeys.CloseSelectView);
            //closeSelectViewArgs.Parameters = isComplete;
            //base.MessageAggregation.SendGeneralMsg<bool>(closeSelectViewArgs);

            //注销所有消息
            //base.MessageAggregation.UnRegisterMsgAll(this);
        }

        private void LoadFolders()
        {
            //桌面
            Folders.Add(new FolderElement(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), null));
            Folders.Last().LoadSubFolderAndFiles(_curFilter);
            //我的文档
            Folders.Add(new FolderElement(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)), null));
            Folders.Last().LoadSubFolderAndFiles(_curFilter);
            foreach (var item in Environment.GetLogicalDrives())
            {
                FolderElement tmpEmt = new FolderElement(new System.IO.DirectoryInfo(item), null);
                tmpEmt.LoadSubFolderAndFiles(_curFilter);
                Folders.Add(tmpEmt);
            }
        }

        #region 进入文件夹和刷新文件夹

        /// <summary>
        /// 进入文件夹
        /// </summary>
        /// <param name="inFolder"></param>
        private void InFolderAndUpdateLevel(FolderElement inFolder)
        {
            if (inFolder != null && inFolder.IsFolder)
            {
                //刷新当前目录
                inFolder.LoadSubFolderAndFiles(_curFilter);
                //刷新子目录
                foreach (var item in inFolder.SubFolders)
                {
                    item.LoadSubFolderAndFiles(_curFilter);
                }
                //记录层级
                CurFolderLevel = inFolder;
                //更新显示内容
                RefreshView(inFolder);
            }
        }

        /// <summary>
        /// 刷新界面显示
        /// </summary>
        /// <param name="curFolder"></param>
        private void RefreshView(FolderElement curFolder)
        {
            //清空之前文件信息
            FolderFileItems.Clear();
            //加载文件夹
            foreach (var item in curFolder.SubFolders)
            {
                FolderFileItems.Add(item);
            }
            //加载文件
            foreach (var item in curFolder.Files)
            {
                FolderFileItems.Add(item);
            }
        }

        #endregion

        #endregion
    }
}
