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
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Shell.NavigationManager;

namespace XLY.SF.Shell.SelectService
{
    [Export(typeof(IPopupWindowService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConcreateSelectService : IPopupWindowService
    {
        #region properties



        #endregion

        private ConcreateSelectService()
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
            var view = IocManagerSingle.Instance.GetViewPart(exportKey);
            view.DataSource.LoadViewModel(parameters);
            var viewContainer = WindowHelper.Instance.CreateShellWindow(view, showInTaskBar, Application.Current.MainWindow);
            viewContainer.ShowDialog();
            return view.DataSource.GetResult();
        }

        #endregion

        #region Tools

        ///// <summary>
        ///// 创建并加载View
        ///// </summary>
        ///// <param name="view"></param>
        ///// <param name="parameters"></param>
        ///// <param name="showInTaskBar"></param>
        ///// <returns></returns>
        //private Shell CreateShellByLoad(UcViewBase view, object parameters, bool showInTaskBar)
        //{
        //    view.DataSource.LoadViewModel(parameters);
        //    var viewContainer = WindowHelper.Instance.CreateShellWindow(view, showInTaskBar, Application.Current.MainWindow);
        //    return viewContainer;
        //}

        private void ResetService()
        {

        }

        #endregion
    }
}
