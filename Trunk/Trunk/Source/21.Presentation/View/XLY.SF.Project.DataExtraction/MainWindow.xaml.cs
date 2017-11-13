using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using XLY.SF.Project.DataExtract;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;

namespace XLY.SF.Project.DataExtraction
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private readonly AndroidDeviceMonitor _monitor = new AndroidDeviceMonitor();

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _monitor.DeviceConnected = Connected;
        }

        #endregion

        #region Methods

        #region Event Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PluginAdapter.Instance.Initialization(null);
            _monitor.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _monitor.Close();
        }

        #endregion

        #region Private

        private void Connected(IDevice device)
        {
            Pump pump = CreatePump(@"F:\Test\Extraction", "test.db", device);
            if (pump == null) return;
            var allexts = PluginAdapter.Instance.GetAllExtractItems(pump);

            DataExtractionParams @params = new DataExtractionParams
            {
                Pump = pump,
                Items = new List<ExtractItem>(allexts),
            };
            Dispatcher.BeginInvoke((Action)delegate()
            {
                ExtractionViewModel vm = (ExtractionViewModel)EV.DataContext;
                vm.Args = @params;
            });
        }

        private static Pump CreatePump(String savePath,String dbFileName, IDevice device)
        {
            Pump pump = new Pump(savePath, dbFileName);
            switch (device.DeviceType)
            {
                case EnumDeviceType.SDCard:
                    pump.OSType = EnumOSType.SDCard;
                    pump.Type = EnumPump.SDCard;
                    break;
                case EnumDeviceType.SIM:
                    pump.OSType = EnumOSType.SIMCard;
                    pump.Type = EnumPump.SIMCard;
                    break;
                case EnumDeviceType.Phone when device is Device dev:
                    pump.OSType = dev.OSType;
                    pump.Type = EnumPump.USB;
                    break;
                default:
                    return null;
            }
            pump.Source = device;
            pump.ScanModel = ScanFileModel.Expert;
            return pump;
        }

        #endregion

        #endregion
    }
}
