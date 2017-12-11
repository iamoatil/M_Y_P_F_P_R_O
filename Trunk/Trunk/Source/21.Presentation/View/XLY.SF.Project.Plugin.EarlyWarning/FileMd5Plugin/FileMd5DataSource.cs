using System;
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
            Items = new DataItems<FileMd5>(dbFilePath);
            Type = typeof(FileMd5);
        }
    }
}
