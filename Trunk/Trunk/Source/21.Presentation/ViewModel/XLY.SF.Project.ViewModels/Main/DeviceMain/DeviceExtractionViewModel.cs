using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main.DeviceMain
{
    [Export(ExportKeys.ExtractionViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceExtractionViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProxyRelayCommandBase _addPlanCommandProxy;

        private readonly ProxyRelayCommandBase _removePlanCommandProxy;

        #endregion

        #region Constructors

        public DeviceExtractionViewModel()
        {
            Content = IocManagerSingle.Instance.GetPart<UcViewBase>("XLY.SF.Project.DataExtraction.ExtractionView");
            _addPlanCommandProxy = new ProxyRelayCommand(Add, base.ModelName,() => PlanName != String.Empty);
            _removePlanCommandProxy = new ProxyRelayCommand<ExtractionPlanModel>(Remove, base.ModelName);
        }

        #endregion

        #region Properties

        public ICommand AddPlanCommand => _addPlanCommandProxy.ViewExecuteCmd;

        public ICommand RemovePlanCommand => _removePlanCommandProxy.ViewExecuteCmd;

        public UcViewBase Content { get; }

        //[Import("XLY.SF.Project.DataExtraction.ExtractionView.GetSelectedItems", typeof(Func<String[]>))]
        //private Func<String[]> GetSelectedItems { get; set; }

        //[Import("XLY.SF.Project.DataExtraction.ExtractionView.SetSelectedItems", typeof(Action<String[]>))]
        //private Action<String[]> SetSelectedItems { get; set; }

        //[Import("XLY.SF.Project.DataExtraction.ExtractionView.SetSelectionChangedHandler", typeof(Action<Action<String>>))]
        //private Action<Action<String>> SetSelectionChangedHandler { get; set; }

        [Import(typeof(IRecordContext<ExtractionPlan>))]
        private IRecordContext<ExtractionPlan> DbService { get; set; }

        #region Plans

        private ObservableCollection<ExtractionPlanModel> _plans;
        public ObservableCollection<ExtractionPlanModel> Plans
        {
            get => _plans;
            private set
            {
                _plans = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region SelectedPlan

        private ExtractionPlanModel _selectedPlan;
        public ExtractionPlanModel SelectedPlan
        {
            get => _selectedPlan;
            set
            {
                //if (_selectedPlan == value) return;
                //_selectedPlan = value;
                //if (value != null)
                //{
                //    SetSelectedItems(SelectedPlan.ExtractItemTokens.ToArray());
                //}
                OnPropertyChanged();
            }
        }

        #endregion

        #region PlanName

        private String _planName = String.Empty;
        public String PlanName
        {
            get => _planName;
            set
            {
                if (value == null)
                {
                    _planName = String.Empty;
                }
                _planName = value.Trim();
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Protected

        protected override void InitLoad(object parameters)
        {
            Content.DataSource.LoadViewModel(parameters);
            var items = DbService.Records.ToModels<ExtractionPlan, ExtractionPlanModel>().ToArray();
            Plans = new ObservableCollection<ExtractionPlanModel>(items);
            //SetSelectionChangedHandler(SelectionChanged);
        }

        protected override void Closed()
        {
            //SetSelectionChangedHandler(null);
        }

        #endregion

        #region Private

        private String Add()
        {
            //String[] tokens = GetSelectedItems();
            //if (tokens.Length == 0) return "没有可添加的方案";
            //ExtractionPlanModel plan = new ExtractionPlanModel();
            //plan.Name = PlanName;
            //plan.ExtractItemTokens = tokens;
            //if (DbService.Add(plan.Entity))
            //{
            //    Plans.Add(plan);
            //    SelectedPlan = plan;
            //    return "添加方案成功";
            //}
            return "添加方案失败";
        }

        private String Remove(ExtractionPlanModel item)
        {
            //if (DbService.Remove(item.Entity))
            //{
            //    Plans.Remove(item);
            //    if (SelectedPlan == item)
            //    {
            //        SetSelectedItems(new String[0]);
            //    }
            //    return "删除方案成功";
            //}
            return "删除方案失败";
        }

        private void SelectionChanged(String selected)
        {
            SelectedPlan = null;
        }

        #endregion

        #endregion
    }
}
