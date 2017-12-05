/* ==============================================================================
* Description：ConfigDbManager  
* Author     ：litao
* Create Date：2017/11/29 19:42:55
* ==============================================================================*/

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Xml;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 把配置文件转换成sqlite数据文件作为数据源
    /// </summary>
    class ConfigDataToDB
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 配置文件所在目录
        /// </summary>
        protected string Dir { get; set; }

        /// <summary>
        /// 临时的db文件路径。只有在初始化成功后才能使用
        /// </summary>
        public string DbPath
        {
            get
            {
                return _dbPath;
            }
        }
        string _dbPath = Path.GetFullPath("configTmp.db");

        public readonly string TableName = "TableName0";
        public readonly string RootNodeColumn = "RootNodeColumn";
        public readonly string CategoryColumn = "CategoryColumn";
        public readonly string ValueColumn = "ValueColumn";

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
                sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0}('{1}' TEXT,'{2}' TEXT ,'{3}' TEXT);", TableName, RootNodeColumn, CategoryColumn, ValueColumn);
                SQLiteCommand command = new SQLiteCommand(sb.ToString(), dbConnection);
                command.ExecuteNonQuery();
            }           
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            _isInitialized = false;
            CreateDbTable();
            _isInitialized = true;
        }

        /// <summary>
        /// 生成数据库文件
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

                //记录，当前数据库中没有条目
                List<SensitiveData> ls = new List<SensitiveData>();
                foreach (SensitiveData item in DbDatas)
                {
                    SQLiteCommand cmd = new SQLiteCommand(string.Format("select * from {0} where {1} = '{2}' and {3} = '{4}' and {5} = '{6}'",
                        TableName, RootNodeColumn, item.RootNodeName, CategoryColumn, item.CategoryName, ValueColumn, item.Value),
                        dbConnection);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        ls.Add(item);
                    }
                }

                //添加数据
                StringBuilder sb = new StringBuilder();
                foreach (SensitiveData item in ls)
                {
                    sb.AppendFormat("insert into {0} values('{1}','{2}','{3}');", TableName, item.RootNodeName, item.CategoryName, item.Value);
                }
                SQLiteCommand command = new SQLiteCommand(sb.ToString(), dbConnection);
                command.ExecuteNonQuery();
            }           
        }
    }
}
