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

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataDisplayView.AbstractDataViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:05:22
* ==============================================================================*/

namespace XLY.SF.Project.DataDisplayView
{
    /// <summary>
    /// 数据展示插件接口定义
    /// </summary>
    public abstract class AbstractDataViewPlugin : IPlugin
    {
        public IPluginInfo PluginInfo { get; set; }

        /// <summary>
        /// 表示用于主布局的插件类型（比如想添加手机视图，则可以添加该类型的插件）
        /// </summary>
        public const string XLY_LAYOUT_KEY = "__LayOut";

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

        public abstract FrameworkElement GetControl(DataViewPluginArgument arg);

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
        public SPFTask Task { get; set; }
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
            CurrentData is SimpleDataSource sp ? sp.Items : 
            null;

        /// <summary>
        /// 选择了某一项的事件，此时将会更新数据预览项
        /// </summary>
        public DelgateDataViewSelectedItemChanged OnSelectedItemChanged { get; set; }
    }
}
