using ProjectExtend.Context;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.CameraView
{
    /// <summary>
    /// TakePhoto.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.TakePhotoView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TakePhoto : UcViewBase
    {
        public TakePhoto()
        {
            AppThread.Instance.Initialize();
            InitializeComponent();
            this.Unloaded += TakePhoto_Unloaded;
        }

        private void TakePhoto_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            player.Stop();
        }

        [Import(ExportKeys.TakePhotoViewModel, typeof(ViewModelBase))]
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
