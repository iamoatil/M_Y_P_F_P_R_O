using System;
using System.Data;
using System.Data.SQLite;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public class FileMd5DataSource : AbstractDataSource
    {
        /// <summary>
        /// FileMd5数据存放的表名
        /// </summary>
        public string TableName { get; private set; }

        public FileMd5DataSource(string dbFilePath)
        {
            TableName = "Table_AutoEarlyWarning";
            //新建TableName为名字的表
            Items = new DataItems<FileMd5DataItem>(dbFilePath, true, TableName);
            Items.DbInstance.DeleteAll(TableName);
            Items.Commit();

            this.PluginInfo = new DataParsePluginInfo();
            PluginInfo.Name ="文件MD5";
            PluginInfo.OrderIndex =0;
            PluginInfo.Group = "MD5";
            PluginInfo.GroupIndex =200;

            Type = typeof(FileMd5DataItem);
        }
    }
}
