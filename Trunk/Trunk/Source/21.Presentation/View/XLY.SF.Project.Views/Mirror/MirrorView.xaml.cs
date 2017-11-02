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

namespace XLY.SF.Project.Views.Mirror
{
    /// <summary>
    /// Mirror.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.MirrorView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MirrorView : UserControl
    {
        public MirrorView()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
