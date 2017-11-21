using System;
using System.Collections.Generic;
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
using XLY.SF.Project.DataDisplayView.ViewModel;

namespace XLY.SF.Project.DataDisplayView
{
    /// <summary>
    /// DataFilterControl.xaml 的交互逻辑
    /// </summary>
    public partial class DataFilterControl : UserControl
    {
        public DataFilterControl()
        {
            InitializeComponent();

            this.DataContext = new DataFilterViewModel();
        }
    }
}
