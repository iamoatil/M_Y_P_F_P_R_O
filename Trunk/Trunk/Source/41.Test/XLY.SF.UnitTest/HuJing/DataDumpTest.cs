using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.DataPump;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class DataDumpTest
    {
        #region Fields

        private readonly AbstractDeviceMonitor _deviceMonitor;

        #endregion

        #region Constructors

        public DataDumpTest()
        {
            _deviceMonitor = new AndroidDeviceMonitor();
            _deviceMonitor.DeviceConnected = Connected;
            _deviceMonitor.Start();
        }

        #endregion

        [TestMethod]
        public void TestExecuteMethod(Pump pump, SourceFileItem source, MultiTaskReporterBase reporter, params ExtractItem[] items)
        {
            DataPumpExecutionContext context = pump.Execute(source, reporter, items);
        }

        [TestMethod]
        public void TestCancelMethod(Pump pump, SourceFileItem source, MultiTaskReporterBase reporter, params ExtractItem[] items)
        {
            DataPumpExecutionContext context = pump.Execute(source, reporter, items);
            context.Cancel();
        }

        #region Private

        private Pump CreatePump(String savePath,String dbFileName, IDevice device, out SourceFileItem souce)
        {
            SourceFileItem item = new SourceFileItem();
            item.ItemType = SourceFileItemType.NormalPath;
            item.Config = @"/system/build.prop";
            souce = item;

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

        private void Connected(IDevice device)
        {
            Pump pump = CreatePump(@"F:\Temp", "a.db", device, out SourceFileItem item);
            if (pump == null) return;
            TestExecuteMethod(pump, item, null);
        }

        #endregion
    }
}
