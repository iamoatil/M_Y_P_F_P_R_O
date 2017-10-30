using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:35:43
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.MessageBase
{
    /// <summary>
    /// 普通通知消息
    /// </summary>
    public class GeneralArgs : ArgsBase
    {
        public GeneralArgs(string generalKey)
        {
            base.MsgToken = generalKey;
        }
    }

    /// <summary>
    /// 普通通知消息
    /// </summary>
    public class GeneralArgs<TParam> : GeneralArgs, IArgsParameter<TParam>
    {
        public GeneralArgs(string generalKey)
            : base(generalKey)
        {
        }

        public TParam Parameters { get; set; }
    }
}
