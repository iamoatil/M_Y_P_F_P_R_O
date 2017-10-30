/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 10:41:54 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Domains.Contract
{
    /// <summary>
    /// 通话记录数据库
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public class CallDataParse : AbstractDataSource
    {
        public CallDataParse(string dbFilePath)
        {
            Items = new DataItems<Call>(dbFilePath);
        }
    }
}
