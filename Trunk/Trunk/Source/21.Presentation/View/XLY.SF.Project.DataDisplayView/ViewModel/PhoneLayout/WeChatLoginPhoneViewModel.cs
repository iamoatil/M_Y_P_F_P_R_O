using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataDisplayView.ViewModel.PhoneLayout.WeChatLoginPhoneViewModel
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/2 15:50:23
* ==============================================================================*/

namespace XLY.SF.Project.DataDisplayView.ViewModel.PhoneLayout
{
    /// <summary>
    /// WeChatLoginPhoneViewModel
    /// </summary>
    public class WeChatLoginPhoneViewModel : ViewModelBase
    {
        #region 账号列表
        private ObservableCollection<object> _Accounts;

        /// <summary>
        /// 账号列表
        /// </summary>	
        public ObservableCollection<object> Accounts
        {
            get { return _Accounts; }
            set
            {
                _Accounts = value;
                OnPropertyChanged();
            }
        }
        #endregion

    }
}
