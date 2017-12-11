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
    public class Md5FileTest
    {
        [TestMethod]
        public void FileBrowingTest()
        {
            ProxyFactory.DeviceMonitor.OnDeviceConnected += (dev, isOnline) =>
            {
                DeviceMonitor_OnDeviceConnected(dev, isOnline);
            };
            ProxyFactory.DeviceMonitor.OpenDeviceService();
            Thread.Sleep(1000*5);
        }

        private async void DeviceMonitor_OnDeviceConnected(IDevice dev, bool isOnline)
        {
            FileMd5DataParser fileMd5DataParser = new FileMd5DataParser();
            fileMd5DataParser.DownloadDirectory(dev,new List<string>() { @"/splash2"},@"d:\Test");
        }        
    }
}
