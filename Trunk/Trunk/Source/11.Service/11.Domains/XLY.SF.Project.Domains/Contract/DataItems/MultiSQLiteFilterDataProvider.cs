using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.DataFilter.Asist;
using XLY.SF.Project.DataFilter.Providers;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.MultiSQLiteFilterDataProvider
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/21 13:31:06
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 支持多数据库文件联合查询的数据库Provider
    /// </summary>
    public class MultiSQLiteFilterDataProvider : SQLiteFilterDataProvider
    {
        public MultiSQLiteFilterDataProvider(string file, string tableName, string password = "") : base(file, tableName, password)
        {
        }

        /// <summary>
        /// 附加数据库的路径
        /// </summary>
        public string AttachedDatabase { get; set; }
        /// <summary>
        /// 附加数据库的别名
        /// </summary>
        public string AttachedDatabaseAliasName { get; set; }

        /// <summary>
        /// 获取数据（目前暂时使用内容为SQL语句的常量表达式）。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="count">集合的大小。</param>
        /// <returns>数据。</returns>
        public override IEnumerable<T> Query<T>(Expression expression)
        {
            bool IsAttachDB = (expression as ConstantExpression).Value.ToString().StartsWith("$") || (expression as ConstantExpression).Value.ToString().StartsWith("#");
            if (!IsAttachDB || string.IsNullOrWhiteSpace(AttachedDatabase))
            {
                return base.Query<T>(expression);
            }
            SQLiteConnection connection = new SQLiteConnection(ConnectionString);
            connection.Flags = SQLiteConnectionFlags.UseConnectionPool;
            connection.Open();

            Attach(connection);

            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = GetSelectionSql(expression, TableName);
            DbDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return new DbEnumerableDataReader<T>(reader);
        }

        /// <summary>
        /// 查询数量。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <returns>集合的大小。</returns>
        public override Int32 GetCount(Expression expression)
        {
            bool IsAttachDB = (expression as ConstantExpression).Value.ToString().StartsWith("$")|| (expression as ConstantExpression).Value.ToString().StartsWith("#");
            if (!IsAttachDB || string.IsNullOrWhiteSpace(AttachedDatabase))
            {
                return base.GetCount(expression);
            }
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Flags = SQLiteConnectionFlags.UseConnectionPool;
                connection.Open();

                Attach(connection);

                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = GetCountSql(expression, TableName);
                return (Int32)(Int64)command.ExecuteScalar();
            }
        }

        private void Attach(SQLiteConnection connection)
        {
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = $"ATTACH DATABASE '{AttachedDatabase}' as {AttachedDatabaseAliasName}";
            cmd.ExecuteNonQuery();
        }

        private String GetSelectionSql(Expression expression, String tableName)
        {
            if (expression.NodeType != ExpressionType.Constant) return null;
            if (expression.Type != typeof(String)) return null;
            ConstantExpression constantExpression = (ConstantExpression)expression;
            String str = constantExpression.Value as String;
            if(str.StartsWith("#"))  //联合查询
            {
                str = str.TrimStart('#');
                string limitStr = "";
                int limit = str.LastIndexOf("LIMIT");
                if(limit >= 0)
                {
                    limitStr = str.Substring(limit);
                    str = str.Substring(0, limit);
                }
                return $"SELECT a.*,b.BookMarkId FROM {tableName} a, {AttachedDatabaseAliasName}.{tableName} b WHERE a.[MD5] = b.[MD5] AND {str} AND b.BookMarkId < 0 " +
                    $" UNION "+
                    $"SELECT a.*,-1 from {tableName} a where a.[MD5] not in (SELECT md5 from {AttachedDatabaseAliasName}.{tableName}) AND {str} " +
                    limitStr;
            }
            else if(str.StartsWith("$"))  
            {
                return $"SELECT a.*,b.BookMarkId FROM {tableName} a, {AttachedDatabaseAliasName}.{tableName} b WHERE a.[MD5] = b.[MD5] AND {str.TrimStart('$')}";
            }
            else
            {
                return $"SELECT * FROM {tableName} a";
            }
        }

        private String GetCountSql(Expression expression, String tableName)
        {
            if (expression.NodeType != ExpressionType.Constant) return null;
            if (expression.Type != typeof(String)) return null;
            ConstantExpression constantExpression = (ConstantExpression)expression;
            String str = (String)constantExpression.Value;
            //return $"SELECT COUNT(*) FROM {tableName} a, {AttachedDatabaseAliasName}.{tableName} b WHERE a.[MD5] = b.[MD5] AND  {str.TrimStart('$')}";
            if (str.StartsWith("#"))
            {
                str = str.TrimStart('#');
                return $"SELECT (SELECT COUNT(*) FROM {tableName} a, {AttachedDatabaseAliasName}.{tableName} b WHERE a.[MD5] = b.[MD5] AND {str} AND b.BookMarkId < 0 )  " +
                    $" + " +
                    $"(SELECT COUNT(*) from {tableName} a where a.[MD5] not in (SELECT md5 from {AttachedDatabaseAliasName}.{tableName}) AND {str}) ";
            }
            else if (str.StartsWith("$"))
            {
                return $"SELECT COUNT(*) FROM {tableName} a, {AttachedDatabaseAliasName}.{tableName} b WHERE a.[MD5] = b.[MD5] AND {str.TrimStart('$')}";
            }
            else
            {
                return $"SELECT COUNT(*) FROM {tableName} a";
            }
        }
    }
}
