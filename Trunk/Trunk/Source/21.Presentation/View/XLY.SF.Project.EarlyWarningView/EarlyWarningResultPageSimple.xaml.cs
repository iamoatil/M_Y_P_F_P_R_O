using System.Windows.Controls;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// EarlyWarningResultPageSimple.xaml 的交互逻辑
    /// </summary>
    public partial class EarlyWarningResultPageSimple : UserControl
    {
        public EarlyWarningResultPageSimple()
        {
            InitializeComponent();
            ResultViewModel vm= new ResultViewModel();
            this.DataContext = vm.CategoryManager;
        }
    }
}
