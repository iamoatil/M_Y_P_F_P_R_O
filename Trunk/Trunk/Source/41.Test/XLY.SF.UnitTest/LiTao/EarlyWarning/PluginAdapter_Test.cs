using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using XLY.SF.Project.EarlyWarningView;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Persistable;
using XLY.SF.Project.ProxyService;

/* ==============================================================================
* Description：PluginAdapterTest  
* Author     ：litao
* Create Date：2017/12/5 19:34:33
* ==============================================================================*/

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class PluginAdapter_Test
    {
        [TestMethod]
        public void RunPluginAdapter()
        {
           // 模拟一个
           /// <summary>
           ///通过导入获取setting对象。其用于读取数据库中的数据
           /// </summary>
        //[Import(typeof(IRecordContext<Inspection>))]
        //public IRecordContext<Inspection> Setting { get; set; }
        IRecordContext<Inspection> Setting = null;

            EarlyWarningPluginAdapter adapter = new EarlyWarningPluginAdapter();
            adapter.Initialize(Setting);
            IDevice[] devs = ProxyFactory.DeviceMonitor.GetCurConnectedDevices();
            adapter.Md5WarningManager.FileDownloader.SetDevice(devs[0]);
            adapter.Detect(@"D:\XLY\SpfData\手里全部提取_20171204[111147]\H60-L01_20171204[111149]\自动提取\");
        }
    }
}
