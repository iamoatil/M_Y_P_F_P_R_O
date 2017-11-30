using System.Windows.Controls;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// EarlyWarningSetting.xaml 的交互逻辑
    /// </summary>
    public partial class EarlyWarningSetting : UserControl
    {
        public EarlyWarningSetting()
        {
            InitializeComponent();
            SettingViewModel vm = new SettingViewModel();
            this.DataContext = vm;
        }
    }
}
