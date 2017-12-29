using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// Sqlite生成的db文件对象。
    /// 例如：data.db文件的对象。此类不负责新建data.db文件。智能预警的逻辑中只使用一个data.db文件的对象，且只使用一个SQLiteConnection对象
    /// </summary>
    public class SqliteDataBaseFile : IDisposable
    {
        /// <summary>
        /// 是否已经初始化成功
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; private set; }

        //列更新器
        public readonly DataDbColumnUpdater ColumnUpdater = new DataDbColumnUpdater();

        /// <summary>
        /// 数据库连接
        /// </summary>
        public SQLiteConnection DbConnection
        {
            get { return _dbConnection; }
        }

        SQLiteConnection _dbConnection;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(string dataDbFile)
        {
            if(IsInitialized)
            {
                return;
            }
            IsInitialized = InnerInitialize(dataDbFile);
        }

        private bool InnerInitialize(string dataDbFile)
        {
            string dbConnectionStr=string.Format("Data Source='{0}'", dataDbFile);
            try
            {
                _dbConnection = new SQLiteConnection(dbConnectionStr);
                _dbConnection.Flags = SQLiteConnectionFlags.UseConnectionPool;
                _dbConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }
            Path = dataDbFile;
            return true;
        }

        /// <summary>
        /// 获取所有列表中sensitiveId为指定值的个数
        /// </summary>
        /// <param name="sensitiveId"></param>
        /// <returns></returns>
        internal int GetWarningCount(int sensitiveId)
        {
            if (!IsInitialized)
            {
                return 0;
            }
            // 获取数据库中所有的表名字
            List<string> tableNameList = new List<string>();
            using (SQLiteCommand cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = "select name from sqlite_master where type = 'table'";
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string tableName = reader["name"].ToString();
                    tableNameList.Add(tableName);
                }
            }
            //统计所有表中SensitiveId为指定值的个数
            int count = 0;
            using (SQLiteCommand cmd = _dbConnection.CreateCommand())
            {
                foreach (var tableName in tableNameList)
                {
                    cmd.CommandText = $"select count(*) from {tableName} where SensitiveId = '{sensitiveId}'";
                    string countStr=cmd.ExecuteScalar().ToString();
                    int c = 0;
                    bool isSuc=int.TryParse(countStr,out c);
                    if(isSuc)
                    {
                        count += c;
                    }
                }
            }
            return count;
        }

        #region IDisposable
        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~SqliteDataBaseFile()
        {
            Dispose(false);
        }

        //这里的参数表示示是否需要释放那些实现IDisposable接口的托管对象
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return; //如果已经被回收，就中断执行
            }
            if (disposing)
            {
                ColumnUpdater.Dispose();
                if (_dbConnection != null)
                {
                    _dbConnection.Dispose();
                }
                IsInitialized = false;
                //TODO:释放那些实现IDisposable接口的托管对象
            }
            //TODO:释放非托管资源，设置对象为null
            _disposed = true;
        }        
        #endregion
    }
}
