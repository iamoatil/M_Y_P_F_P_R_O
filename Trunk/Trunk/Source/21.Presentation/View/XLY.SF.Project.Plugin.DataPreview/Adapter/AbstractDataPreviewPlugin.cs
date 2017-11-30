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
* Assembly   ：	XLY.SF.Project.Plugin.DataPreview.AbstractDataPreviewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:05:22
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataPreview
{
    /// <summary>
    /// 数据预览插件接口定义
    /// </summary>
    [Plugin]
    public abstract class AbstractDataPreviewPlugin : IPlugin
    {
        public IPluginInfo PluginInfo { get; set; }

        public object Execute(object arg, IAsyncTaskProgress progress)
        {
            return GetControl(arg as DataPreviewPluginArgument);
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// 获取显示的插件控件
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public abstract FrameworkElement GetControl(DataPreviewPluginArgument arg);

        /// <summary>
        /// 将控件转换为TabItem，便于显示到界面上
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public FrameworkElement ToControl(DataPreviewPluginArgument arg)
        {
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
    /// 数据展示时传递的参数
    /// </summary>
    public class DataPreviewPluginArgument
    {
        /// <summary>
        /// 当前的数据，如果是文件，则为文件名；如果是数据对象，则为对象实例
        /// </summary>
        public object CurrentData { get; set; }

        /// <summary>
        /// 如果是数据对象，则表示当前插件的ID
        /// </summary>
        public string PluginId { get; set; }
        /// <summary>
        /// 如果是数据对象，表示当前数据的类型
        /// </summary>
        public object Type { get; set; }
    }
}
