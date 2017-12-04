using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Plugin.Adapter;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Project.Plugin.DataPreview;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.DataPreview.View;
using System.Windows.Controls;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataPreview.ViewModel.MainPreviewModel
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/30 13:27:09
* ==============================================================================*/

namespace XLY.SF.Project.DataPreview.ViewModel
{
    /// <summary>
    /// MainPreviewModel
    /// </summary>
    [Export(ExportKeys.DataPreViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainPreviewModel : ViewModelBase
    {
        public MainPreviewModel()
        {

        }

        #region 事件
        protected override void InitLoad(object parameters)
        {
            LoadPlugin();
        }

        public override void ReceiveParameters(object parameters)
        {
            base.ReceiveParameters(parameters);
            ResetLayout(parameters);
        }

        #endregion

        #region 属性

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


        #endregion

        #region 方法
        /// <summary>
        /// 加载预览插件
        /// </summary>
        public void LoadPlugin()
        {
            if (DataPreviewPluginAdapter.Instance.Plugins == null || DataPreviewPluginAdapter.Instance.Plugins.Count() == 0)
            {
                DataPreviewPluginAdapter.Instance.Plugins = PluginAdapter.Instance.GetPluginsByType<DataPreviewPluginInfo>(PluginType.SpfDataPreview).ToList().ConvertAll(p => (AbstractDataPreviewPlugin)p.Value);
            }
        }

        /// <summary>
        /// 重新设置视图
        /// </summary>
        private void ResetLayout(object parameters)
        {
            Release();

            LayoutViewItems = new ObservableCollection<object>();
            if (parameters != null)
            {
                DataPreviewPluginArgument arg;
                if (parameters is string file)
                {
                    arg = new DataPreviewPluginArgument() { CurrentData = parameters };//@"C:\Users\fhjun\Desktop\130914+岁月如歌.mp3"
                }
                else
                {
                    dynamic p = parameters;
                    arg = new DataPreviewPluginArgument() { CurrentData = p.CurrentData, PluginId = p.PluginId, Type = p.Type };//@"C:\Users\fhjun\Desktop\130914+岁月如歌.mp3"
                }
                foreach (var item in DataPreviewPluginAdapter.Instance.GetView(arg))
                {
                    LayoutViewItems.Add(item.ToControl(arg));
                }
            }
            SelectedLayoutViewItem = LayoutViewItems.FirstOrDefault(); //设置默认选中第一项
        }

        /// <summary>
        /// 释放视图
        /// </summary>
        public override void Release()
        {
            base.Release();

            if (LayoutViewItems.IsValid())
            {
                foreach (var view in LayoutViewItems)
                {
                    if (view is TabItem ti)
                    {
                        if (null != ti && null != ti.Content && ti.Content is IDataPreviewRelease rv)
                        {
                            rv?.Release();
                        }
                    }
                }
            }

            LayoutViewItems?.Clear();
            LayoutViewItems = null;
            SelectedLayoutViewItem = null;
        }

        #endregion

        #region Tree
        private ObservableCollection<DataExtactionItem> _TreeNodes = null;

        /// <summary>
        /// Tree
        /// </summary>	
        public ObservableCollection<DataExtactionItem> TreeNodes
        {
            get { return _TreeNodes; }
            set
            {
                _TreeNodes = value;
                OnPropertyChanged();
            }
        }
        #endregion

    }
}
