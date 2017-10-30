/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 10:34:24 
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
    /// 联系人数据集
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
    public class ContactDataSource : AbstractDataSource
    {
        public ContactDataSource(string dbFilePath)
        {
            Items = new DataItems<Contact>(dbFilePath);
        }
    }
}
