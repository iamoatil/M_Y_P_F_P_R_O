// ***********************************************************************
// Assembly:XLY.SF.Project.Domains.Contract.DataItems
// Author:Songbing
// Created:2017-06-08 13:43:43
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    //[SQLiteFunction(Name = "RegExp", FuncType = FunctionType.Scalar, Arguments = 2)]
    //public class MyRegExp : SQLiteFunction
    //{
    //    public override object Invoke(object[] args)
    //    {
    //        return Regex.IsMatch(Convert.ToString(args[0]), Convert.ToString(args[1]));
    //    }
    //}

    /// <summary>
    /// Sqlite数据库
    /// </summary>
    [Serializable]
    public class SqliteDbFile
    {
        public static CJKTokenizer s_Tokenizer;

        /// <summary>
        /// 是否使用虚表，虚表可以支持全文检索
        /// </summary>
        public static bool IsUseVirtualTable { get; set; } = false;

        /// <summary>
        /// 对象缓存，一个数据库文件对应一个SqliteDbFile
        /// </summary>
        private static Dictionary<string, SqliteDbFile> s_DbFileCahe;

        static SqliteDbFile()
        {
            s_Tokenizer = new CJKTokenizer();
            s_DbFileCahe = new Dictionary<string, SqliteDbFile>();
        }

        /// <summary>
        /// 创建SqliteDbFile
        /// </summary>
        /// <param name="dbfile">数据库文件路径</param>
        /// <returns></returns>
        public static SqliteDbFile GetSqliteDbFile(string dbfile)
        {
            if (!s_DbFileCahe.Keys.Contains(dbfile))
            {
                lock (s_DbFileCahe)
                {
                    if (!s_DbFileCahe.Keys.Contains(dbfile))
                    {
                        return new SqliteDbFile(dbfile);
                    }
                }
            }

            return s_DbFileCahe[dbfile];
        }

        /// <summary>
        /// Key列名
        /// 同一个表可能保存了多个IDataItems的数据，该列保存IDataItems的Key.ToString()
        /// </summary>
        public const string KeyColumnName = "XLYKey";


        public const string MD5ColumnName = "MD5";
        public const string BookmarkColumnName = "BookMarkId";

        /// <summary>
        /// Json数据列名
        /// 用户保存C#实体类对象的Json序列化字符串
        /// </summary>
        public const string JsonColumnName = "XLYJson";

        /// <summary>
        /// 数据库文件路径
        /// </summary>
        public string DbFilePath { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string DbConnectionStr { get { return string.Format("Data Source='{0}'", DbFilePath); } }
        /// <summary>
        /// 书签数据库连接字符串
        /// </summary>
        public string DbBookmarkConnectionStr { get { return string.Format("Data Source='{0}'", DbFilePath.Insert(DbFilePath.LastIndexOf('.'), "_bmk")); } }

        [NonSerialized]
        private SQLiteTransaction _DbTransaction;

        /// <summary>
        /// 数据库事务
        /// </summary>
        public SQLiteTransaction DbTransaction
        {
            get
            {
                if (null == _DbTransaction)
                {
                    lock (LockDbTran)
                    {
                        if (null == _DbTransaction)
                        {
                            _DbTransaction = BeginTransaction();
                        }
                    }
                }
                return _DbTransaction;
            }
            set
            {
                _DbTransaction = value;
            }
        }

        [NonSerialized]
        private Dictionary<string, string> _TableNameCache;

        /// <summary>
        /// 数据库表名缓存
        /// </summary>
        public Dictionary<string, string> TableNameCache { get => _TableNameCache; set => _TableNameCache = value; }

        [NonSerialized]
        private object LockDbTran = new object();

        [NonSerialized]
        private object LockTableNameCache = new object();

        private SqliteDbFile(string dbfile)
        {
            DbFilePath = dbfile;
            TableNameCache = new Dictionary<string, string>();

            if (!File.Exists(DbFilePath))
            {
                var basePath = new FileInfo(DbFilePath).DirectoryName;
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                SQLiteConnection.CreateFile(DbFilePath);
            }

            s_DbFileCahe.Add(dbfile, this);
        }

        /// <summary>
        /// 创建数据库表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string CreateTable<T>(string defTableName = null) where T : AbstractDataItem
        {
            var dataType = typeof(T);
            var dataTypeName = dataType.FullName;

            string tableName = defTableName;
            if (!TableNameCache.Keys.Contains(dataTypeName))
            {
                lock (LockTableNameCache)
                {
                    if (!TableNameCache.Keys.Contains(dataTypeName))
                    {
                        if (string.IsNullOrWhiteSpace(tableName))
                            tableName = GetTableName(dataTypeName);
                        else
                            TableNameCache[dataTypeName] = tableName;

                        var disss = DisplayAttributeHelper.FindDisplayAttributes(typeof(T));

                        StringBuilder sb = new StringBuilder();
                        if (IsUseVirtualTable)
                        {
                            sb.AppendFormat("CREATE VIRTUAL TABLE {0} USING fts3(", tableName);
                        }
                        else
                        {
                            sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0}(", tableName);
                        }

                        sb.AppendFormat("{0} CHAR(50) NOT NULL", KeyColumnName);
                        foreach (var dis in disss)
                        {
                            if (dis.Visibility != EnumDisplayVisibility.ShowInUI)        //只在界面展示的列不需要存储
                            {
                                sb.AppendFormat(",{0} {1}", dis.Key, dis.DataType);
                            }
                        }
                        sb.AppendFormat(",{0} TEXT", JsonColumnName);

                        if (IsUseVirtualTable)
                        {
                            sb.AppendFormat(",tokenize={0}", s_Tokenizer.Name);
                        }

                        sb.Append(");");

                        ExecuteNonQueryWithTransaction(sb.ToString());

                        ExecuteNonQueryBookmark($"CREATE TABLE IF NOT EXISTS {tableName}({MD5ColumnName} CHAR(64) NOT NULL, {BookmarkColumnName} INTEGER, PRIMARY KEY (MD5));");

                        //创建索引，会降低插入数据的效率
                        //ExecuteNonQueryWithTran(string.Format("CREATE INDEX {0}_key_index on {0}({1}); ", tableName, s_KeyColumnName));
                    }
                }
            }

            return TableNameCache[dataTypeName];
        }

        public string CreateTable(IEnumerable<string> colunms)
        {
            string tableName = String.Format("Table_{0}", Guid.NewGuid().ToString("N").ToUpper());

            StringBuilder sb = new StringBuilder();
            if (IsUseVirtualTable)
            {
                sb.AppendFormat("CREATE VIRTUAL TABLE {0} USING fts3(", tableName);
            }
            else
            {
                sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0}(", tableName);
            }

            sb.AppendFormat("{0} CHAR(50) NOT NULL", KeyColumnName);
            foreach (var col in colunms)
            {
                sb.AppendFormat(",{0} TEXT", col);
            }

            if (IsUseVirtualTable)
            {
                sb.AppendFormat(",tokenize={0}", s_Tokenizer.Name);
            }

            sb.Append(");");

            ExecuteNonQueryWithTransaction(sb.ToString());

            ExecuteNonQueryBookmark($"CREATE TABLE IF NOT EXISTS {tableName}({MD5ColumnName} CHAR(64) NOT NULL, {BookmarkColumnName} INTEGER, PRIMARY KEY (MD5));");

            //创建索引，会降低插入数据的效率
            //ExecuteNonQueryWithTran(string.Format("CREATE INDEX {0}_key_index on {0}({1}); ", tableName, s_KeyColumnName));

            return tableName;
        }

        public void Add<T>(T obj, string key) where T : AbstractDataItem
        {
            if (null == obj || null == key)
            {
                return;
            }

            var tableName = TableNameCache[typeof(T).FullName];

            List<SQLiteParameter> parameters = new List<SQLiteParameter>();

            var diss = DisplayAttributeHelper.FindDisplayAttributes(typeof(T));
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO {0} VALUES(@{1}", tableName, KeyColumnName);
            parameters.Add(new SQLiteParameter(string.Format("@{0}", KeyColumnName), System.Data.DbType.String) { Value = key });

            foreach (var dis in diss)
            {
                if (dis.Visibility != EnumDisplayVisibility.ShowInUI)
                {
                    sb.AppendFormat(",@{0}", dis.Key);
                    parameters.Add(new SQLiteParameter(string.Format("@{0}", dis.Key), System.Data.DbType.String) { Value = dis.GetValue(obj) });
                }
            }
            sb.AppendFormat(",@{0}", JsonColumnName);
            parameters.Add(new SQLiteParameter(string.Format("@{0}", JsonColumnName), System.Data.DbType.String) { Value = Serializer.JsonSerilize(obj) });

            sb.Append(");");

            ExecuteNonQueryWithTransaction(sb.ToString(), parameters.ToArray());

            //添加属性监听事件，在修改其属性时自动更新数据库
            obj.OnPropertyValueChangedEvent -= Update;     
            obj.OnPropertyValueChangedEvent += Update;     
        }

        public void Add(JObject obj, string key, string tableName, IEnumerable<string> colunms)
        {
            if (null == obj || null == key)
            {
                return;
            }

            List<SQLiteParameter> parameters = new List<SQLiteParameter>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO {0} VALUES(@{1}", tableName, KeyColumnName);
            parameters.Add(new SQLiteParameter(string.Format("@{0}", KeyColumnName), System.Data.DbType.String) { Value = key });

            foreach (var col in colunms)
            {
                sb.AppendFormat(",@{0}", col);
                parameters.Add(new SQLiteParameter(string.Format("@{0}", col), System.Data.DbType.String) { Value = obj.GetValue(col).ToString() });
            }

            sb.Append(");");

            ExecuteNonQueryWithTransaction(sb.ToString(), parameters.ToArray());
        }

        private void Update(object obj, string propertyName, object propertyValue)
        {
            if (obj is AbstractDataItem item)
            {
                if(propertyName != BookmarkColumnName)
                {
                    Update(item);       
                }
                else
                {
                    UpdateBookmark(item);
                }
            }
        }

        /// <summary>
        /// 更新数据，使用MD5值作为主键来更新数据；
        /// 此时MD5不能重新计算,而是在更新完成后自动计算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Update<T>(T obj) where T : AbstractDataItem
        {
            string oldMd5 = obj.MD5;        //使用旧的MD5值作为主键来更新数据
            obj.Recalculate();      //计算新值，比如新的MD5
            if(oldMd5 == obj.MD5)
            {
                return;
            }
            Type type = obj.GetType();
            
            var tableName = TableNameCache[type.FullName];
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();

            var diss = DisplayAttributeHelper.FindDisplayAttributes(type);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update {0} set ", tableName);
            foreach (var dis in diss)
            {
                if (dis.Visibility != EnumDisplayVisibility.ShowInUI)
                {
                    sb.AppendFormat("{0} = @{0}, ", dis.Key);
                    parameters.Add(new SQLiteParameter($"@{dis.Key}", System.Data.DbType.String) { Value = dis.GetValue(obj) });
                }
            }
            sb.AppendFormat("{0} = @{0}", JsonColumnName);
            parameters.Add(new SQLiteParameter($"@{JsonColumnName}", System.Data.DbType.String) { Value = Serializer.JsonSerilize(obj) });

            //sb.AppendFormat(" where {0} = @{0}", MD5ColumnName);
            //parameters.Add(new SQLiteParameter($"@{MD5ColumnName}", System.Data.DbType.String) { Value = oldMd5 });
            sb.AppendFormat(" where {0} = '{1}' ", MD5ColumnName, oldMd5);
            
            ExecuteNonQueryWithTransaction(sb.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// 更新数据的书签，并保存到数据库中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void UpdateBookmark<T>(T obj) where T : AbstractDataItem
        {
            ExecuteNonQueryBookmark($"REPLACE INTO {TableNameCache[obj.GetType().FullName]} ({BookmarkColumnName},{MD5ColumnName}) values ({obj.BookMarkId}, '{obj.MD5}');");
        }

        /// <summary>
        /// 获取数据列表的书签状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        public void GetDataItemsBookmark<T>(IEnumerable<T> view) where T : AbstractDataItem
        {
            if (view == null || view.Count() == 0)
            {
                return;
            }
            StringBuilder sb = new StringBuilder(2048);     //数据每个分页约50行数据
            sb.Append($"select * from {TableNameCache[view.First().GetType().FullName]} where {MD5ColumnName} in (");
            foreach (var item in view)
            {
                sb.Append($"'{item.MD5}', ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(");");
            var dt = ExecuteReaderBookmark(sb.ToString());
            Dictionary<string, int> dicBmk = new Dictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                dicBmk[row[MD5ColumnName].ToString()] = row[BookmarkColumnName].ToSafeString().ToSafeInt(-1);
            }
            foreach (var item in view)
            {
                item.BookMarkId = dicBmk.ContainsKey(item.MD5) ? dicBmk[item.MD5] : -1;

                //添加属性监听事件，在修改其属性时自动更新数据库
                item.OnPropertyValueChangedEvent -= Update;
                item.OnPropertyValueChangedEvent += Update;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SQLiteTransaction BeginTransaction()
        {
            var conn = new SQLiteConnection(DbConnectionStr);
            conn.Open();

            if (IsUseVirtualTable)
            {
                s_Tokenizer.RegisterMe(conn);
            }

            return conn.BeginTransaction();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Commit()
        {
            if (null != _DbTransaction)
            {
                DbTransaction.Commit();
                DbTransaction = null;
            }
        }

        private string GetTableName(string dataTypeName)
        {
            var tableName = String.Format("Table_{0}", Guid.NewGuid().ToString("N").ToUpper());
            TableNameCache.Add(dataTypeName, tableName);

            return tableName;
        }

        public void SetTableName(string dataTypeName, string tableName)
        {
            TableNameCache[dataTypeName] = tableName;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ExecuteNonQueryWithTransaction(string sql, params SQLiteParameter[] parameters)
        {
            using (var com = new SQLiteCommand(DbTransaction.Connection))
            {
                com.CommandText = sql;
                if (parameters.IsValid())
                {
                    com.Parameters.AddRange(parameters);
                }
                com.ExecuteNonQuery();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ExecuteNonQueryBookmark(string sql, params SQLiteParameter[] parameters)
        {
            using(SQLiteConnection con  = new SQLiteConnection(DbBookmarkConnectionStr))
            {
                con.Open();
                using (var com = new SQLiteCommand(con))
                {
                    com.CommandText = sql;
                    if (parameters.IsValid())
                    {
                        com.Parameters.AddRange(parameters);
                    }
                    com.ExecuteNonQuery();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private DataTable ExecuteReaderBookmark(string sql, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection con = new SQLiteConnection(DbBookmarkConnectionStr))
            {
                con.Open();
                using (var com = new SQLiteCommand(con))
                {
                    com.CommandText = sql;
                    if (parameters.IsValid())
                    {
                        com.Parameters.AddRange(parameters);
                    }
                    var reader = com.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();
                    return dt;
                }
            }
        }
    }
}
