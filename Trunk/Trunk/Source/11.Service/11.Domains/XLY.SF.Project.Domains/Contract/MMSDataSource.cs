/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/17 16:23:46 
 * explain :  
 *
*****************************************************************************/

using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 彩信数据集
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public class MMSDataSource : AbstractDataSource
    {
        public MMSDataSource(string dbFilePath)
        {
            Items = new DataItems<MMS>(dbFilePath);
            Type = typeof(MMS);
        }
    }
}
