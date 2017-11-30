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
    class ConfigDbManager
    {
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
        
        /// <summary>
        /// 生成数据库文件
        /// </summary>
        /// <param name="DbDatas"></param>
        public void GenerateDbFile(IEnumerable<SensitiveData> DbDatas)
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
            SQLiteConnection dbConnection = new SQLiteConnection(string.Format("Data Source={0}", DbPath));
            dbConnection.Open();
            //创建Table
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0}('{1}' TEXT,'{2}' TEXT ,'{3}' TEXT);", TableName, RootNodeColumn, CategoryColumn, ValueColumn);
            SQLiteCommand command = new SQLiteCommand(sb.ToString(), dbConnection);
            command.ExecuteNonQuery();
            //添加数据
            sb = new StringBuilder();
            foreach (SensitiveData item in DbDatas)
            {
                sb.AppendFormat("insert into {0} values('{1}','{2}','{3}');", TableName, item.RootNodeName,item.CategoryName,item.Value);
            }
            command = new SQLiteCommand(sb.ToString(), dbConnection);
            command.ExecuteNonQuery();
        }
    }    
}
