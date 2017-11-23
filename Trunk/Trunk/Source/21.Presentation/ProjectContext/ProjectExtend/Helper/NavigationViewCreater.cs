using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;

namespace XLY.SF.Project.Extension.Helper
{
    /// <summary>
    /// 导航视图创建器
    /// </summary>
    public class NavigationViewCreater
    {
        #region 创建消息【创建View】

        /// <summary>
        /// 创建消息参数
        /// </summary>
        /// <param name="exportViewkey"></param>
        /// <param name="parameter">传递的参数</param>
        public static UcViewBase CreateView(string exportViewkey, object parameter)
        {
            UcViewBase targetView = null;
            if (!string.IsNullOrEmpty(exportViewkey))
            {
                targetView = IocManagerSingle.Instance.GetViewPart(exportViewkey);
                if (targetView != null)
                {
                    //传递参数
                    if (targetView.DataSource != null && !targetView.DataSource.IsLoaded)
                    {
                        try
                        {
                            targetView.DataSource.LoadViewModel(parameter);
                            //加载ViewContainer
                            targetView.DataSource.SetViewContainer(targetView);
                        }
                        catch (Exception ex)
                        {
                            LoggerManagerSingle.Instance.Error(ex, string.Format("加载模块Key【{0}】失败", exportViewkey));
                        }
                    }
                }
                else
                    LoggerManagerSingle.Instance.Error(string.Format("导入模块Key【{0}】失败", exportViewkey));
            }
            return targetView;
        }

        #endregion
    }
}
