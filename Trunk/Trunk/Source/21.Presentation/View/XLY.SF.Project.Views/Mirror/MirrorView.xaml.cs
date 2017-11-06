using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Mirror
{
    /// <summary>
    /// Mirror.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.MirrorView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MirrorView : UcViewBase
    {
        public MirrorView()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.MirrorView, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return this.DataContext as ViewModelBase;
            }
            set
            {
                value.SetViewContainer(this);
                this.DataContext = value;
            }
        }       
    }
}
