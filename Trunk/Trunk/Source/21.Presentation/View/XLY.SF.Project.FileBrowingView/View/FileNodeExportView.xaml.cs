using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.FileBrowingView
{
    /// <summary>
    /// FileNodeExportView.xaml 的交互逻辑
    /// </summary>
    [Export("ExportKey_ModuleFileNodeExportView", typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class FileNodeExportView : UcViewBase
    {
        public FileNodeExportView()
        {
            InitializeComponent();
        }

        [Import("ExportKey_ModuleFileNodeExportViewModel", typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get => base.DataSource;
            set => base.DataSource = value;
        }
    }
}
