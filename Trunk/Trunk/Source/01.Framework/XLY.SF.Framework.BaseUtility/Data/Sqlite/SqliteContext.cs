﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// SQLite操作上下文
    /// </summary>
    public class SqliteContext : IDisposable
    {
        #region Properties

        /// <summary>
        /// 数据源配置
        /// </summary>
        public string DataSource { get; set; }
        public SQLiteConnectionStringBuilder builder;
        #endregion

        #region 构造函数
        /// <summary>
        /// SQLite操作帮助类
        /// 参数filePath为文件绝对路径
        /// </summary>
        public SqliteContext(string filePath)
        {
            DataSource = String.Format("Data Source={0}", filePath);
            DbFilePath = filePath;
        }

        /// <summary>
        /// DB文件全路径
        /// </summary>
        public string DbFilePath { get; set; }

        /// <summary>
        /// Sqlite操作上下文构造函数
        /// 由于DF项目目前读取sqlite没有先把数据源全部拷贝出来，但设备又是只读的，所以只能暂时统一先处理到临时目录中。
        /// 读取sqlite数据库时会文件目录下生成很多临时文件，如回顾日志、主数据库日志文件，无法重定向位置。
        /// </summary>
        /// <param name="filePath">设备中文件路径</param>
        /// <param name="isCopy">是否拷贝到临时文件中做处理</param>
        /// <param name="pwd">密码</param>
        public SqliteContext(string filePath, bool isCopy, string pwd = "")
        {
            string tempfile;

            if (isCopy)
            {
                tempfile = Path.Combine(Path.GetTempPath(), new FileInfo(filePath).Name);
                System.IO.File.Copy(filePath, tempfile, true);
            }
            else
            {
                tempfile = filePath;
            }

            if (string.IsNullOrEmpty(pwd))
            {
                DataSource = String.Format("Data Source={0}", tempfile);
            }
            else
            {
                DataSource = String.Format("Data Source={0};Password={1}", tempfile, pwd);
            }
        }

        /// <summary>
        /// SQLite操作帮助类
        /// 参数filePath为文件绝对路径或相对路径；password为数据库密码
        /// </summary>
        public SqliteContext(string filePath, string passWord)
        {
            DataSource = String.Format("Data Source={0};Password={1}", filePath, passWord);
        }

        #endregion

        #region SearchData

        #region FindDataTable
        /// <summary>
        /// 查询获取DataTable
        /// </summary>
        public DataTable FindDataTable(string sql)
        {
            Guard.ArgumentNotNullOrEmpty(sql, "SqlString");
            DataTable dt = new DataTable();
            UsingSafeConnectionS(new SQLiteString(sql), dt.Load);
            return dt;
        }
        #endregion

        #region Check Table

        /// <summary>
        /// 判断表是否存在(会同时检查表及视图)
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool Exist(string tableName)
        {
            Guard.ArgumentNotNullOrEmpty(tableName, "tableName");
            const string sql = "select * from sqlite_master s where (s.type = 'table' or s.type='view') and (lower(s.tbl_name = '{0}') or upper(s.tbl_name)='{0}')";
            var res = Find(new SQLiteString(string.Format(sql, tableName)));
            return res.IsValid();
        }

        public bool ExistTable(string tableName)
        {
            try
            {
                const string sql = "select * from sqlite_master s where (s.type = 'table' or s.type='view') and (lower(s.tbl_name = '{0}') or upper(s.tbl_name)='{0}')";
                var res = Find(new SQLiteString(string.Format(sql, tableName)));
                return res.IsValid();
            }
            catch
            {
                return false;
            }
        }

        public bool ExistField(string tableName, string fieldName)
        {
            try
            {
                if (ExistTable(tableName))
                {
                    var result = Find(new SQLiteString(String.Format("PRAGMA table_info({0}) ", tableName)));

                    return result.IsValid() && result.Any((f) => f.name == fieldName);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region FindDataTableByName
        /// <summary>
        /// 根据表明获取器所有数据
        /// </summary>
        public DataTable FindDataTableByName(string tableName)
        {
            var sql = string.Format("Select * from {0}", tableName);
            return FindDataTable(sql);
        }
        #endregion

        #region Find

        /// <summary>
        /// 执行sql，获取动态类型数据集合
        /// </summary>
        public IEnumerable<dynamic> Find(string sql)
        {
            try
            {
                var items = FindData(new SQLiteString(sql));
                if (items.Any())
                {
                    return items;
                }
                else
                {
                    return new List<dynamic>();
                }
            }
            catch
            {
                return new List<dynamic>();
            }
        }

        public IEnumerable<dynamic> Find(SQLiteString sql)
        {
            try
            {
                var items = FindData(sql);
                if (items.Any())
                {
                    return items;
                }
                else
                {
                    return new List<dynamic>();
                }
            }
            catch
            {
                return new List<dynamic>();
            }
        }

        public void Find(string sql, Action<dynamic> callback)
        {
            try
            {
                var s = new SQLiteString(sql);

                using (SQLiteConnection con = new SQLiteConnection(DataSource))
                {
                    con.Open();

                    SQLiteCommand com = new SQLiteCommand(con);
                    s.SetCommand(com);

                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            callback(reader.ToDynamic());
                        }
                        reader.Close();
                    }
                    con.Close();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 外部不能调用该方法！！！
        /// 否则可能在迭代的时候报错！
        /// 请使用安全的 public IEnumerable<dynamic> Find(string sql) 和 public IEnumerable<dynamic> Find(SQLiteString sql)
        /// 暂时不用该方法，因为数据库连接的释放问题，可能会导致文件长期呗占用
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        //private IEnumerable<dynamic> FindData(SQLiteString sql)
        //{
        //    SQLiteConnection con = new SQLiteConnection(DataSource);
        //    con.Open();

        //    SQLiteCommand com = new SQLiteCommand(con);
        //    sql.SetCommand(com);

        //    var reader = com.ExecuteReader();

        //    while (reader.Read())
        //    {
        //        yield return reader.ToDynamic();
        //    }
        //    reader.Close();
        //    con.Close();
        //}

        public List<dynamic> FindData(SQLiteString sql)
        {
            try
            {
                List<dynamic> items = new List<dynamic>();
                UsingSafeConnection(sql, (reader) =>
                {
                    while (reader.Read())
                    {
                        items.Add(reader.ToDynamic());
                    }
                });
                return items;
            }
            catch
            {
                return new List<dynamic>();
            }
        }

        #endregion

        #region FindByExistingConnection
        /// <summary>
        /// 执行sql，获取动态类型数据集合
        /// </summary>
        public List<dynamic> FindByExistingConnection(SQLiteString sql)
        {
            List<dynamic> items = new List<dynamic>();
            UsingExistingConnection(sql, (reader) =>
            {
                while (reader.Read())
                {
                    items.Add(reader.ToDynamic());
                }
            });
            return items;
        }
        #endregion

        #region FindByName
        /// <summary>
        /// 获取指定表名的所有数据
        /// </summary>
        public IEnumerable<dynamic> FindByName(string tableName)
        {
            if (!Exist(tableName))
            {
                return new List<dynamic>();
            }
            var sql = string.Format("Select * from {0}", tableName);
            return Find(new SQLiteString(sql));
        }
        #endregion

        #region FindDataTableAndSort
        /// <summary>
        /// 查询表数据，并将结果排序（解决sqlite中中文排序错误的问题）
        /// sortKeys：排序的参数，比如"username asc"、"username asc, age desc"
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sortKeys"></param>
        /// <returns></returns>
        public List<dynamic> FindDataTableAndSort(string sql, string sortKeys = "")
        {
            DataTable dt = FindDataTable(sql);
            if (string.IsNullOrEmpty(sortKeys))
                return dt.ToDynamicCollection();
            DataView dv = dt.DefaultView;
            dv.Sort = sortKeys;
            DataTable dt2 = dv.ToTable();
            return dt2.ToDynamicCollection();
        }
        #endregion

        #endregion

        #region Execute

        #region ExecuteNonQuery
        /// <summary>
        /// 执行Sql，返回受影响的行数
        /// </summary>
        public int ExecuteNonQuery(SQLiteString sql)
        {
            int result = 0;
            UsingSafeConnection(com =>
                {
                    com.CommandText = sql.SqlText;
                    sql.SetCommand(com);
                    result = com.ExecuteNonQuery();
                });

            return result;
        }
        /// <summary>
        /// 执行Sql，返回受影响的行数
        /// </summary>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(new SQLiteString(sql));
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行sql，返回首行首列的值
        /// </summary>
        public string ExecuteScalar(SQLiteString sql)
        {
            string result = string.Empty;
            UsingSafeConnection(com =>
            {
                com.CommandText = sql.SqlText;
                sql.SetCommand(com);
                result = com.ExecuteScalar().ToSafeString();
            });
            return result;
        }
        /// <summary>
        /// 执行sql，返回首行首列的值
        /// </summary>
        public string ExecuteScalar(string sql)
        {
            return ExecuteScalar(new SQLiteString(sql));
        }
        #endregion

        #endregion

        #region UsingSafeConnection(SQLiteCommand)
        /// <summary>
        /// 创建一个安全的数据库连接，回调函数可以执行其他sql指令
        /// </summary>
        /// <param name="callBack"></param>
        public void UsingSafeConnection(Action<SQLiteCommand> callBack)
        {
            using (SQLiteConnection con = new SQLiteConnection(DataSource))
            {
                con.Open();
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    try
                    {
                        callBack(com);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("SQLite database [{0}] occurse errors", DataSource), ex);
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }
        #endregion

        #region UsingSafeConnection(SQLiteDataReader)
        /// <summary>
        /// 创建一个安全的数据库连接，执行sql指令，回调函数使用参数DataReader处理数据
        /// </summary>
        /// <param name="callBack"></param>
        public void UsingSafeConnection(SQLiteString sql, Action<DBDataReader> callBack)
        {
            using (SQLiteConnection con = new SQLiteConnection(DataSource))
            {
                con.Open();
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    try
                    {
                        com.CommandText = sql.SqlText;
                        sql.SetCommand(com);
                        var reader = com.ExecuteReader();
                        callBack(new DBDataReader(reader));
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Log4NetService.LoggerManagerSingle.Instance.Error(ex, string.Format("SQLite database [{0}] occurse errors", DataSource));
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }

        private void UsingSafeConnectionS(SQLiteString sql, Action<SQLiteDataReader> callBack)
        {
            using (SQLiteConnection con = new SQLiteConnection(DataSource))
            {
                con.Open();
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    try
                    {
                        com.CommandText = sql.SqlText;
                        sql.SetCommand(com);
                        var reader = com.ExecuteReader();
                        callBack(reader);
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Log4NetService.LoggerManagerSingle.Instance.Error(ex, string.Format("SQLite database [{0}] occurse errors", DataSource));
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }

        public void UsingSafeConnection(string sqlStr, Action<DBDataReader> callBack)
        {
            var sql = new SQLiteString(sqlStr);

            using (SQLiteConnection con = new SQLiteConnection(DataSource))
            {
                con.Open();
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    try
                    {
                        com.CommandText = sql.SqlText;
                        sql.SetCommand(com);
                        var reader = com.ExecuteReader();
                        callBack(new DBDataReader(reader));
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Log4NetService.LoggerManagerSingle.Instance.Error(ex, string.Format("SQLite database [{0}] occurse errors", DataSource));
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
        }

        #endregion

        #region UsingSafeTransaction(SQLiteCommand)
        /// <summary>
        /// 创建一个安全的数据库事物处理，回调函数可以执行sql指令
        /// </summary>
        /// <param name="callBack"></param>
        public void UsingSafeTransaction(Action<SQLiteCommand> callBack)
        {
            using (SQLiteConnection con = new SQLiteConnection(DataSource))
            {
                con.Open();
                using (SQLiteTransaction tran = con.BeginTransaction())
                {
                    using (SQLiteCommand com = new SQLiteCommand(con))
                    {
                        try
                        {
                            callBack(com);
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new Exception(string.Format("SQLite database [{0}] occurse errors", DataSource), ex);
                        }
                        finally
                        {
                            tran.Commit();
                            con.Close();
                        }
                    }
                }
            }
        }
        #endregion

        #region UsingExistingConnection
        private SQLiteConnection _SQLiteConnection;
        /// <summary>
        /// 已经存在的SQLite连接，使用此连接，必须手动地调用Dispose方法释放资源
        /// </summary>
        public SQLiteConnection ExistingConnection
        {
            get
            {
                if (_SQLiteConnection == null || _SQLiteConnection.State != ConnectionState.Open)
                {
                    _SQLiteConnection = new SQLiteConnection(DataSource);
                    _SQLiteConnection.Open();
                }
                return _SQLiteConnection;
            }
        }

        public void UsingExistingConnection(SQLiteString sql, Action<SQLiteDataReader> callBack)
        {
            using (SQLiteCommand com = new SQLiteCommand(ExistingConnection))
            {
                try
                {
                    com.CommandText = sql.SqlText;
                    sql.SetCommand(com);
                    var reader = com.ExecuteReader();
                    callBack(reader);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("SQLite database [{0}] occurse errors", DataSource), ex);
                }
            }
        }

        public void UsingExistingConnection(string sqlStr, Action<DBDataReader> callBack)
        {
            var sql = new SQLiteString(sqlStr);

            using (SQLiteCommand com = new SQLiteCommand(ExistingConnection))
            {
                try
                {
                    com.CommandText = sql.SqlText;
                    sql.SetCommand(com);
                    var reader = com.ExecuteReader();
                    callBack(new DBDataReader(reader));
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("SQLite database [{0}] occurse errors", DataSource), ex);
                }
            }
        }

        #endregion

        #region Dispose
        /// <summary>
        /// 释放非托管资源
        /// </summary>
        public void Dispose()
        {
            if (_SQLiteConnection == null)
            {
                return;
            }
            _SQLiteConnection.Close();
            _SQLiteConnection.Dispose();
            _SQLiteConnection = null;
        }
        #endregion

        #region 合并表

        /// <summary>
        /// 合并表数据并创建新表,添加索引
        /// </summary>
        /// <param name="tables">要合并的表，用,分隔 例如  aaa,bbb,ccc</param>
        /// <param name="newTableName">新表表名</param>
        /// <param name="indexName">索引名称</param>
        /// <param name="indexColum">索引列</param>
        public void MergeTables(string tables, string newTableName, string indexName, string indexColum)
        {
            var lsTables = tables.Split(',');

            //1.创建新表
            this.ExecuteNonQuery(new SQLiteString(string.Format("create table {0} as select * from {1} where 1=0", newTableName, lsTables[0])));
            this.ExecuteNonQuery(new SQLiteString(string.Format("CREATE INDEX {0} ON {1} ({2})", indexName, newTableName, indexColum)));

            //2.插入数据
            foreach (var tb in lsTables)
            {
                this.ExecuteNonQuery(new SQLiteString(string.Format("insert into {0} select * from {1}", newTableName, tb)));
            }
        }

        /// <summary>
        /// 合并表数据并创建新表
        /// </summary>
        /// <param name="tables">要合并的表，用,分隔 例如  aaa,bbb,ccc</param>
        /// <param name="newTableName">新表表名</param>
        public void MergeTables(string tables, string newTableName)
        {
            var lsTables = tables.Split(',');

            //1.创建新表
            ExecuteNonQuery(new SQLiteString(string.Format("create table {0} as select * from {1} where 1=0", newTableName, lsTables[0])));

            //2.插入数据
            foreach (var tb in lsTables)
            {
                ExecuteNonQuery(new SQLiteString(string.Format("insert into {0} select * from {1}", newTableName, tb)));
            }
        }

        #endregion

    }

    public class DBDataReader
    {
        private SQLiteDataReader Reader { get; set; }

        public bool HasRows { get => Reader.HasRows; }

        public DBDataReader(SQLiteDataReader reader)
        {
            Reader = reader;
        }

        public bool Read()
        {
            if (null == Reader)
            {
                return false;
            }

            return Reader.Read();
        }

        public dynamic ToDynamic()
        {
            if (null == Reader)
            {
                return null;
            }

            return Reader.ToDynamic();
        }
    }
}
