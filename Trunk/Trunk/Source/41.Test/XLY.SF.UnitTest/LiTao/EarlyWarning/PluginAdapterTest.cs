using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.EarlyWarningView;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Persistable;

/* ==============================================================================
* Description：PluginAdapterTest  
* Author     ：litao
* Create Date：2017/12/5 19:34:33
* ==============================================================================*/

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class PluginAdapterTest
    {
        [TestMethod]
        public void RunPluginAdapter()
        {
            //模拟一个
            IRecordContext<Inspection> Setting = new Settings();
            
            EarlyWarningPluginAdapter adapter = new EarlyWarningPluginAdapter();
            adapter.Initialize(Setting);
            adapter.Detect("");
        }
    }
}
