using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Main
{
    /// <summary>
    /// CaseCreationView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.CaseCreationView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CaseCreationView : UcViewBase
    {
        public CaseCreationView()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.CaseCreationViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;
            }
        }
    }
}
