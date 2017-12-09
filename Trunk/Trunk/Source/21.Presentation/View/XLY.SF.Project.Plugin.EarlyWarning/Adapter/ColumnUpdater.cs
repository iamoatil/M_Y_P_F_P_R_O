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

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 数据的列更新器
    /// </summary>
    internal class ColumnUpdater : IDisposable
    {
        const string SensitiveId = "SensitiveId";
        const string SensitiveIDColumn = "SensitiveIDColumn";
        const string JsonColumnName = ConstDefinition.XLYJson;

        //源数据库的名字，表名，列名
        string _srcDbPath;
        string _srcTable;
        string _srcValueColumn { get { return _srcTable + "." + _keyColumn; } }

        //目标数据库的名字,表名，列名
        const string TgrDbAliasName = "configDbAliasName";
        const string TgrTable = TgrDbAliasName + ".TableName0";
        const string TgrValueColumn = TgrTable + ".ValueColumn";       

        //列名
        string _keyColumn;
        //源数据库的连接。使用完毕后注意Dispose
        SQLiteConnection _dbConnection;

        protected bool IsInitialized = false;

        /// <summary>
        /// 初始化操作需要的参数
        /// </summary>
        internal bool Initialize(string dbPath, string srcTableName, string configDbPath, string keyColumn)
        {
            IsInitialized = false;

            _srcDbPath = dbPath;
            _srcTable = srcTableName;
            _keyColumn = keyColumn;
            //连接数据库
            var connectionStringBuilder = new SQLiteConnectionStringBuilder()
            {
                DataSource = dbPath,
                Password = ""
            };
            _dbConnection = new SQLiteConnection(connectionStringBuilder.ConnectionString);
            _dbConnection.Flags = SQLiteConnectionFlags.UseConnectionPool;
            _dbConnection.Open();

            //检测是否指定的表和列是否存在
            string cmdText = $"select * from sqlite_master where type = 'table' and name = '{srcTableName}'";
            SQLiteCommand cmd = _dbConnection.CreateCommand();
            cmd.CommandText = cmdText;
            object ret = cmd.ExecuteScalar(CommandBehavior.SingleResult);
            if (ret == null)
            {
                return IsInitialized;
            }
            cmdText = $"select * from sqlite_master where name='{srcTableName}' and sql like '%{keyColumn}%'";
            cmd.CommandText = cmdText;
            ret = cmd.ExecuteScalar(CommandBehavior.SingleResult);
            if (ret == null)
            {
                return IsInitialized;
            }
            //附加configDbManager数据库 
            cmd.CommandText = $"attach database '{configDbPath}' as {TgrDbAliasName}";
            cmd.ExecuteNonQuery();

            IsInitialized = true;
            return IsInitialized;
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
        /// 关键字列更新数据
        /// </summary>
        internal void KeyWordColumnUpdate()
        {
            if (!IsInitialized)
            {
                return;
            }
            //更新：假设tableName为表A，{configDbAliasName}.TableName0为表B
            //     在表A的Url中找等于表B的ValueColumn的条目，然后把这些条目的SensitiveID修改成对应的表B的ValueColumn的值，把这些条目的XLYJson中SensitiveID值也修改过来。
            string cmdText = $"update {_srcTable} set {SensitiveId} =(select {SensitiveIDColumn} from {TgrTable} where  {_srcValueColumn} like '%'||{TgrValueColumn}||'%' limit 0,1)" +
                $" where {_srcValueColumn} in (select {_srcValueColumn} from {_srcTable} join {TgrTable} where {_srcValueColumn} like '%'||{TgrValueColumn}||'%')";
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

                    string newSensitiveStr = $"{ SensitiveId}\": {senId}";
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
        ~ColumnUpdater()
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
