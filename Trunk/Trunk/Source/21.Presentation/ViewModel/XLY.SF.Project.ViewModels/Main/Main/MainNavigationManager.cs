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


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/5/10 15:53:45
 * 类功能说明：
 * 1.主要管理主界面内的导航
 * 
 *************************************************/

namespace XLY.SF.Project.ViewModels.Main
{
    public class MainNavigationManager : XLY.SF.Framework.Core.Base.ViewModel.NotifyPropertyBase
    {
        public MainNavigationManager()
        {
            SystemContext.Instance.CaseChanged += Instance_CaseChanged;
        }

        #region 主界面导航

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

        #region 界面显示调整

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
