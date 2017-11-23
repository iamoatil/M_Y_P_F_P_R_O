using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    [Export(ExportKeys.CaseSelectionViewModel, typeof(ViewModelBase))]
    public class CaseSelectionViewModel : ViewModelBase
    {
        #region Cosntructors

        public CaseSelectionViewModel()
        {
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

        #endregion

        #region Properties

        [Import(typeof(IRecordContext<RecentCase>))]
        private IRecordContext<RecentCase> DbService { get; set; }

        public RecentCaseEntityModel SelectedItem { get; set; }

        public ICommand ConfirmCommand { get; }

        public ICommand CancelCommand { get; }

        #region Cases

        private IEnumerable<RecentCaseEntityModel> _cases;
        public IEnumerable<RecentCaseEntityModel> Cases
        {
            get => _cases;
            private set
            {
                _cases = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public

        public override Object GetResult()
        {
            return SelectedItem;
        }

        #endregion

        #region Protected

        protected override void InitLoad(Object parameters)
        {
            Cases = DbService.Records.OrderByDescending(x => x.Timestamp).ToModels<RecentCase, RecentCaseEntityModel>().ToArray();
        }

        #endregion

        #region Private

        private void Confirm()
        {
            CloseView();
        }

        private void Cancel()
        {
            SelectedItem = null;
            CloseView();
        }

        #endregion

        #endregion
    }
}
