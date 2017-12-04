using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.DataExtraction
{
    /// <summary>
    /// ExtractionView.xaml 的交互逻辑
    /// </summary>
    [Export("XLY.SF.Project.DataExtraction.ExtractionView", typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ExtractionView : UcViewBase
    {
        #region Fields

        private Action<String> _selectionChangedCallback;

        #endregion

        #region Constructors

        public ExtractionView()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataSource = new ExtractionViewModel();
            }
        }

        #endregion

        #region Properties

        internal Boolean IsSelftHost
        {
            get => ((ExtractionViewModel)DataSource).IsSelfHost;
            set => ((ExtractionViewModel)DataSource).IsSelfHost = value;
        }

        #endregion

        #region Methods

        #region Public

        [Export("XLY.SF.Project.DataExtraction.ExtractionView.GetSelectedItems", typeof(Func<String[]>))]
        public String[] GetSelectedItems()
        {
            ExtractionViewModel vm = (ExtractionViewModel)DataSource;
            return vm.GetSelectedItems();
        }

        [Export("XLY.SF.Project.DataExtraction.ExtractionView.SetSelectedItems", typeof(Action<String[]>))]
        public void SetSelectedItems(params String[] ids)
        {
            ExtractionViewModel vm = (ExtractionViewModel)DataSource;
            vm.SetSelectedItems(ids);
        }

        [Export("XLY.SF.Project.DataExtraction.ExtractionView.SetSelectionChangedHandler", typeof(Action<Action<String>>))]
        public void SetSelectionChangedHandler(Action<String> selectionChangedCallback)
        {
            _selectionChangedCallback = selectionChangedCallback;
        }

        #endregion

        #region Event Handlers

        private void cb_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            ExtractionItem ei = (ExtractionItem)cb.DataContext;
            _selectionChangedCallback?.Invoke(ei.Target.Token);
        }

        #endregion

        #endregion
    }
}
