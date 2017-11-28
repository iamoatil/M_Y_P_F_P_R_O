using System;
using System.Windows;
using System.Windows.Input;


namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// MainWindows.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [STAThread]
        static void Main()
        {
            MainWindow win = new MainWindow();
            win.ShowDialog();
        }

        public MainWindow()
        {
            SettingCommand = new RelayCommand(
                () =>
                {
                    Window setting = new Window()
                    {
                        ShowInTaskbar = true,
                        Content = new EarlyWarningSetting()
                    };
                    setting.ShowDialog();
                });
             DetectCommand = new RelayCommand(Detect);
            InitializeComponent();
        }

        public ICommand SettingCommand { get; private set; }

        private void Detect()
        {
            _earlyWarning.Detect();
            ResultGrid.Children.Clear();
            ResultGrid.Children.Add(new EarlyWarningResultPage());
        }

        /// <summary>
        /// 智能预警
        /// </summary>
        DetectionManager _earlyWarning { get { return DetectionManager.Instance; } }

        /// <summary>
        /// 检测命令
        /// </summary>
        public ICommand DetectCommand { get; private set; }
    }
}
