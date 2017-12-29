using ProjectExtend.Context;
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
using XLY.SF.Framework.Language;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Management.Guide
{
    /// <summary>
    /// AboutUsView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.AboutUsView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AboutUsView : UcViewBase
    {
        public AboutUsView()
        {
            InitializeComponent();
        }
        [Import(ExportKeys.AboutUsViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource { get => base.DataSource; set => base.DataSource = value; }
    }
}
