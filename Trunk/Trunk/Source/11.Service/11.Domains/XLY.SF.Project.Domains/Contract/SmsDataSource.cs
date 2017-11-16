/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 10:37:51 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 短信数据集
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public class SmsDataSource : AbstractDataSource
    {
        public SmsDataSource(string dbFilePath)
        {
            Items = new DataItems<SMS>(dbFilePath);
            Type = typeof(SMS);
        }
    }
}
