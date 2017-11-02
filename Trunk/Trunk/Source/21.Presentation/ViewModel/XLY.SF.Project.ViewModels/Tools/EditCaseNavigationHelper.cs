using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.Model.MessageElement;

namespace XLY.SF.Project.ViewModels.Tools
{
    /// <summary>
    /// 案例编辑界面导航服务
    /// </summary>
    public class EditCaseNavigationHelper
    {
        /// <summary>
        /// 当前案例编辑界面打开状态
        /// </summary>
        public static bool CurEditViewOpenStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// 设置子界面状态
        /// </summary>
        /// <param name="status">是否展开创建案例界面</param>
        public static void SetEditCaseViewStatus(bool isExpanded)
        {
            SubViewMsgModel curStatus = new SubViewMsgModel(isExpanded);
            SysCommonMsgArgs<SubViewMsgModel> sysArgs = new SysCommonMsgArgs<SubViewMsgModel>(SystemKeys.SetSubViewStatus);
            sysArgs.Parameters = curStatus;
            MsgAggregation.Instance.SendSysMsg<SubViewMsgModel>(sysArgs);

            NavigationArgs args = new NavigationArgs(ExportKeys.CaseCreationView, null);
            MsgAggregation.Instance.SendNavigationMsgForMainView(args);
            CurEditViewOpenStatus = isExpanded;
        }
    }
}
