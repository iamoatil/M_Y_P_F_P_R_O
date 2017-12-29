/* ==============================================================================
* Description：ConfigDbManager  
* Author     ：litao
* Create Date：2017/11/29 19:42:55
* ==============================================================================*/

using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Linq;
using System.Data.Common;
using System.Diagnostics;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 其中实现了： 把配置文件转换成sqlite数据文件的功能
    /// </summary>
    class DbFromConfigData
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 临时的db文件路径。只有在初始化成功后才能使用
        /// </summary>
        public string DbPath { get; private set; }      

        public readonly string TableName = "TableName0";
        public readonly string RootNodeColumn = "RootNodeColumn";
        public readonly string CategoryColumn = "CategoryColumn";
        public readonly string ValueColumn = "ValueColumn";
        public readonly string SensitiveIDColumn = "SensitiveIDColumn";

        private void CreateDbTable()
        {
            //创建数据库
            if (!File.Exists(DbPath))
            {
                var dir = Path.GetDirectoryName(DbPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                SQLiteConnection.CreateFile(DbPath);
            }
            //打开数据
            using (SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}", DbPath)))
            {
                dbConnection.Open();
                //创建Table
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0}('{1}' TEXT,'{2}' TEXT ,'{3}' TEXT,'{4}' TEXT);", TableName, RootNodeColumn, CategoryColumn, ValueColumn, SensitiveIDColumn);
                SQLiteCommand command = new SQLiteCommand(sb.ToString(), dbConnection);
                command.ExecuteNonQuery();
            }           
        }

        /// <summary>
        /// 使用一个目录来指定configTmp.db的位置，并且创建数据库和对应的table
        /// </summary>
        public void Initialize(string dir)
        {
            _isInitialized = false;

            //修整dir
            if(!dir.EndsWith("\\"))
            {
                dir += "\\";
            }
            string configTmp = dir + "configTmp.db";
            DbPath = configTmp;
            CreateDbTable();

            _isInitialized = true;
        }

        /// <summary>
        /// 把配置文件转换成sqlite数据文件作为数据源
        /// </summary>
        /// <param name="DbDatas"></param>
        public void GenerateDbFile(IEnumerable<SensitiveData> DbDatas)
        {
            if(!_isInitialized)
            {
                return;
            }
            //打开数据
            using (SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}", DbPath)))
            {
                dbConnection.Open();
                //删除数据库中所有数据
                SQLiteCommand deleteCmd = new SQLiteCommand($"delete from {TableName}", dbConnection);
                deleteCmd.ExecuteNonQuery();

                CreateDbTable();
                //添加数据
                DbTransaction trans = dbConnection.BeginTransaction();
                try
                {
                    SQLiteCommand insertCmd = new SQLiteCommand(dbConnection);
                    foreach (SensitiveData item in DbDatas)
                    {
                        insertCmd.CommandText = $"insert into {TableName} values('{item.RootNodeName}','{item.CategoryName}','{item.Value}','{item.SensitiveId}')";
                        insertCmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
                catch (System.Exception)
                {
                    trans.Rollback();
                    Trace.WriteLine("数据添加失败！");
                } 
            }           
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset()
        {
            //删除数据库
            if (File.Exists(DbPath))
            {
                File.Delete(DbPath);
            }
        }
    }
}
