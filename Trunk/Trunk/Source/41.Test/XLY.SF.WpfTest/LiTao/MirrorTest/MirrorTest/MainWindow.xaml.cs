using System;
using System.Threading;
using System.Windows;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ProxyService;
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
            MirrorView.DataContext = _mirrorViewModel;
            this.Loaded += Window_Loaded;
        }

        MirrorViewModel _mirrorViewModel = new MirrorViewModel();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProxyFactory.DeviceMonitor.OnDeviceConnected += (dev, isOnline) =>
            {
                this.Dispatcher.Invoke(new Action(() => DeviceMonitor_OnDeviceConnected(dev, isOnline)));
            };
            ProxyFactory.DeviceMonitor.OpenDeviceService();

        }
        private void DeviceMonitor_OnDeviceConnected(IDevice dev, bool isOnline)
        {
            _mirrorViewModel.SourcePosition.InitializeDevice(dev);
        }
    }
}
