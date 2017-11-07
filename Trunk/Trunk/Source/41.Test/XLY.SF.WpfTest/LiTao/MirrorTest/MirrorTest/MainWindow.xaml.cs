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
            
        }
        MirrorViewModel _mirrorViewModel = new MirrorViewModel();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartServerHost();
            

            ProxyFactory.DeviceMonitor.OnDeviceConnected += (dev, isOnline) =>
            {
                this.Dispatcher.Invoke(new Action(() => DeviceMonitor_OnDeviceConnected(dev, isOnline)));
            };
            ProxyFactory.DeviceMonitor.OpenDeviceService();
        }

        /// <summary>
        /// 启动DllServerHost.exe的原因是，安卓镜像采用的dll（研究部提供）是x86的，而现在我们的SPPro是x64的，所以不能直接调用。
        /// 当前采取的策略是镜像功能作为一个独立的程序运行，其数据通过wcf传递给SPPro，这样就能解决X64调用X86的问题了。
        /// 
        /// 所以，镜像功能的测试需要先运行x86的镜像独立程序。
        /// </summary>
        private void StartServerHost()
        {
            const string serverHostPath = @"ServerHost\DllServerHost.exe";
            if (!File.Exists(serverHostPath))
            {
                throw new Exception("程序不能运行，请确保文件"+ serverHostPath+"存在");
            }
            Process.Start(serverHostPath);
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
