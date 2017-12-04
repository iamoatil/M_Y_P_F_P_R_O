using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

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

            var preview = IocManagerSingle.Instance.GetViewPart(ExportKeys.DataPreView);
            if (preview != null)
            {
                preview.DataSource.LoadViewModel();
                _preVM = preview.DataSource;
                preView.Content = preview;

                MsgAggregation.Instance.RegisterGeneralMsg<object>(this, MessageKeys.PreviewKey, RefreshPreview);
            }
        }

        private ViewModelBase _preVM = null;

        private void RefreshPreview(object data)
        {
            if (data is GeneralArgs<object> g)
            {
                if (g.Parameters == null)
                {
                    _preVM?.Release();
                }
                else
                {
                    _preVM?.ReceiveParameters(g.Parameters);
                }
            }
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
