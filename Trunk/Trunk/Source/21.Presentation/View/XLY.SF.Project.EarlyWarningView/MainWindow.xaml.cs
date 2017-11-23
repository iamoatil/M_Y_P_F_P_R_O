using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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

            InitializeComponent();
        }

        public ICommand SettingCommand { get; private set; }
    }
}
