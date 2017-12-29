using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 可以勾选的数据装饰属性
    /// </summary>
    public class CheckDecorationProperty:DecorationProperty
    {
        public CheckDecorationProperty(object defaultValue = null, object metaData = null, EventHandler<DecorationArgs> onChanged = null) : base(defaultValue, metaData, onChanged)
        {
            _cacheData = new CacheData<object, object>(2000);
        }
        public const string DPTableName = "t_check";
        public const string DPDBName = "data_chk.db";
        public const string DPDirName = "Analysis";     //数据库存储文件夹
        public const string KeyColName = "key";
        public const string StateColName = "state";
        private List<string> _tables = new List<string>();
        private CacheData<object, object> _cacheData;       //缓存数据结果集(加快速度)，在一定时间后自动删除

        public override object GetValue(object obj)
        {
            var param = GetParam(obj);
            if (_cacheData.Contains(param.key))
            {
                return _cacheData[param.key];
            }
            if(param.chkDB == null)
            {
                return DefaultValue;
            }
            if (!_tables.Contains(param.chkDB))
            {
                SqliteCache.ExecuteNonQuery(param.chkDB, $"CREATE TABLE IF NOT EXISTS {DPTableName}({KeyColName} CHAR(64) NOT NULL, {StateColName} INT, PRIMARY KEY ({KeyColName}));");
                _tables.Add(param.chkDB);
            }
            var v = SqliteCache.ExecuteScalar(param.chkDB, $"select {StateColName} from {DPTableName} where {KeyColName} = '{param.key}'");
            object res;
            if (v is int a)
            {
                if (a == -1)
                    res = null;
                else
                    res = a == 1;
            }
            else
                res = DefaultValue;
            _cacheData[param.key] = res;
            return res;
        }

        public override void SetValue(object obj, object value, bool isTriggerByChild)
        {
            var oldValue = GetValue(obj);
            if (oldValue == null && value == null)
                return;
            else if (oldValue != null && oldValue.Equals(value))
                return;
            var param = GetParam(obj);
            if(param.chkDB == null)
            {
                return;
            }
            SqliteCache.BeginTransaction(param.chkDB);
            if((value == null && DefaultValue == null) || (value != null && value.Equals(DefaultValue)))    //如果等于默认值，则从数据库中删除
            {
                SqliteCache.ExecuteNonQuery(param.chkDB, $"DELETE FROM {DPTableName} WHERE {KeyColName} = '{param.key}'");
            }
            else  //否则将新值插入数据库
            {
                int a = value == null ? -1 : (bool)value ? 1 : 0;
                SqliteCache.ExecuteNonQuery(param.chkDB, $"REPLACE INTO {DPTableName} ({StateColName}, {KeyColName}) values ({a}, '{param.key}');");
            }
            
            _cacheData[param.key] = value;
            TriggerEvent(obj, new DecorationArgs() { Value = value, TriggerByChild = isTriggerByChild });
        }

        public override void SetListValue(IEnumerable<object> obj, object value)
        {
            if (obj == null || !obj.Any())
                return;
            var param = GetParam(obj.ElementAt(0));
            if (param.chkDB == null)
            {
                return;
            }
            SqliteCache.BeginTransaction(param.chkDB);
            if ((value == null && DefaultValue == null) || (value != null && value.Equals(DefaultValue)))    //如果等于默认值，则从数据库中删除
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"DELETE FROM {DPTableName} WHERE 1 = 2 ");
                foreach (var item in obj)
                {
                    string key = GetKey(item);
                    sb.Append($"OR {KeyColName} = '{key}'");
                    _cacheData[key] = value;
                }
                SqliteCache.ExecuteNonQuery(param.chkDB, sb.ToString());
            }
            else  //否则将新值插入数据库
            {
                int a = value == null ? -1 : (bool)value ? 1 : 0;
                StringBuilder sb = new StringBuilder();
                sb.Append($"REPLACE INTO {DPTableName} ({StateColName}, {KeyColName}) values ");
                foreach (var item in obj)
                {
                    string key = GetKey(item);
                    sb.Append($"({a}, '{key}'),");
                    _cacheData[key] = value;
                }
                SqliteCache.ExecuteNonQuery(param.chkDB, sb.Remove(sb.Length - 1, 1).ToString());
            }
            //int a = value == null ? -1 : (bool)value ? 1 : 0;
            //StringBuilder sb = new StringBuilder();
            //sb.Append($"REPLACE INTO {DPTableName} ({StateColName}, {KeyColName}) values ");
            //foreach (var item in obj)
            //{
            //    string key = GetKey(item);
            //    sb.Append($"({a}, '{key}'),");
            //    _cacheData[key] = value;
            //}
            //SqliteCache.ExecuteNonQuery(param.chkDB, sb.Remove(sb.Length - 1, 1).ToString());
            foreach (var item in obj)
            {
                TriggerEvent(item, new DecorationArgs() { Value = value });
            }
        }

        public string GetDbFilePath(string devicePath)
        {
            string path = Path.Combine(devicePath, DPDirName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, DPDBName);
        }

        private (string chkDB, string key) GetParam(object obj)
        {
            string chkDB;
            string key;
            if (obj is IDecoration de)
            {
                chkDB = (string)de.GetMetaData(this);
                key = de.GetKey(this);
            }
            else
            {
                chkDB = MetaData?.ToString();
                key = obj.GetHashCode().ToString();
            }
            return (chkDB == null ? null : GetDbFilePath(chkDB), key);
        }

        private string GetKey(object obj)
        {
            string key;
            if (obj is IDecoration de)
            {
                key = de.GetKey(this);
            }
            else
            {
                key = obj.GetHashCode().ToString();
            }
            return key;
        }
    }
}
