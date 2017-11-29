/* ==============================================================================
* Description：SqlFile  
* Author     ：litao
* Create Date：2017/11/28 14:55:48
* ==============================================================================*/

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using XLY.SF.Project.DataFilter.Providers;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class SqlFile
    {

        private string _path  ;
        private string _bookMarkPath;

        SQLiteConnection _dbConnection;
        SQLiteConnection _dbBookMarkConnection;
        public SqlFile()
        {
            _path = Path.GetFullPath("EarlyWarning");
            _bookMarkPath = Path.GetFullPath("EarlyWarningBookMark");

           Create(_path);
           Create(_bookMarkPath);
            _dbConnection = new SQLiteConnection(string.Format("Data Source={0}", _path));
            _dbConnection.Open();
            _dbBookMarkConnection = new SQLiteConnection(string.Format("Data Source={0}", _bookMarkPath));
            _dbBookMarkConnection.Open();
        }        

        private void Create(string path)
        {
            if (!File.Exists(path))
            {
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                SQLiteConnection.CreateFile(path);               
            }           
        }

        public void CreateTable(IEnumerable<string> colunms,string tableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0}(", tableName);
            sb.AppendFormat("'{0}' CHAR(50) NOT NULL", SqliteDbFile.KeyColumnName);

            foreach (var col in colunms)
            {
                sb.AppendFormat(",'{0}' TEXT", col);
            }            

            sb.Append(");");

            SQLiteCommand command = new SQLiteCommand(sb.ToString(), _dbConnection);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 把被标记的数据写到路径为_bookMarkPath的数据库中
        /// </summary>
        public void MarkItem()
        {

        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            File.Delete(_path);
            File.Delete(_bookMarkPath);
        }

        /// <summary>
        /// 把含有敏感字符的数据保存到路径为 _path的数据库中
        /// </summary>
        internal void WriteResult(IEnumerable<dynamic> result, AbstractDataSource dataSource)
        {
            string tableName = dataSource.Items.DbTableName;
            SQLFilterDataProvider.DbEnumerableDataReader<object> lst = (SQLFilterDataProvider.DbEnumerableDataReader<object>)result;
            List<string> cols = lst.Columns.Select(it => it.Key).Where(it =>it != SqliteDbFile.KeyColumnName).ToList();
            CreateTable(cols, tableName);

            Type type = (Type)dataSource.Type;
            PropertyInfo[] proInfos=type.GetProperties();
            Dictionary<string, PropertyInfo> proDic = new Dictionary<string, PropertyInfo>();
            foreach (var item in proInfos)
            {
                if (cols.Contains(item.Name))
                {
                    proDic.Add(item.Name, item);
                }
                else if (item.CustomAttributes.Count() > 0)
                {
                    var customAtrrs = item.CustomAttributes.Where(it => it.AttributeType.Name == "DisplayAttribute");
                    if (customAtrrs.Count() > 0
                        && customAtrrs.ElementAt(0).NamedArguments.Count() > 0)
                    {
                        string display = customAtrrs.ElementAt(0).NamedArguments[0].TypedValue.ToString();
                        if(cols.Contains(display))
                        {
                            proDic.Add(display, item);
                        }                        
                        continue;
                    }
                }
            }

            foreach (AbstractDataItem item in result)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("insert into {0} values('{1}'", tableName, item.MD5);

                foreach (var col in cols)
                {
                    if (!proDic.Keys.Contains(col))
                    {
                        sb.AppendFormat(",'{0}'", "");
                        continue;
                    }
                    var proInfo = proDic[col];
                    sb.AppendFormat(",'{0}'", proInfo.GetValue(item));
                }

                sb.AppendFormat(");");

                SQLiteCommand command = new SQLiteCommand(sb.ToString(), _dbConnection);
                command.ExecuteNonQuery();
            }
        }
    }
}
