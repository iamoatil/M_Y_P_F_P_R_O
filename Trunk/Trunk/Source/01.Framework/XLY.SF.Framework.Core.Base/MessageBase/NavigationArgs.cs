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
    /// 普通导航消息
    /// </summary>
    public class NormalNavigationArgs : ArgsBase
    {
        #region Properties

        /// <summary>
        /// 初始化时参数
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 是否为返回消息【返回上一个界面】
        /// </summary>
        public bool IsBackArgs { get; set; }

        #region 窗体控制属性

        /// <summary>
        /// 是否显示任务条
        /// </summary>
        public bool ShowInTaskBar { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool TopMost { get; set; }

        #endregion

        #endregion

        #region 构造【私有】

        /// <summary>
        /// 带参数的导航消息
        /// </summary>
        /// <param name="exportViewkey">显示的View</param>
        /// <param name="parameter">创建View时的参数</param>
        /// <param name="showInTaskBar">是否显示任务条</param>
        private NormalNavigationArgs(string exportViewkey,
                                object parameter,
                                bool showInTaskBar = false,
                                bool isTopMost = false)
            : base(exportViewkey)
        {
            ShowInTaskBar = showInTaskBar;
            TopMost = isTopMost;
            Parameter = parameter;
        }

        #endregion

        #region 创建

        /// <summary>
        /// 创建主界面导航消息
        /// </summary>
        /// <param name="exportViewkey">显示的View</param>
        /// <param name="parameter">创建View时的参数</param>
        /// <param name="isBackArgs">是否为返回消息</param>
        /// <returns></returns>
        public static NormalNavigationArgs CreateMainViewNavigationArgs(string exportViewkey, object parameter, bool isBackArgs = false)
        {
            NormalNavigationArgs argsResult = new NormalNavigationArgs(exportViewkey, parameter);
            argsResult.IsBackArgs = isBackArgs;
            //记录导航日志
            NavigationLogHelper.AddNavigationLog(argsResult, false);
            return argsResult;
        }

        /// <summary>
        /// 创建新窗体导航消息
        /// </summary>
        /// <param name="exportViewkey">显示的View</param>
        /// <param name="parameter">创建View时的参数</param>
        /// <param name="showInTaskBar">是否显示任务条</param>
        /// <param name="isTopMost">是否置顶显示</param>
        /// <returns></returns>
        public static NormalNavigationArgs CreateWindowNavigationArgs(string exportViewkey, object parameter, bool showInTaskBar = false, bool isTopMost = false)
        {
            NormalNavigationArgs argsResult = new NormalNavigationArgs(exportViewkey, parameter, showInTaskBar, isTopMost);
            //记录导航日志
            NavigationLogHelper.AddNavigationLog(argsResult, true);
            return argsResult;
        }

        #endregion
    }

    /// <summary>
    /// 关闭打开窗体消息
    /// </summary>
    public class CloseViewOfNewWindowArgs : ArgsBase
    {
        public CloseViewOfNewWindowArgs(Guid closeViewModelID)
            : base(SystemKeys.CloseWindow)
        {
            CloseViewModelID = closeViewModelID;
        }

        /// <summary>
        /// 要关闭的ViewModelID
        /// </summary>
        public Guid CloseViewModelID { get; }
    }
}
