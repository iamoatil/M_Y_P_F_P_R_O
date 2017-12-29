using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.MessageBase.Navigation;
using XLY.SF.Framework.Core.Base.ViewModel;
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
        /// 展开前的界面
        /// </summary>
        private static object beforeViewOnIsExpanded;

        /// <summary>
        /// 设置子界面状态
        /// </summary>
        /// <param name="isExpanded">是否展开创建案例界面</param>
        /// <param name="needCaseInfo">是否需要传递案例信息</param>
        /// <param name="needCollapsedNavigation">是否需要折叠导航，true：折叠案例编辑界面时自动返回最后一个非案例编辑界面</param>
        public static void SetEditCaseViewStatus(bool isExpanded, bool needCaseInfo, bool needCollapsedNavigation = true)
        {
            //便于以后扩展用
            SubViewMsgModel curStatus = new SubViewMsgModel(isExpanded);
            //展开或收起案例名
            SysCommonMsgArgs<SubViewMsgModel> sysArgs = new SysCommonMsgArgs<SubViewMsgModel>(SystemKeys.SetSubViewStatus);
            sysArgs.Parameters = curStatus;
            MsgAggregation.Instance.SendSysMsg<SubViewMsgModel>(sysArgs);

            if (isExpanded)
            {
                //展开案例编辑界面
                var @params = needCaseInfo ? SystemContext.Instance.CurrentCase : null;
                NormalNavigationArgs args = NormalNavigationArgs.CreateMainViewNavigationArgs(ExportKeys.CaseCreationView, @params);
                MsgAggregation.Instance.SendNavigationMsgForMainView(args);
            }
            else if (needCollapsedNavigation)
            {
                /*
                 * TODO
                 * 此处特殊处理
                 * 因为案例编辑界面是固定的子界面
                 * 同时为了防止返回时界面状态丢失
                 * 所以将打开案例管理界面之前的界面缓存
                 * 在发送导航消息时【参数就是缓存的上一个界面】
                 * 
                 * beforeViewOnIsExpanded==null：刚启动程序，登录进入主界面。由于逻辑问题会在登录后执行一次返回
                 * 
                 */

                //折叠，还原为上个界面
                var beforeViewKey = NavigationLogHelper.GetBeforeViewKeyBySkipKeyAtMainView(ExportKeys.CaseCreationView);
                if (!string.IsNullOrWhiteSpace(beforeViewKey))
                {
                    NormalNavigationArgs args = beforeViewOnIsExpanded == null ?
                        NormalNavigationArgs.CreateMainViewNavigationArgs(beforeViewKey, null) : NormalNavigationArgs.CreateMainViewNavigationArgs(beforeViewKey, beforeViewOnIsExpanded, true);
                    MsgAggregation.Instance.SendNavigationMsgForMainView(args);
                }
            }
            CurEditViewOpenStatus = isExpanded;
        }

        /// <summary>
        /// 记录展开前的界面【用于案例界面返回】
        /// </summary>
        public static void RecordBeforeViewOnisExpanded(object beforeViewOnIsExpanded)
        {
            EditCaseNavigationHelper.beforeViewOnIsExpanded = beforeViewOnIsExpanded;
        }

        /// <summary>
        /// 重置当前案例界面状态【折叠】
        /// </summary>
        public static void ResetCurCaseStatus()
        {
            CurEditViewOpenStatus = false;
            //便于以后扩展用
            SubViewMsgModel curStatus = new SubViewMsgModel(CurEditViewOpenStatus);
            //展开或收起案例名
            SysCommonMsgArgs<SubViewMsgModel> sysArgs = new SysCommonMsgArgs<SubViewMsgModel>(SystemKeys.SetSubViewStatus);
            sysArgs.Parameters = curStatus;
            MsgAggregation.Instance.SendSysMsg<SubViewMsgModel>(sysArgs);
        }
    }
}
