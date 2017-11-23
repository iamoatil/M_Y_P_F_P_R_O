using System.ComponentModel;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.FileBrowingView
{
    /// <summary>
    /// FileBrowingViewControl.xaml 的交互逻辑
    /// </summary>
    [Export("ExportKey_FileBrowingView", typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class FileBrowingViewControl : UcViewBase
    {
        public FileBrowingViewControl()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataSource = new FileBrowingViewModel();
            }
            else
            {
                DataSource = new FileBrowingViewModel();
            }
        }
    }
}
