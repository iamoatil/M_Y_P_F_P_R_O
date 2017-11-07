using DllClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using XLY.SF.Framework.Core.Base.CoreInterface;
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
            this.Foreground = Brushes.Black;
            MirrorView.DataContext = _mirrorViewModel;
            this.Loaded += Window_Loaded;
            this.Unloaded += Window_Unloaded;
        }
        MirrorViewModel _mirrorViewModel = new MirrorViewModel();
        ServerHostManager serverHostManager = new ServerHostManager();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            serverHostManager.StartServerHost();            

            ProxyFactory.DeviceMonitor.OnDeviceConnected += (dev, isOnline) =>
            {
                this.Dispatcher.Invoke(new Action(() => DeviceMonitor_OnDeviceConnected(dev, isOnline)));
            };
            ProxyFactory.DeviceMonitor.OpenDeviceService();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            serverHostManager.StopServerHost();
        }      

        private void DeviceMonitor_OnDeviceConnected(IDevice dev, bool isOnline)
        {
            
            _mirrorViewModel.SourcePosition.RefreshPartitions(dev);

            SPFTask task = new SPFTask();
            task.Name = "TestName";
            task.Device = (Device)dev;
            IAsyncProgress asyncProgress = new DefaultAsyncProgress();
            asyncProgress.OnAdvance += (step, message) => { Console.WriteLine(string.Format("step:{0}  message:{1}", step, message)); };
            _mirrorViewModel.Initialize(task, asyncProgress);
        }
    }
}
