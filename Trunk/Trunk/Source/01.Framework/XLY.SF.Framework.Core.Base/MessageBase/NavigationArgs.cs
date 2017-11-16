using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageBase.Navigation;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.Log4NetService.LoggerEnum;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:36:08
 * 类功能说明：
 * 1.用于发送导航消息
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.MessageBase
{
    /// <summary>
    /// 导航消息
    /// </summary>
    public class NavigationArgs : ArgsBase
    {
        #region Properties

        /// <summary>
        /// 目前待定
        /// </summary>
        public Guid ViewModelID { get; private set; }

        /// <summary>
        /// 导航目标视图
        /// </summary>
        public UcViewBase TargetView { get; private set; }

        /// <summary>
        /// 初始化时参数
        /// </summary>
        public object InitParameter { get; private set; }

        #region 窗体控制属性

        /// <summary>
        /// 是否显示任务条
        /// </summary>
        public bool ShowInTaskBar { get; private set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool TopMost { get; private set; }

        #endregion

        #endregion

        #region 构造【私有】

        /// <summary>
        /// View关闭消息
        /// </summary>
        /// <param name="openedViewModelID">要关闭的ViewModelID</param>
        private NavigationArgs(Guid openedViewModelID)
        {
            this.ViewModelID = openedViewModelID;
        }

        /// <summary>
        /// 带参数的导航消息
        /// </summary>
        /// <param name="exportViewkey">显示的View</param>
        /// <param name="parameter">创建View时的参数</param>
        /// <param name="showInTaskBar">是否显示任务条</param>
        private NavigationArgs(string exportViewkey, object parameter, bool showInTaskBar = false, bool isTopMost = false)
        {
            ShowInTaskBar = showInTaskBar;
            CreateView(exportViewkey, parameter);
        }

        #endregion

        #region 创建

        /// <summary>
        /// 创建主界面导航消息
        /// </summary>
        /// <param name="exportViewkey">显示的View</param>
        /// <param name="parameter">创建View时的参数</param>
        /// <returns></returns>
        public static NavigationArgs CreateMainViewNavigationArgs(string exportViewkey, object parameter)
        {
            NavigationArgs argsResult = new NavigationArgs(exportViewkey, parameter);
            //记录导航日志
            NavigationLogHelper.AddNavigationLog(argsResult, false);
            return argsResult;
        }

        /// <summary>
        /// 创建窗体导航消息
        /// </summary>
        /// <param name="exportViewkey">显示的View</param>
        /// <param name="parameter">创建View时的参数</param>
        /// <param name="showInTaskBar">是否显示任务条</param>
        /// <param name="isTopMost">是否置顶显示</param>
        /// <returns></returns>
        public static NavigationArgs CreateWindowNavigationArgs(string exportViewkey, object parameter, bool showInTaskBar = false, bool isTopMost = false)
        {
            NavigationArgs argsResult = new NavigationArgs(exportViewkey, parameter, showInTaskBar, isTopMost);
            //记录导航日志
            NavigationLogHelper.AddNavigationLog(argsResult, true);
            return argsResult;
        }

        /// <summary>
        /// 创建关闭导航消息
        /// </summary>
        /// <param name="openedViewModelID"></param>
        /// <returns></returns>
        public static NavigationArgs CreateCloseNavigationArgs(Guid openedViewModelID)
        {
            NavigationArgs argsResult = new NavigationArgs(openedViewModelID);
            return argsResult;
        }

        #endregion

        #region 创建消息【创建View】

        /// <summary>
        /// 创建消息参数
        /// </summary>
        /// <param name="exportViewkey"></param>
        /// <param name="parameter">传递的参数</param>
        private void CreateView(string exportViewkey, object parameter = null)
        {
            if (!string.IsNullOrEmpty(exportViewkey))
            {
                var view = IocManagerSingle.Instance.GetViewPart(exportViewkey);
                if (view != null)
                {
                    //传递参数
                    if (!view.DataSource.IsLoaded)
                    {
                        try
                        {
                            view.DataSource.LoadViewModel(parameter);
                        }
                        catch (Exception ex)
                        {
                            LoggerManagerSingle.Instance.Error(ex, string.Format("加载模块Key【{0}】失败", exportViewkey));
                            return;
                        }
                    }
                    this.ViewModelID = view.DataSource.ViewModelID;
                    //加载ViewContainer
                    view.DataSource.SetViewContainer(view);
                    this.TargetView = view;
                    base.MsgToken = exportViewkey;
                }
                else
                    LoggerManagerSingle.Instance.Error(string.Format("导入模块Key【{0}】失败", exportViewkey));
            }
        }

        #endregion
    }
}
