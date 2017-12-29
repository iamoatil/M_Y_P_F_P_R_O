using GalaSoft.MvvmLight.Threading;
using System;
using System.Windows;
using System.Windows.Media;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ProxyService;

namespace XLY.SF.Project.MirrorView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Foreground = Brushes.Black;
            MirrorView.DataContext = _mirrorViewModel;
            this.Loaded += Window_Loaded;
            this.Closed += MainWindow_Closed; ;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
        }

        CmdMirrorViewModel _mirrorViewModel = new CmdMirrorViewModel();


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProxyFactory.DeviceMonitor.OnDeviceConnected += (dev, isOnline) =>
            {
                this.Dispatcher.Invoke(new Action(() => DeviceMonitor_OnDeviceConnected(dev, isOnline)));
            };
            ProxyFactory.DeviceMonitor.OpenDeviceService();

            //初始化MvvmLigth的DispatcherHelper
            DispatcherHelper.Initialize();
        }

        private void DeviceMonitor_OnDeviceConnected(IDevice dev, bool isOnline)
        {
            if (dev is Device device)
            {
                _mirrorViewModel.SourcePosition.RefreshPartitions(device);
                _mirrorViewModel.Initialize(dev.ID);
            }
        }
    }
}
