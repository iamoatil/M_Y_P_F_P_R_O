using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EarlyWarningTest
{
    [TestClass]
    public class Md5ConfigFileDirTest
    {
        [TestMethod]
        public void GetAllData()
        {
            Md5ConfigFileDir dir = new Md5ConfigFileDir();
            dir.Initialize(@"D:\Test\");
            List<SensitiveData> ls = dir.GetAllData();
        }
    }
}
