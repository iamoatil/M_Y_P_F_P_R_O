using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
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
            Thread.Sleep(1000*10000);
        }

        private async void DeviceMonitor_OnDeviceConnected(IDevice dev, bool isOnline)
        {
            AbsFileBrowsingService Service = FileBrowsingServiceFactory.GetFileBrowsingService(dev);
            FileBrowingNode rootNode = await Service.GetRootNode();

            List<FileBrowingNode> nodes = await Service.GetChildNodes(rootNode);
            FileBrowingNode curNode = null;
            foreach (var node in nodes)
            {
                if (node.Name == "splash2")
                {
                    curNode = node;
                }
            }

            if (curNode != null)
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                FileBrowingIAsyncTaskProgress fts = new FileBrowingIAsyncTaskProgress();
                await Service.Download(curNode, Path.GetTempPath(), true, cts, fts);
            }

            //OpenFileFolderNode(Roots[0]);
        }

       

        /// <summary>
        /// 加载节点数据
        /// </summary>
        /// <param name="doSomething"></param>
        public async void LoadingData(Action doSomething)
        {
            await Task.Run(doSomething);
        }
    }
}
