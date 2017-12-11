using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.DataDisplayView
{
    /// <summary>
    /// MainDisplayView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.DataDisplayView, typeof(UcViewBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class MainDisplayView : UcViewBase
    {
        public MainDisplayView()
        {
            InitializeComponent();

            AsyncOperator.LoadAsyncOperation(this);

            var preview = IocManagerSingle.Instance.GetViewPart(ExportKeys.DataPreView);
            if(preview != null)
            {
                preview.DataSource.LoadViewModel();
                _preVM = preview.DataSource;
                preView.Content = preview;

                MsgAggregation.Instance.RegisterGeneralMsg<object>(this, MessageKeys.PreviewKey, RefreshPreview);
            }
        }

        private ViewModelBase _preVM = null;

        [Import(ExportKeys.DataDisplayViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return base.DataSource as ViewModelBase;
            }
            set
            {
                //value.SetViewContainer(this);
                base.DataSource = value;
                (value as ViewModel.MainDisplayViewModel).FilterVM = filterCtrl.DataContext as ViewModel.DataFilterViewModel;
            }
        }

        private void RefreshPreview(object data)
        {
            if(data is Framework.Core.Base.MessageBase.GeneralArgs<object> g)
                _preVM?.ReceiveParameters(g.Parameters);
        }
    }
}
