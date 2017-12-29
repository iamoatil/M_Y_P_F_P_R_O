using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 提供数据库操作类（支持缓存连接）
    /// </summary>
    public class SqliteCache
    {
        private static Dictionary<string, SQLiteConnection> _connections = new Dictionary<string, SQLiteConnection>();
        private static Dictionary<string, SQLiteTransaction> _transactions = new Dictionary<string, SQLiteTransaction>();
        private static Dictionary<string, Tuple<DateTime, int>> _connectTimeout = new Dictionary<string, Tuple<DateTime, int>>();
        private static object _lockobj = new object();
        private static TimeoutTimer _timer;
        static SqliteCache()
        {
            _timer = new TimeoutTimer(500, ClearTimerCache);
        }

        private static void ClearTimerCache()
        {
            lock (_lockobj)
            {
                var timeItems = _connectTimeout.Where(item => item.Value.Item1.AddMilliseconds(item.Value.Item2) < DateTime.Now).Select(p => p.Key).ToList();
                foreach (var item in timeItems)
                {
                    CloseConnection(item);
                }
            }
        }

        private static string GetConnectString(string db)
        {
            return string.Format("Data Source='{0}'", db);
        }

        /// <summary>
        /// 获取或创建一个数据库连接，并设置超时自动断开时间
        /// </summary>
        /// <param name="db"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static SQLiteConnection GetConnection(string db, int timeout = 1000)
        {
            SQLiteConnection con;
            if (!_connections.ContainsKey(db))
            {
                con = new SQLiteConnection(GetConnectString(db));
                con.Open();
                _connections[db] = con;
                if (timeout > 0)
                {
                    lock (_lockobj)
                        _connectTimeout.Add(db, new Tuple<DateTime, int>(DateTime.Now, timeout));
                }
            }
            else
            {
                con = _connections[db];
            }
            
            return con;
        }

        /// <summary>
        /// 关闭一个数据库连接，如果db为null则全部关闭
        /// </summary>
        /// <param name="db"></param>
        public static void CloseConnection(string db)
        {
            if (db == null)
            {
                foreach (var item in _connections)
                {
                    CloseTransaction(item.Key);
                    item.Value.Close();
                    item.Value.Dispose();
                }
                _connections.Clear();
                _connectTimeout.Clear();
            }
            else
            {
                if (_connections.ContainsKey(db))
                {
                    CloseTransaction(db);
                    _connections[db].Close();
                    _connections.Remove(db);
                    _connectTimeout.Remove(db);
                }
            }
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public static void ExecuteNonQuery(string db, string sql, params SQLiteParameter[] parameters)
        {
            SQLiteConnection con = GetConnection(db);
            using (var com = new SQLiteCommand(con))
            {
                com.CommandText = sql;
                if (parameters != null && parameters.Length > 0)
                {
                    com.Parameters.AddRange(parameters);
                }
                com.ExecuteNonQuery();
            }
            ResetTimeout(db);
        }

        /// <summary>
        /// 批量执行sql语句
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="obj"></param>
        /// <param name="parameters"></param>
        /// <param name="action"></param>
        public static void ExecuteActionNonQuery(string db, string sql, IEnumerable<object> obj, SQLiteParameter[] parameters, Action<SQLiteParameterCollection, object> action)
        {
            SQLiteConnection con = GetConnection(db);
            using (var com = new SQLiteCommand(con))
            {
                com.CommandText = sql;

                if (parameters != null && parameters.Length > 0)
                {
                    com.Parameters.AddRange(parameters);
                }
                foreach (var item in obj)
                {
                    action(com.Parameters, item);
                    com.ExecuteNonQuery();
                }
            }
            ResetTimeout(db);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string db, string sql, params SQLiteParameter[] parameters)
        {
            SQLiteConnection con = GetConnection(db);
            using (var com = new SQLiteCommand(con))
            {
                com.CommandText = sql;
                if (parameters != null && parameters.Length > 0)
                {
                    com.Parameters.AddRange(parameters);
                }
                ResetTimeout(db);
                return com.ExecuteScalar();
            }
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable ExecuteReader(string db, string sql, params SQLiteParameter[] parameters)
        {
            SQLiteConnection con = GetConnection(db);

            using (var com = new SQLiteCommand(con))
            {
                com.CommandText = sql;
                if (parameters != null && parameters.Length > 0)
                {
                    com.Parameters.AddRange(parameters);
                }
                var reader = com.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                reader.Close();
                ResetTimeout(db);
                return dt;
            }
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="db"></param>
        public static void BeginTransaction(string db)
        {
            if (_transactions.ContainsKey(db))
                return;
            _transactions[db] = GetConnection(db).BeginTransaction();
        }

        /// <summary>
        /// 关闭事务
        /// </summary>
        /// <param name="db"></param>
        public static void CloseTransaction(string db)
        {
            if (_transactions.ContainsKey(db))
            {
                _transactions[db].Commit();
                _transactions.Remove(db);
            }
        }
        public static List<string> GetExistTables(string db)
        {
            var dt = ExecuteReader(db, "select name from sqlite_master where type='table' order by name");
            List<string> tables = new List<string>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    tables.Add(dr[0]?.ToString());
                }
            }
            return tables;
        }
        private static void ResetTimeout(string db)
        {
            lock (_lockobj)
                if (_connectTimeout.ContainsKey(db))
                {
                    _connectTimeout[db] = new Tuple<DateTime, int>(DateTime.Now, _connectTimeout[db].Item2);
                }
        }
    }
}
