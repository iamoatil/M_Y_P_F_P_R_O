using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Project.ViewDomain.MefKeys;
using ProjectExtend.Context;
using XLY.SF.Project.ViewModels.Tools;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/5/10 15:53:45
 * 类功能说明：
 * 1.管理主界面内的导航
 * 
 *************************************************/

namespace XLY.SF.Project.ViewModels.Main
{
    public class MainNavigationManager : XLY.SF.Framework.Core.Base.ViewModel.NotifyPropertyBase
    {
        #region Properties

        #region 主界面View

        private object _mainView;
        /// <summary>
        /// 子界面
        /// </summary>
        public object MainView
        {
            get
            {
                return _mainView;
            }
            set
            {
                _mainView = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 程序缓存View

        /// <summary>
        /// 当前缓存View
        /// </summary>
        private Dictionary<Guid, UcViewBase> _cacheViews;

        #endregion

        #endregion

        public MainNavigationManager()
        {
            _cacheViews = new Dictionary<Guid, UcViewBase>();

            SystemContext.Instance.CaseChanged += Instance_CaseChanged;

            //注册主界面导航消息
            MsgAggregation.Instance.RegisterNaviagtionMsg(this, SystemKeys.MainUcNavigation, MainNavigationCallback);
        }

        #region 主界面导航

        //主界面导航回调
        private void MainNavigationCallback(NavigationArgs args)
        {
            MainView = args.TargetView;
            ///*
            // * 由于ViewModel未引用WPF类库，所以无法用UcViewBase来做比较
            // * 此处使用object来判断是否导航成功
            // */
            //UcViewBase targetView;
            //if (Navigationhelper.CreateNavigationView(args, out targetView))
            //{
            //    MainView = targetView;
            //}
        }

        #endregion

        #region 【案例更新】

        private void Instance_CaseChanged(object sender, PropertyChangedEventArgs<Project.CaseManagement.Case> e)
        {
            if (e.NewValue == null)
            {
                IsShowCurCaseNameRow = false;
                IsShowDeviceListRow = false;
                EditCaseNavigationHelper.ResetCurCaseStatus();
            }
            else
            {
                IsShowDeviceListRow = true;
                IsShowCurCaseNameRow = true;
            }
        }

        #endregion

        #region 界面显示调整【此处设计不是很好】

        private bool _isShowCurCaseNameRow;
        /// <summary>
        /// 是否显示当前案例名称行
        /// </summary>
        public bool IsShowCurCaseNameRow
        {
            get
            {
                return this._isShowCurCaseNameRow;
            }

            set
            {
                this._isShowCurCaseNameRow = value;
                base.OnPropertyChanged();
            }
        }


        private bool _isShowDeviceListRow;
        /// <summary>
        /// 是否显示设备列表行
        /// </summary>
        public bool IsShowDeviceListRow
        {
            get
            {
                return this._isShowDeviceListRow;
            }

            set
            {
                this._isShowDeviceListRow = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
    }
}
