
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SQLite;
using XLY.SF.Project.EarlyWarningView;

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class DataDbColumnUpdater_Test
    {
        [TestMethod]
        public void TestAttach()
        {
            ExtractDir extractDir = new ExtractDir();
            extractDir.Initialize(@"C:\SPF\默认案例171226043615\H60-L01\自动提取\");
            extractDir.LoadDataSource();
            SqliteDataBaseFile dataDotDbFile = new SqliteDataBaseFile();
            dataDotDbFile.Initialize(extractDir.DbFile);

            DataDbColumnUpdater updater = new DataDbColumnUpdater();
            updater.Initialize(dataDotDbFile, @"C:\SPF\默认案例171226043615\H60-L01\configTmp.db");
            updater.AttachConfigDataBase();
            dataDotDbFile.Dispose();
        }
    }
}
