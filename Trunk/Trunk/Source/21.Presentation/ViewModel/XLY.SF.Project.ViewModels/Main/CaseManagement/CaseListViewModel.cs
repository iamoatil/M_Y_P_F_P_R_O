using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
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
    public class CaseListViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _openCommandProxy;

        private readonly ProxyRelayCommandBase _deleteCommandProxy;

        private readonly ProxyRelayCommandBase _searchCommandProxy;

        private readonly ProxyRelayCommandBase _selectAllCommandProxy;

        #endregion

        #region Constructors

        public CaseListViewModel()
        {
            FilterArgs = new CaseFilterArgs();
            _openCommandProxy = new ProxyRelayCommand<RecentCaseEntityModel>(Open);
            _deleteCommandProxy = new ProxyRelayCommand(Delete, CanDelete);
            _searchCommandProxy = new ProxyRelayCommand(Search);
            _selectAllCommandProxy = new ProxyRelayCommand<Boolean>(SelectAll);
        }

        #endregion

        #region Properties
        
        #region Items

        private IEnumerable<CaseItem> _items;
        public IEnumerable<CaseItem> Items
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

        public ICommand DeleteCommand => _deleteCommandProxy.ViewExecuteCmd;

        public ICommand SearchCommand => _searchCommandProxy.ViewExecuteCmd;

        public ICommand SelectAllCommand => _selectAllCommandProxy.ViewExecuteCmd;

        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }

        [Import(typeof(IDatabaseContext))]
        private IDatabaseContext DbService { get; set; }

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

        protected override void LoadCore(object parameters)
        {
            //PagingRequest paging = new PagingRequest(1, 100);
            //var result = DbService.QueryOfPaging<RecentCaseEntity, RecentCaseEntityModel>(paging, (e) => true);
            //PageCount = result.PageCount;
            var result = DbService.RecentCases.ToModels<RecentCase, RecentCaseEntityModel>().ToArray();
            Int32 index = 1;
            Items = result.OrderByDescending(x=>x.Timestamp).Select(x => new CaseItem(x, index++)).ToArray();
        }

        #endregion

        #region Private

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
                MessageBox.ShowNoticeMsg(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.ViewLanguage_View_CaseNotExist));
                return $"打开案例[{currentCase.Name}]失败";
            }
        }

        private String Delete()
        {
            if (_items == null) return "案例列表为空";
            var selected = _items.Where(x => x.IsChecked).ToArray();
            Case @case = null;
            foreach (CaseItem item in selected)
            {
                @case = Case.Open(item.CaseInfo.CaseProjectFile);
                if (@case == null) continue;
                @case.Delete();
            }
            DbService.RemoveRange(selected.Select(x=>x.CaseInfo).ToArray());
            Items = _items.Except(selected).ToArray();
            return "删除选择的案例";
        }

        private Boolean CanDelete()
        {
            if (_items == null) return false;
            return _items.Any(x => x.IsChecked);
        }

        private String Search()
        {
            String keyword = FilterArgs.Keyword;
            DateTime? begin = FilterArgs.Begin;
            DateTime? end = FilterArgs.End;
            IQueryable<RecentCase> result = DbService.RecentCases;
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
            Items = models.Select(z => new CaseItem(z, index++));
            return "查询案例";
        }

        private String SelectAll(Boolean isChecked)
        {
            if (_items == null) return "案例列表为空";
            foreach (CaseItem item in _items)
            {
                item.IsChecked = isChecked;
            }
            return "选择所有案例";
        }

        #endregion

        #endregion
    }
}
