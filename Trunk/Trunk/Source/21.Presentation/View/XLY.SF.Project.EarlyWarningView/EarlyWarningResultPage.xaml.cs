using System.ComponentModel.Composition;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.DataDisplayView;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// EarlyWarningMainPage.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.AutoWarningView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public partial class EarlyWarningResultPage : UcViewBase
    {
        public EarlyWarningResultPage()
        {
            InitializeComponent();
            
            AsyncOperator.LoadAsyncOperation(this);

            //var preview = IocManagerSingle.Instance.GetViewPart(ExportKeys.DataPreView);
            //if (preview != null)
            //{
            //    preview.DataSource.LoadViewModel();
            //    _preVM = preview.DataSource;
            //    preView.Content = preview;

            //    MsgAggregation.Instance.RegisterGeneralMsg<object>(this, MessageKeys.PreviewKey, RefreshPreview);
            //}
        }

        /// <summary>
        ///通过导入获取setting对象。其用于读取数据库中的数据
        /// </summary>
        [Import(typeof(IRecordContext<Inspection>))]
        public IRecordContext<Inspection> Setting { get; set; }

        private ViewModelBase _preVM = null;

        [Import(ExportKeys.AutoWarningViewModel, typeof(ViewModelBase))]
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

        private void RefreshPreview(object data)
        {
            if (data is Framework.Core.Base.MessageBase.GeneralArgs<object> g)
                _preVM?.ReceiveParameters(g.Parameters);
        }

        private void UcViewBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //DetectionManager.Instance.ConfigDataFilter.Setting = Setting;
            //DetectionManager.Instance.Detect(@"D:\XLY\SpfData\有自动提取数据的_20171130[052023]\H60-L01_20171130[052024]\");
        }
    }
}
