using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using XLY.SF.Project.EarlyWarningView;
using XLY.SF.Project.ProxyService;
using XLY.SF.Project.Services;

/* ==============================================================================
* Description：Md5FileTest  
* Author     ：litao
* Create Date：2017/12/8 15:34:39
* ==============================================================================*/

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class FileDownloader_Test
    {
        [TestMethod]
        public void DownloaderTest()
        {
            ProxyFactory.DeviceMonitor.OnDeviceConnected += (dev, isOnline) =>
            {
                DeviceMonitor_OnDeviceConnected(dev, isOnline);
            };
            ProxyFactory.DeviceMonitor.OpenDeviceService();
            Thread.Sleep(1000*30);
        }

        private async void DeviceMonitor_OnDeviceConnected(IDevice dev, bool isOnline)
        {
            FileDownloader fileDownloader = new FileDownloader();
            fileDownloader.Initialize(dev,@"d:\Test\",new List<string>() { @"/splash2"});
            fileDownloader.DownloadDirectory();
        }        
    }
}
