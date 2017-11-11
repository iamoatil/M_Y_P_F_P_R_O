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
            //_monitor.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_monitor.Close();
        }

        #endregion

        #region Private

        private void Connected(IDevice device)
        {
            Pump pump = CreatePump(device);
            if (pump == null) return;
            pump.SavePath = @"F:\Temp\Extraction";
            pump.DbFileName = "test.db";
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

        private static Pump CreatePump(IDevice device)
        {
            Pump pump = null;
            switch (device.DeviceType)
            {
                case EnumDeviceType.SDCard:
                    pump = new Pump { OSType = EnumOSType.SDCard, Type = EnumPump.SDCard };
                    break;
                case EnumDeviceType.SIM:
                    pump = new Pump { OSType = EnumOSType.SIMCard, Type = EnumPump.SIMCard };
                    break;
                case EnumDeviceType.Phone when device is Device dev:
                    pump = new Pump { OSType = dev.OSType, Type = EnumPump.USB };
                    break;
                default:
                    return null;
            }
            pump.Source = device;
            pump.ScanModel = ScanFileModel.Expert;
            pump.SavePath = @"F:\Temp";
            return pump;
        }

        #endregion

        #endregion
    }
}
