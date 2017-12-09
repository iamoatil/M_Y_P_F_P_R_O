using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Themes;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Shell.CommWindow;
using XLY.SF.Project.Extension.Helper;
using ProjectExtend.Context;
using XLY.SF.Project.ViewDomain.Model.PresentationNavigationElement;
using XLY.SF.Framework.Log4NetService;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:19:23
 * 类功能说明：
 *      1. 非模式对话框导航管理
 *
 *************************************************/

namespace XLY.SF.Shell.NavigationManager
{
    public class WindowNavigationHelper
    {

        public WindowNavigationHelper()
        {

        }

        /// <summary>
        /// 注册导航消息
        /// </summary>
        public void RegisterNavigation()
        {
            //注册监听消息
            MsgAggregation.Instance.RegisterSysMsg(this, SystemKeys.LoginComplete, LoginSuccessCallback);

            //注册打开新窗口事件
            MsgAggregation.Instance.RegisterNaviagtionMsg(this, SystemKeys.OpenNewWindow, OpenNewWindowCallback);
            //注册关闭窗口事件
            MsgAggregation.Instance.RegisterNavigationOfCloseWindow(this, CloseWindowCallback);
        }

        #region 弹窗导航

        #region 关闭窗体

        //通过消息关闭打开的窗体
        private void CloseWindowCallback(CloseViewOfNewWindowArgs args)
        {
            WindowHelper.Instance.CloseOpenedWindow(args.CloseViewModelID);
        }

        //直接关闭窗体触发
        private void NewWindow_Closed(object sender, EventArgs e)
        {
            var curWin = sender as Shell;
            if (curWin != null)
            {
                WindowHelper.Instance.RemoveOpenedWindowAndCleanUp(curWin.Content.ViewID);
            }
        }

        #endregion

        //打开非模式对话框
        private void OpenNewWindowCallback(NormalNavigationArgs args)
        {
            /*
             * TODO
             * 
             * 由于可以同时提取多个手机，所以设备主页会多次打开【通过设备ID区分界面】
             * 设备主页同时会在新窗口中显示【默认在主界面导航】
             * 此处需要从缓存中查找和创建
             * 由于主进程Shell未引用ViewModel模块，所以此处只能折中使用object[]传值【主要需要Device.ID来区分界面，object[0]为Device.ID】
             * 
             */
            UcViewBase targetView = null;
            if (args.MsgToken == ExportKeys.DeviceMainView)
            {
                var devTmp = args.Parameter as object[];
                if (devTmp != null && devTmp.Length == 2)
                {
                    PreCacheToken delToken = new PreCacheToken(devTmp[0].ToString(), ExportKeys.DeviceMainView);

                    if (!SystemContext.Instance.CurCacheViews.TryGetFirstView(delToken, out targetView))
                    {
                        targetView = NavigationViewCreater.CreateView(args.MsgToken, devTmp[1]);
                        SystemContext.Instance.CurCacheViews.AddViewCache(delToken, targetView);
                    }
                    targetView.Title = devTmp[0].ToString();

                    //此处暂用Tag存储Device.ID
                    targetView.Tag = devTmp[1];
                }
            }
            else
            {
                targetView = NavigationViewCreater.CreateView(args.MsgToken, args.Parameter);
            }
            if (targetView != null)
            {
                var newWindow = WindowHelper.Instance.CreateShellWindow(targetView, args.ShowInTaskBar, Application.Current.MainWindow);
                if (args.MsgToken == ExportKeys.DeviceMainView)
                {
                    //此处暂用Tag存储Device.ID
                    newWindow.Tag = targetView.Tag;

                    newWindow.IsBackWindow = true;
                    newWindow.Width = 1500;
                    newWindow.Height = 1000;
                    newWindow.SizeToContent = SizeToContent.Manual;
                }
                
                newWindow.Closed += NewWindow_Closed;
                newWindow.Show();
            }
        }

        #endregion

        #region 登录成功

        private void LoginSuccessCallback(SysCommonMsgArgs args)
        {
            //注销登录界面消息监听
            MsgAggregation.Instance.UnRegisterMsg<SysCommonMsgArgs>(this, SystemKeys.LoginComplete, LoginSuccessCallback);

            //注册主界面，正式启动程序
            var view = IocManagerSingle.Instance.GetViewPart(ExportKeys.ModuleMainUcView);
            Application.Current.MainWindow.Content = view;
            view.DataSource.LoadViewModel(null);
            Application.Current.MainWindow.Show();
        }

        #endregion

        #region 执行程序初始化

        /* 由于初始化流程不便 
         * 此处独立处理
         * 等待加载页面关闭后（关闭动画完成）
         * 再进入登录界面
         */
        /// <summary>
        /// 执行初始化程序
        /// </summary>
        public void ExecuteProgramInitialise()
        {
            ////加载界面
            //var loadingView = IocManagerSingle.Instance.GetViewPart(ExportKeys.ModuleLoadingView);
            //loadingView.DataSource.LoadViewModel(XLY.SF.Shell.Properties.Resources.ProposedSolutionConfig);         //传递推荐配置内容
            //var loadingWindow = WindowHelper.Instance.CreateShellWindow(loadingView, false);
            //loadingWindow.Show();

            //var loginView = IocManagerSingle.Instance.GetViewPart(ExportKeys.ModuleLoginView);
            //loginView.DataSource.LoadViewModel(XLY.SF.Shell.Properties.Resources.ProposedSolutionConfig);         //传递推荐配置内容
            //var loginWindow = WindowHelper.Instance.CreateShellWindow(loginView, true);
            //loginWindow.Show();

            NormalNavigationArgs loginArgs = NormalNavigationArgs.CreateWindowNavigationArgs(ExportKeys.ModuleLoginView,
                XLY.SF.Shell.Properties.Resources.ProposedSolutionConfig, true, true);
            MsgAggregation.Instance.SendNavigationMsgForWindow(loginArgs);
        }

        #endregion        
    }
}
