using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    [Export(ExportKeys.CaseListViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CaseListViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _openCommandProxy;

        private readonly ProxyRelayCommandBase _removeCommandProxy;

        private readonly ProxyRelayCommandBase _searchCommandProxy;

        private readonly ProxyRelayCommandBase _removeBatchCommandProxy;

        #endregion

        #region Constructors

        public CaseListViewModel()
        {
            FilterArgs = new CaseFilterArgs();
            _openCommandProxy = new ProxyRelayCommand<RecentCaseEntityModel>(Open);
            _removeCommandProxy = new ProxyRelayCommand<CaseItem>(Remove);
            _searchCommandProxy = new ProxyRelayCommand(Search);
            SelectAllCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand<Boolean>(SelectAll, (b) => Items != null && Items.Count != 0);
            SelectCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand<Boolean>(Select);
            _removeBatchCommandProxy = new ProxyRelayCommand(RemoveBatch);
        }

        #endregion

        #region Properties
        
        #region Items

        private ObservableCollection<CaseItem> _items;
        public ObservableCollection<CaseItem> Items
        {
            get => _items;
            private set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public CaseFilterArgs FilterArgs { get; }

        public ICommand OpenCommand => _openCommandProxy.ViewExecuteCmd;

        public ICommand RemoveBatchCommand => _removeBatchCommandProxy.ViewExecuteCmd;

        public ICommand RemoveCommand => _removeCommandProxy.ViewExecuteCmd;

        public ICommand SearchCommand => _searchCommandProxy.ViewExecuteCmd;

        public ICommand SelectAllCommand { get; }

        public ICommand SelectCommand { get; }

        #region IsSelectAll

        private Boolean? _isSelectAll = false;

        public Boolean? IsSelectAll
        {
            get => _isSelectAll;
            private set
            {
                _isSelectAll = value;
                OnPropertyChanged();
            }
        }

        #endregion

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        [Import(typeof(IRecordContext<RecentCase>))]
        private IRecordContext<RecentCase> DbService { get; set; }

        //#region PageNo

        //private Int32 _pageNo = 1;

        //public Int32 PageNo
        //{
        //    get => _pageNo;
        //    private set
        //    {
        //        _pageNo = value;
        //        OnPropertyChanged();
        //    }
        //}

        //#endregion

        //#region PageCount

        //private Int32 _pageCount = 0;

        //public Int32 PageCount
        //{
        //    get => _pageCount;
        //    private set
        //    {
        //        _pageCount = value;
        //        OnPropertyChanged();
        //    }
        //}

        //#endregion

        #endregion

        #region Methods     

        #region Protected

        protected override void InitLoad(object parameters)
        {
            //PagingRequest paging = new PagingRequest(1, 100);
            //var result = DbService.QueryOfPaging<RecentCaseEntity, RecentCaseEntityModel>(paging, (e) => true);
            //PageCount = result.PageCount;
            var result = DbService.Records.ToModels<RecentCase, RecentCaseEntityModel>().ToArray();
            Int32 index = 1;
            var items = result.OrderByDescending(x=>x.Timestamp).Select(x => new CaseItem(x, index++)).ToArray();
            Items = new ObservableCollection<CaseItem>(items);
        }

        #endregion

        #region Private

        private String RemoveBatch()
        {
            if (_items == null) return "没有可删除的案例";
            var selected = _items.Where(x => x.IsChecked).ToArray();
            if (selected.Length == 0) return "没有可删除的案例";
            if (MessageBox.ShowDialogWarningMsg("是否确定删除？"))
            {
                Case @case = null;
                foreach (CaseItem item in selected)
                {
                    Items.Remove(item);
                    @case = Case.Open(item.CaseInfo.CaseProjectFile);
                    if (@case == null) continue;
                    @case.Delete();
                }
                DbService.RemoveRange(selected.Select(x => x.CaseInfo.Entity).ToArray());
                return "删除选择的案例";
            }
            return "取消删除案例";
        }

        private String Open(RecentCaseEntityModel caseInfo)
        {
            Case currentCase = SystemContext.Instance.CurrentCase;
            if (currentCase != null && currentCase.CaseInfo.Id == caseInfo.CaseID)
            {
                return $"案例[{currentCase.Name}]已打开";
            }
            currentCase = Case.Open(caseInfo.CaseProjectFile);
            if (currentCase != null)
            {
                SystemContext.Instance.CurrentCase = currentCase;
                NavigationForMainWindow(ExportKeys.DeviceSelectView);
                CloseView();
                return $"打开案例[{currentCase.Name}]成功";
            }
            else
            {
                MessageBox.ShowDialogSuccessMsg(SystemContext.LanguageManager[Languagekeys.ViewLanguage_View_CaseNotExist]);
                return $"打开案例[{currentCase.Name}]失败";
            }
        }

        private String Remove(CaseItem item)
        {
            if (MessageBox.ShowDialogWarningMsg("是否确定删除？"))
            {
                Case @case = Case.Open(item.CaseInfo.CaseProjectFile);
                @case?.Delete();
                Items.Remove(item);
                DbService.Remove(item.CaseInfo.Entity);
                return "删除案例";
            }
            return "取消删除案例";
        }

        private String Search()
        {
            String keyword = FilterArgs.Keyword;
            DateTime? begin = FilterArgs.Begin;
            DateTime? end = FilterArgs.End;
            IQueryable<RecentCase> result = DbService.Records;
            if (!String.IsNullOrWhiteSpace(keyword))
            {
                if (begin != null && end != null)
                {
                    result = result.Where((e) => e.Name.Contains(keyword) || e.Number.Contains(keyword) && e.Timestamp >= begin.Value && e.Timestamp <= end.Value);
                }
                else if (begin != null && end == null)
                {
                    result = result.Where((e) => e.Name.Contains(keyword) || e.Number.Contains(keyword) && e.Timestamp >= begin.Value);
                }
                else if (begin == null && end != null)
                {
                    result = result.Where((e) => e.Name.Contains(keyword) || e.Timestamp <= end.Value);
                }
                else
                {
                    result = result.Where((e) => e.Name.Contains(keyword));
                }
            }
            else
            {
                if (begin != null && end != null)
                {
                    result = result.Where((e) => e.Timestamp >= begin.Value && e.Timestamp <= end.Value);
                }
                else if (begin != null && end == null)
                {
                    result = result.Where((e) => e.Timestamp >= begin.Value);
                }
                else if (begin == null && end != null)
                {
                    result = result.Where((e) => e.Timestamp <= end.Value);
                }
            }
            RecentCaseEntityModel[] models = result.OrderByDescending(y => y.Timestamp).ToModels<RecentCase, RecentCaseEntityModel>().ToArray();
            Int32 index = 1;
            var items = models.Select(z => new CaseItem(z, index++)).ToArray();
            Items = new ObservableCollection<CaseItem>(items);
            return "查询案例";
        }

        private void SelectAll(Boolean isSelectAll)
        {
            IsSelectAll = isSelectAll;
            foreach (var item in Items)
            {
                item.IsChecked = isSelectAll;
            }
        }

        private void Select(Boolean isChecked)
        {
            if (Items.All(x => x.IsChecked))
            {
                IsSelectAll = true;
            }
            else if (Items.All(x => !x.IsChecked))
            {
                IsSelectAll = false;
            }
            else
            {
                IsSelectAll = null;
            }
        }

        #endregion

        #endregion
    }
}
