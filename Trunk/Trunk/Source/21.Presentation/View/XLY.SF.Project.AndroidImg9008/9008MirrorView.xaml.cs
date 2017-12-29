using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.AndroidImg9008
{
    /// <summary>
    /// _9008MirrorView.xaml 的交互逻辑
    /// </summary>
    /// <summary>
    /// Mirror.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.Mirror9008View, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class _9008MirrorView : UcViewBase
    {
        public _9008MirrorView()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.Mirror9008ViewModel, typeof(ViewModelBase))]
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
