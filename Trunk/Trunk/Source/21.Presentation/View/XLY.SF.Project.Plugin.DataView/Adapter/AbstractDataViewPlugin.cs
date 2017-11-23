using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Plugin;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataView.AbstractDataViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:05:22
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// 数据展示插件接口定义
    /// </summary>
    [Plugin]
    public abstract class AbstractDataViewPlugin : IPlugin
    {
        public IPluginInfo PluginInfo { get; set; }

        public object Execute(object arg, IAsyncTaskProgress progress)
        {
            return GetControl(arg as DataViewPluginArgument);
        }

        public void Dispose()
        {

        }

        public event DelgateDataViewSelectedItemChanged SelectedDataChanged;

        /// <summary>
        /// 触发数据选择事件
        /// </summary>
        /// <param name="data"></param>
        protected void OnSelectedDataChanged(object data)
        {
            SelectedDataChanged?.Invoke(data);
        }

        /// <summary>
        /// 获取显示的插件控件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public abstract FrameworkElement GetControl(DataViewPluginArgument arg);

        /// <summary>
        /// 将控件转换为TabItem，便于显示到界面上
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public FrameworkElement ToControl(DataViewPluginArgument arg)
        {
            SelectedDataChanged += arg.OnSelectedItemChanged;
            TabItem ti = new TabItem();
            if(!string.IsNullOrWhiteSpace(PluginInfo.Icon))      //设置了图标
            {
                try
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(PluginInfo.Icon, UriKind.RelativeOrAbsolute));
                    img.Stretch = System.Windows.Media.Stretch.UniformToFill;
                    img.Width = img.Height = 16;
                    img.ToolTip = PluginInfo.Name;
                    ti.Header = img;
                }
                catch (Exception)
                {

                }
            }
            if(ti.Header == null)
            {
                ti.Header = PluginInfo.Name;
            }
            ti.Content = GetControl(arg);
            return ti;
        }
    }

    /// <summary>
    /// 数据展示空控件当前选择项改变事件（主要用于数据预览窗口更新）
    /// </summary>
    /// <param name="data"></param>
    public delegate void DelgateDataViewSelectedItemChanged(object data);

    /// <summary>
    /// 数据展示时传递的参数
    /// </summary>
    public class DataViewPluginArgument
    {
        /// <summary>
        /// 当前的数据集合
        /// </summary>
        public IDataSource DataSource { get; set; }
        /// <summary>
        /// 当前的数据，如果是treedatasource，则为TreeNode，如果是SimpleDataSource，则为DataSource
        /// </summary>
        public object CurrentData { get; set; }

        /// <summary>
        /// 返回当前数据的列表Items
        /// </summary>
        public IDataItems Items => CurrentData is TreeNode node ? node.Items : 
            CurrentData is AbstractDataSource sp ? sp.Items : 
            null;

        /// <summary>
        /// 选择了某一项的事件，此时将会更新数据预览项
        /// </summary>
        public DelgateDataViewSelectedItemChanged OnSelectedItemChanged { get; set; }
    }

    /// <summary>
    /// 视图获取时的配置项
    /// </summary>
    public class DataViewConfigure
    {
        /// <summary>
        /// 表示用于主布局的插件类型（比如想添加手机视图，则可以添加该类型的插件）
        /// </summary>
        public const string XLY_LAYOUT_KEY = "__LayOut";

        /// <summary>
        /// 默认的表格视图的ID
        /// </summary>
        public const string DEFAULT_GRID_VIEW_ID = "7B51FA8D-F7F6-4EE3-B3B9-780C29B9B778";

        /// <summary>
        /// 当存在多视图时，是否隐藏默认的表格视图，默认为隐藏
        /// </summary>
        public bool IsDefaultGridViewVisibleWhenMultiviews { get; set; } = false;
    }
}
