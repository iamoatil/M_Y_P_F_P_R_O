using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Project.ViewDomain.Model.MessageElement;

namespace XLY.SF.Project.ViewModels.Tools
{
    /// <summary>
    /// 子界面帮助类
    /// </summary>
    public class SubViewNavigationHelper
    {
        /// <summary>
        /// 设置子界面状态
        /// </summary>
        /// <param name="status">子界面状态</param>
        public static void SetSubViewStatus(SubViewMsgModel status)
        {
            SysCommonMsgArgs<SubViewMsgModel> args = new SysCommonMsgArgs<SubViewMsgModel>(SystemKeys.SetSubViewStatus);
            args.Parameters = status;
            MsgAggregation.Instance.SendSysMsg<SubViewMsgModel>(args);
        }
    }
}
