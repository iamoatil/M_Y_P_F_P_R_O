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
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.DataPreview
{
    /// <summary>
    /// MainPreview.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.DataPreView, typeof(UcViewBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class MainPreview : UcViewBase
    {
        public MainPreview()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.DataPreViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return base.DataContext as ViewModelBase;
            }
            set
            {
                //value.SetViewContainer(this);
                if(this.DataContext != value)
                {
                    base.DataContext = value;
                }                
            }
        }
    }
}
