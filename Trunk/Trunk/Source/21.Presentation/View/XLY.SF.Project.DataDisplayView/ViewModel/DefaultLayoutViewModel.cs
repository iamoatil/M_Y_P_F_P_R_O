using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            SelecedNodeChanged = new RelayCommand<object>(DoSelecedNodeChanged);
            if (!IsTreeDataSource)
            {
                ResetLayoutViews(_arg.DataSource?.PluginInfo?.Guid, (_arg.DataSource as AbstractDataSource)?.Type, _arg.DataSource);
            }
        }

        private DataViewPluginArgument _arg;

        #region 数据
        /// <summary>
        /// 绑定的树形菜单数据
        /// </summary>	
        public object TreeNodes=>(_arg.DataSource as TreeDataSource)?.TreeNodes; 

        /// <summary>
        /// 是否是TreeDataSource,为false则表示不显示TreeView
        /// </summary>
        public bool IsTreeDataSource => _arg.DataSource is TreeDataSource;

        #region 布局视图集合
        private ObservableCollection<object> _layoutViewItems;

        /// <summary>
        /// 布局视图集合
        /// </summary>	
        public ObservableCollection<object> LayoutViewItems
        {
            get { return _layoutViewItems; }
            set
            {
                _layoutViewItems = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 当前选择的布局视图
        private object _selectedLayoutViewItem;

        /// <summary>
        /// 当前选择的布局视图
        /// </summary>	
        public object SelectedLayoutViewItem
        {
            get { return _selectedLayoutViewItem; }
            set
            {
                _selectedLayoutViewItem = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        #region Commond
        #region 选择了某个节点的数据命令

        public RelayCommand<object> SelecedNodeChanged { get; set; }

        /// <summary>
        /// 选择了某个节点的数据命令
        /// </summary>
        private void DoSelecedNodeChanged(object node)
        {
            if (node == null)
                return;
            if (node is TreeNode treeItem)
            {
                ResetLayoutViews(_arg.DataSource?.PluginInfo?.Guid, treeItem.Type, treeItem);
            }
        }

        /// <summary>
        /// 重置界面元素
        /// </summary>
        /// <param name="pluginId">插件ID</param>
        /// <param name="type">数据Items类型</param>
        /// <param name="currentData">当前节点</param>
        private void ResetLayoutViews(string pluginId, object type, object currentData)
        {
            LayoutViewItems = new ObservableCollection<object>();
            bool isFirst = false;
            foreach (var item in DataViewPluginAdapter.Instance.GetView(pluginId, type))
            {
                LayoutViewItems.Add(item.ToControl(new DataViewPluginArgument() { CurrentData = currentData, DataSource = _arg.DataSource }, null));
                if (!isFirst)   //设置默认选中第一项
                {
                    SelectedLayoutViewItem = LayoutViewItems[0];
                    isFirst = true;
                }
            }
        }
        #endregion

        #endregion
    }
}
