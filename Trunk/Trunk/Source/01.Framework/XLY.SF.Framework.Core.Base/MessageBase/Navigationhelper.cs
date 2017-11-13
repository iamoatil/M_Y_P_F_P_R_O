using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;

namespace XLY.SF.Framework.Core.Base.MessageBase
{
    /// <summary>
    /// 导航帮助器
    /// </summary>
    public class Navigationhelper
    {
        #region 创建View

        /// <summary>
        /// 创建消息参数
        /// </summary>
        /// <param name="args">导航消息</param>
        /// <param name="targetView">导航的目标View</param>
        public static bool CreateNavigationView(NavigationArgs args, out UcViewBase targetView)
        {
            targetView = null;
            if (args != null && !string.IsNullOrEmpty(args.MsgToken))
            {
                //当前项目未添加必要引用，所以此处暂用try catch
                targetView = IocManagerSingle.Instance.GetViewPart(args.MsgToken);
                try
                {
                    //传递参数
                    if (!targetView.DataSource.IsLoaded)
                    {
                        targetView.DataSource.LoadViewModel(args.InitParameter);
                    }
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex, string.Format("加载模块Key【{0}】失败", args.MsgToken));
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
