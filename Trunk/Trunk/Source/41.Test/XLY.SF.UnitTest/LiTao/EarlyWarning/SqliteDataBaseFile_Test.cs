using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SQLite;
using XLY.SF.Project.EarlyWarningView;

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class SqliteDataBaseFile_Test
    {
        [TestMethod]
        public void Test()
        {
            SqliteDataBaseFile sqlFile = new SqliteDataBaseFile();
            sqlFile.Initialize(@"C:\SPF\默认案例171228054327\H60-L01_1\自动提取\data.db");
            SQLiteConnection con = sqlFile.DbConnection;
            SQLiteCommand cmd=con.CreateCommand();
            cmd.CommandText = "select name from sqlite_master where type = 'table'";
            cmd.CommandText = "select count(*) from Table_AutoEarlyWarning where SensitiveId = '100'";
            string count=cmd.ExecuteScalar().ToString();
            //SQLiteDataReader reader=cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    string name=reader["name"].ToString();
            //}
            //int count=sqlFile.GetWarningCount(1);
        }

        [TestMethod]
        public void TestConfigFile()
        {
            ConfigFile file = new ConfigFile();
            file.Initialize(@"C:\SPF\默认案例171228054327\H60-L01_1\自动提取\EarlyWarningConfig\PublicSafety.xml");
            file.SetWarningCount(100);
        }

    }
}
