/* ==============================================================================
* Description：SourceDbUpdater  
* Author     ：litao
* Create Date：2017/12/6 20:24:43
* ==============================================================================*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// data.db文件的列更新器
    /// 智能预警的逻辑中只且只使用一个SQLiteConnection对象
    /// </summary>
    public class DataDbColumnUpdater : IDisposable
    {
        const string SensitiveId = "SensitiveId";
        const string SensitiveIDColumn = "SensitiveIDColumn";
        const string JsonColumnName = ConstDefinition.XLYJson;

        //源数据库的名字，表名，列名
       
        string _srcTable;
        string _srcValueColumn { get { return _srcTable + "." + _keyColumn; } }

        //目标数据库的名字,表名，列名
        string _configDbPath;
        const string TgrDbAliasName = "configDbAliasName";
        const string TgrTable = TgrDbAliasName + ".TableName0";
        const string TgrValueColumn = TgrTable + ".ValueColumn";       

        //列名
        string _keyColumn;

        SQLiteConnection _dbConnection;
        protected bool IsInitialized = false;

        protected bool IsAttached = false;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(SqliteDataBaseFile dataDotDbFile, string configDbPath)
        {
            IsInitialized = InnerInitialize(dataDotDbFile, configDbPath);
        }

        private bool InnerInitialize(SqliteDataBaseFile dataDbFile, string configDbPath)
        {
            string dbFile = dataDbFile.Path;
            if(!File.Exists(dbFile))
            {
                return false;
            }

            _dbConnection = dataDbFile.DbConnection;
            _configDbPath = configDbPath;

            return true;
        }

        public bool CheckTableAndColumn(string srcTableName,string keyColumn)
        {
            _srcTable = srcTableName;
            _keyColumn = keyColumn;
            //连接数据库

            //检测是否指定的表和列是否存在
            string cmdText = $"select * from sqlite_master where type = 'table' and name = '{srcTableName}'";
            SQLiteCommand cmd = _dbConnection.CreateCommand();
            cmd.CommandText = cmdText;
            object ret = cmd.ExecuteScalar(CommandBehavior.SingleResult);
            if (ret == null)
            {
                return false;
            }
            cmdText = $"select * from sqlite_master where name='{srcTableName}' and sql like '%{keyColumn}%'";
            cmd.CommandText = cmdText;
            ret = cmd.ExecuteScalar(CommandBehavior.SingleResult);
            if (ret == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 附加config的数据
        /// </summary>
        public void AttachConfigDataBase()
        {
            if (!IsInitialized)
            {
                return;
            }
            if(IsAttached)
            {
                return;
            }
            //附加configDbManager数据库 
            SQLiteCommand cmd = _dbConnection.CreateCommand();
            cmd.CommandText = $"attach database '{_configDbPath}' as {TgrDbAliasName}";
            cmd.ExecuteNonQuery();
            IsAttached = true;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        internal void Update()
        {
            if (!IsInitialized)
            {
                return;
            }
            //更新：假设tableName为表A，{configDbAliasName}.TableName0为表B
            //     在表A的Url中找等于表B的ValueColumn的条目，然后把这些条目的SensitiveID修改成对应的表B的ValueColumn的值，把这些条目的XLYJson中SensitiveID值也修改过来。
            string cmdText = $"update {_srcTable} set {SensitiveId} = (select {SensitiveIDColumn} from {TgrTable} where {TgrValueColumn} = {_srcValueColumn} limit 0,1) " +
                $"where {_srcValueColumn} in (select {TgrValueColumn} from {TgrTable})";
            SQLiteCommand cmd = _dbConnection.CreateCommand();
            cmd.CommandText = cmdText;
            cmd.ExecuteNonQuery();

            XLYJsonColumnUpdate();
        }
        

        /// <summary>
        /// 更新XLYJson列:先获取已经更改的数据项，然后修改这些数据项的Json列的数据
        /// </summary>
        private void XLYJsonColumnUpdate()
        {
            if (!IsInitialized)
            {
                return;
            }

            //先获取已经更改的数据项
            string cmdText = $"select {JsonColumnName},{SensitiveId} from {_srcTable} join {TgrTable} on {_srcValueColumn} like '%'||{TgrValueColumn}||'%'";
            SQLiteCommand cmd = _dbConnection.CreateCommand();
            cmd.CommandText = cmdText;
            DbDataReader reader = cmd.ExecuteReader();
            List<string> cmds = new List<string>();
            while (reader.Read())
            {
                string jsonStr = reader[JsonColumnName].ToString();
                string senId = reader[SensitiveId].ToString();

                int startIndex = jsonStr.IndexOf(SensitiveId);
                if(startIndex > 0 )
                {
                    int stopIndex = jsonStr.IndexOf("\r", startIndex);

                    string newSensitiveStr = $"{ SensitiveId}\": {senId},";
                    string newJsonStr = jsonStr.Replace(jsonStr.Substring(startIndex, stopIndex - startIndex), newSensitiveStr);

                    string cmdString = $"update {_srcTable} set {JsonColumnName} = '{newJsonStr}' where {JsonColumnName} = '{jsonStr}'";
                    cmds.Add(cmdString);
                }                
            }
            reader.Close();
            //修改这些数据项的Json列的数据
            foreach (string cmdstr in cmds)
            {
                cmd = _dbConnection.CreateCommand();
                cmd.CommandText = cmdstr;
                cmd.ExecuteNonQuery();
            }
            
        }

        #region IDisposable
        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DataDbColumnUpdater()
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
                //去掉附加configDbManager数据库 
                try
                {
                    SQLiteCommand cmd = _dbConnection.CreateCommand();
                    cmd.CommandText = $"detach database '{TgrDbAliasName}'";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
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
