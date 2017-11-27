using System.Windows.Controls;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// EarlyWarningMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class EarlyWarningResultPage : UserControl
    {
        public EarlyWarningResultPage()
        {
            InitializeComponent();
            ResultViewModel vm= new ResultViewModel();
            this.DataContext = vm.CategoryManager;
        }
    }
}
