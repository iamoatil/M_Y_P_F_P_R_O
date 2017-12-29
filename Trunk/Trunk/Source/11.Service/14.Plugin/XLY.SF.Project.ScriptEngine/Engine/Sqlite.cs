using System;
using System.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace XLY.SF.Project.ScriptEngine
{
    /// <summary>
    /// Sqlite数据库操作
    /// </summary>
    public class Sqlite
    {
        /// <summary>
        /// 特征库文件的基本路径，为脚本文件的目录
        /// </summary>
        public string CharatorBasePath { get; set; }
        /// <summary>
        /// 数据分析恢复：指定源数据文件、特征库，及表名称，表名称多个以逗号','隔开。
        /// 恢复成功返回新的db路径
        /// 恢复失败则返回原来的路径
        /// </summary>
        public string DataRecovery(string sourcedb, string charatorPath, string tableNames, bool isScanDebris)
        {
            return SqliteRecoveryHelper.DataRecovery(sourcedb, System.IO.Path.Combine(CharatorBasePath, charatorPath), tableNames, isScanDebris);
        }

        /// <summary>
        /// 数据分析恢复：指定源数据文件、特征库，及表名称，表名称多个以逗号','隔开。
        /// 恢复成功返回新的db路径
        /// 恢复失败则返回原来的路径
        /// </summary>
        public string DataRecovery(string sourcedb, string charatorPath, string tableNames)
        {
            return SqliteRecoveryHelper.DataRecovery(sourcedb, System.IO.Path.Combine(CharatorBasePath, charatorPath), tableNames);
        }


        /// <summary>
        /// 查询sqlite数据库：参数1：文件路径；参数2:sql语句
        /// columns为列名，多个逗号隔开；encodestr为编码格式，如UTF-8，Unicode，GBK
        /// </summary>
        public string Find(string file, string sql, string columns, string encodestr)
        {
            try
            {
                var list = this._Find(file, sql);
                this.Encode(list, columns, encodestr);
                return JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                var str = string.Format("execute sql errors: {0}, {1}", file, ex.AllMessage());
                Console.WriteLine(str);
                return string.Empty;
            }
        }

        public string Find(string file, string sql)
        {
            return this.Find(file, sql, string.Empty, string.Empty);
        }

        #region BigFind
        private List<dynamic> list = new List<dynamic>();
        /// <summary>
        /// 执行sql，获取动态类型数据集合
        /// </summary>
        public void BigFindInit(string file, string sql)
        {
            try
            {
                list = this._Find(file, sql);
                this.Encode(list, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                var str = string.Format("execute sql error: {0}, {1}", file, ex.AllMessage());
                Console.WriteLine(str);
                //LogHelper.Error(str, ex);
            }
        }
        public string BigFind()
        {
            List<dynamic> items = new List<dynamic>();
            if (list.Count != 0)
            {
                items = list.Take(10000).ToList();
                if (list.Count() >= 10000)
                    list.RemoveRange(0, 10000);
                else
                    list.RemoveRange(0, list.Count());
                var result = from u in items
                             select new
                             {
                                 Id = u.fid,
                                 Name = u.file_name,
                                 MD5 = u.file_md5,
                                 Path = u.server_path,
                                 Size = u.file_size + " bytes",
                                 Time = (new Convert()).LinuxToDateTime(u.server_ctime),
                                 DataState = (new Convert()).ToDataState(u.XLY_DataType)
                             };
                return JsonConvert.SerializeObject(result).Replace("[{", "{").Replace("}]", "}");
            }
            else
            {
                return "";
            }
        }

        #endregion

        public string FindAndSort(string file, string sql, string sortKeys = "")
        {
            try
            {
                if (!System.IO.File.Exists(file))
                {
                    Console.WriteLine(string.Format("File {0} is not exist", file));
                    return null;
                }
                SqliteContext context = new SqliteContext(file);
                var list = context.FindDataTableAndSort(sql, sortKeys);
                this.Encode(list, string.Empty, string.Empty);
                return JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("execute sql errors: {0}, {1}", file, ex.AllMessage()));
                throw;
            }
        }

        /// <summary>
        /// 查询sqlite数据库：参数1：文件路径；参数2:表名称
        /// columns为列名，多个逗号隔开；encodestr为编码格式，如UTF-8，Unicode，GBK
        /// </summary>
        public string FindByName(string file, string name, string columns, string encodestr)
        {
            try
            {
                var sql = string.Format("Select * from {0}", name);
                var list = this._Find(file, sql);
                this.Encode(list, columns, encodestr);
                return list == null ? "[]" : JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                var str = string.Format("execute sql errors: {0}, {1}", file, ex.AllMessage());
                Console.WriteLine(str);
                //LogHelper.Error(str, ex);
                return string.Empty;
            }
        }

        public string FindByName(string file, string name)
        {
            return this.FindByName(file, name, string.Empty, string.Empty);
        }

        private List<dynamic> _Find(string file, string sql)
        {
            try
            {
                if (!System.IO.File.Exists(file))
                {
                    Console.WriteLine(string.Format("File {0} is not Found", file));
                    return null;
                }
                using (SqliteContext context = new SqliteContext(file))
                {
                    var res = context.Find(new SQLiteString(sql));
                    return res;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("execute sql errors: {0}, {1}", file, ex.AllMessage()));
                throw;
            }
        }

        /// <summary>
        /// 对byte数据进行转码 。
        /// columns为列名，多个逗号隔开；encodestr为编码格式，如UTF-8，Unicode，GBK
        /// </summary>
        private void Encode(List<dynamic> items, string columns, string encodestr)
        {
            if (columns.IsInvalid() || encodestr.IsInvalid()) return;
            if (items.IsInvalid()) return;
            var cs = columns.Replace("，", ",").Split(',');
            var encode = System.Text.Encoding.GetEncoding(encodestr);
            foreach (var item in items)
            {
                try
                {
                    DynamicX obj = (DynamicX)item;
                    foreach (var c in cs)
                    {
                        var value = obj.Get(c) as byte[];
                        obj.Set(c, value.GetString(encode).Replace("\0", ""));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("tranvse encoding error: " + e.AllMessage());
                }

            }
        }

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public bool TableIsExist(string file, string tablename)
        {
            try
            {
                SqliteContext context = new SqliteContext(file);
                return context.Exist(tablename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("execute sql errors: {0}, {1}", file, ex.AllMessage()));
                throw;
            }
        }

        /// <summary>
        /// 查询数据库，同时通过正则表达式过滤数据
        /// </summary>
        /// <param name="file">db文件</param>
        /// <param name="sql">SQL</param>
        /// <param name="colNames">过滤的列，和regex一一对应</param>
        /// <param name="regex">过滤的列对应的正则表达式，和regex一一对应</param>
        /// <returns></returns>
        public string FindByRegex(string file, string sql, string[] colNames, string[] regex)
        {
            if (!colNames.IsValid() || !regex.IsValid())
            {
                return Find(file, sql);
            }

            try
            {
                if (colNames.Length != regex.Length)
                {
                    throw new Exception("the length of columns is not equal the length of regex");
                }

                //var list = this._Find(file, sql);
                if (!System.IO.File.Exists(file))
                {
                    Console.WriteLine(string.Format("File {0} is not Found", file));
                    return null;
                }
                SqliteContext context = new SqliteContext(file);
                var dt = context.FindDataTable(sql);

                Dictionary<string, Regex> dicRegexes = new Dictionary<string, Regex>();
                for (int j = 0; j < colNames.Length; j++)
                {
                    dicRegexes[colNames[j]] = new Regex(regex[j]);
                }

                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (dicRegexes.Any(r => !r.Value.IsMatch(dt.Rows[i][r.Key].ToSafeString())))        //如果某列值不满足正则匹配，则删除该行数据
                    {
                        dt.Rows.RemoveAt(i);
                    }
                }

                var list = dt.ToDynamicCollection();
                this.Encode(list, string.Empty, string.Empty);
                return JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                var str = string.Format("execute sql errors: {0}, {1}", file, ex.AllMessage());
                Console.WriteLine(str);
                //LogHelper.Error(str, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 解密支付宝数据
        /// </summary>
        /// <param name="ChatMsgDBFile"></param>
        /// <param name="ShareXmlFile"></param>
        /// <param name="Dencrypt_File"></param>
        /// <returns></returns>
        public string DencryptChatMsg(string ChatMsgDBFile, string ShareXmlFile, string Dencrypt_File)
        {
            return dencrypt_chatMsg(ChatMsgDBFile, ShareXmlFile, Dencrypt_File).ToString();
        }

        [DllImport("AliPay_Android.dll", EntryPoint = "dencrypt_chatMsg")]
        public static extern int dencrypt_chatMsg(string ChatMsgDBFile, string ShareXmlFile, string Dencrypt_File);

    }
}
