using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataDisplayView.ViewModel.DefaultLayoutViewModel
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/30 19:16:49
* ==============================================================================*/

namespace XLY.SF.Project.DataDisplayView.ViewModel
{
    /// <summary>
    /// 默认布局控件ViewModel
    /// </summary>
    public class DefaultLayoutViewModel : ViewModelBase
    {
        public DefaultLayoutViewModel(DataViewPluginArgument arg)
        {
            _arg = arg;
        }

        private DataViewPluginArgument _arg;

        #region 数据
        #region 绑定的树形菜单数据
        private object _treeNodes;

        /// <summary>
        /// 绑定的树形菜单数据
        /// </summary>	
        public object TreeNodes
        {
            get { return (_arg.DataSource as TreeDataSource).TreeNodes; }
            set
            {
                _treeNodes = value;
                OnPropertyChanged();
            }
        }
        #endregion



        #endregion
    }
}
