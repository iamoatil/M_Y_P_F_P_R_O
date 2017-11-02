using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public const string XLY_LAYOUT_KEY = "__LayOut";

        public object Execute(object arg, IAsyncProgress progress)
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

        public FrameworkElement ToControl(DataViewPluginArgument arg, DelgateDataViewSelectedItemChanged e)
        {
            SelectedDataChanged += e;
            TabItem ti = new TabItem() { Header = PluginInfo.Name };
            ti.Content = GetControl(arg);
            return ti;
        }
    }

    /// <summary>
    /// 数据展示空控件当前选择项改变事件（主要用于数据预览窗口更新）
    /// </summary>
    /// <param name="data"></param>
    public delegate void DelgateDataViewSelectedItemChanged(object data);

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
    }
}
