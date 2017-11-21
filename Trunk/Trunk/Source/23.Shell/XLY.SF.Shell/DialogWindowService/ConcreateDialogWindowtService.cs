using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Extension.Helper;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Shell.NavigationManager;

namespace XLY.SF.Shell.DialogWindowService
{
    [Export(typeof(IPopupWindowService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConcreateDialogWindowtService : IPopupWindowService
    {
        #region properties



        #endregion

        private ConcreateDialogWindowtService()
        {
        }

        #region 打开文件，选择路径，保存文件服务

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="filter">筛选</param>
        /// <returns></returns>
        public string OpenFileDialog(string filter)
        {
            string result = string.Empty;
            var view = IocManagerSingle.Instance.GetViewPart(ExportKeys.SelectControlView);
            view.DataSource.LoadViewModel(filter);
            var viewContainer = WindowHelper.Instance.CreateShellWindow(view, false, Application.Current.MainWindow);
            viewContainer.ShowDialog();
            if (view.DataSource.DialogResult)
                result = view.DataSource.GetResult()?.ToString();

            return result;
        }

        #endregion

        #region 弹窗服务

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="exportKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ShowDialogWindow(string exportKey, object parameters, bool showInTaskBar)
        {
            object result = null;
            UcViewBase targetView;
            var viewArgs = CreateNavigationArgs(exportKey, parameters, showInTaskBar, out targetView);

            var viewContainer = WindowHelper.Instance.CreateShellWindow(targetView, viewArgs.ShowInTaskBar, Application.Current.MainWindow);
            viewContainer.ShowDialog();
            if (targetView.DataSource.DialogResult)
                result = targetView.DataSource.GetResult()?.ToString();

            return result;
        }

        #endregion

        #region Tools

        /// <summary>
        /// 创建导航消息，但并不用导航事件发送
        /// </summary>
        /// <param name="exportKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private NormalNavigationArgs CreateNavigationArgs(string exportKey, object parameters, bool showInTaskBar, out UcViewBase targetView)
        {
            if (string.IsNullOrWhiteSpace(exportKey))
                throw new NullReferenceException(string.Format("导航目标窗体Key【{0}】为空", exportKey));
            NormalNavigationArgs result = NormalNavigationArgs.CreateWindowNavigationArgs(exportKey, parameters, showInTaskBar);
            targetView = NavigationViewCreater.CreateView(result.MsgToken, result.Parameter);
            return result;
        }

        private void ResetService()
        {

        }

        #endregion
    }
}
