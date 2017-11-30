using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using XLY.SF.Project.EarlyWarningView;

namespace EarlyWarningTest
{
    [TestClass]
    public class EarlyWarningTest
    {
        [TestMethod]
        public void Intialize()
        {
            EarlyWarning earlyWarning = new EarlyWarning();
            bool ret=earlyWarning.Initialize();
        }
        
    }
}
