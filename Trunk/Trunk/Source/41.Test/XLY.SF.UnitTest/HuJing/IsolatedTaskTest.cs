using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.IsolatedTaskEngine;

namespace XLY.SF.UnitTest.HuJing
{
    [TestClass]
    public class IsolatedTaskTest
    {
        [TestMethod]
        public void TestConfig()
        {
            EngineSetup setupInfo = new EngineSetup("isolatedTask", @"F:\Source\Workspaces\SPF-PRO\Trunk\Trunk\Source\41.Test\XLY.SF.UnitTest\HuJing\IsolatedTask.config");
        }
    }
}
