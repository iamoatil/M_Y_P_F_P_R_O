using System.Windows;
using XLY.SF.Project.ViewModels.Main;

namespace MirrorTest
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            MirrorView.DataContext = new MirrorViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
