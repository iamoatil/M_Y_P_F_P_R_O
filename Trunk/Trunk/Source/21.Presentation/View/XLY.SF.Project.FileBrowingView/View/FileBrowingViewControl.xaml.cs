using System.ComponentModel;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.FileBrowingView
{
    /// <summary>
    /// FileBrowingViewControl.xaml 的交互逻辑
    /// </summary>
    [Export("ExportKey_ModuleFileBowingView", typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class FileBrowingViewControl : UcViewBase
    {
        public FileBrowingViewControl()
        {
            InitializeComponent();
        }

        [Import("ExportKey_ModuleFileBowingViewModel", typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get => base.DataSource;
            set => base.DataSource = value;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
