using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Plugin.DataView.View.Controls;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataView.DefaultGridViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:07:41
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// 默认表格视图
    /// </summary>
    public class DefaultGridViewPlugin : AbstractDataViewPlugin
    {
        public DefaultGridViewPlugin()
        {
            var p = new DataViewPluginInfo() {
                Guid = DataViewConfigure.DEFAULT_GRID_VIEW_ID,
                Name = Languagekeys.GridView,
                ViewType = new List<DataViewSupportItem>(),
                OrderIndex = 0,
                PluginType = PluginType.SpfDataView,
                Icon = "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_view_grid.png"
            };
            p.ViewType.Add(new DataViewSupportItem() { PluginId = "*", TypeName = "*" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataViewPluginArgument arg)
        {
            DefaultGridViewControl grid = new DefaultGridViewControl();
            grid.DataContext = arg; 
            grid.OnSelectedDataChanged -= OnSelectedDataChanged;
            grid.OnSelectedDataChanged += OnSelectedDataChanged;
            return grid;
        }
    }
}
