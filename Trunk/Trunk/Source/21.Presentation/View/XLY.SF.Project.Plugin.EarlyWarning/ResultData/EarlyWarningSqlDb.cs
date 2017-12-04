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
    class EarlyWarningSqlDb:IDisposable
    {
        /// <summary>
        /// 是否已经初始化。对象要初始化后才能使用
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 数据库文件的路径
        /// </summary>
        private string _path;

        /// <summary>
        /// 数据库的连接
        /// </summary>
        SQLiteConnection _dbConnection;

        public EarlyWarningSqlDb()
        {
            _path = Path.GetFullPath("EarlyWarning");         
        }

        /// <summary>
        /// 创建数据库文件
        /// </summary>
        /// <param name="path"></param>
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

        /// <summary>
        /// 创建指定了列的Table
        /// </summary>
        /// <param name="colunms"></param>
        /// <param name="tableName"></param>
        private void CreateTable(IEnumerable<string> colunms, string tableName)
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
        /// 对象要初始化后才能使用
        /// </summary>
        /// <returns></returns>
        internal bool Initialize()
        {
            _isInitialized = false;

            Create(_path);
            //todo 必须要释放SQLiteConnection
            _dbConnection = new SQLiteConnection(string.Format("Data Source={0}", _path));
            _dbConnection.Open();

            _isInitialized = true;
            return _isInitialized;
        }        

        /// <summary>
        /// 把含有敏感字符的数据保存到路径为 _path的数据库中
        /// </summary>
        internal void WriteResult(IEnumerable<dynamic> result, string tableName,Type type)
        {
            if(!_isInitialized)
            {
                return;
            }
            
            SQLFilterDataProvider.DbEnumerableDataReader<object> lst = (SQLFilterDataProvider.DbEnumerableDataReader<object>)result;
            List<string> cols = lst.Columns.Select(it => it.Key).Where(it => it != SqliteDbFile.KeyColumnName).ToList();
            CreateTable(cols, tableName);
            
            PropertyInfo[] proInfos = type.GetProperties();
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
                        if (cols.Contains(display))
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

        #region IDisposable
        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~EarlyWarningSqlDb()
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
                _dbConnection.Dispose();
                _isInitialized = false;
                File.Delete(_path);
                //TODO:释放那些实现IDisposable接口的托管对象
            }
            //TODO:释放非托管资源，设置对象为null
            _disposed = true;
        }
        #endregion
    }
}
