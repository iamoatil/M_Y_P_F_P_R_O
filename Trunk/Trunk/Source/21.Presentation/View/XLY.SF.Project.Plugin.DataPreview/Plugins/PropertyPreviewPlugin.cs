using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.DataPreview.View;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataPreview.DefaultGridViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:07:41
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataPreview
{
    /// <summary>
    /// 默认属性视图
    /// </summary>
    public class PropertyPreviewPlugin : AbstractDataPreviewPlugin
    {
        public PropertyPreviewPlugin()
        {
            var p = new DataPreviewPluginInfo() {
                Guid = "{DF62D94B-3F52-49B6-B7DF-5245F24C42B6}",
                Name = Languagekeys.BasicView,
                ViewType = new List<DataPreviewSupportItem>(),
                OrderIndex = 0,
                PluginType = PluginType.SpfDataPreview,
            };
            p.ViewType.Add(new DataPreviewSupportItem() { PluginId = "*", TypeName = "*" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataPreviewPluginArgument arg)
        {
            PropertyViewControl grid = new PropertyViewControl();
            grid.DataContext = arg; 
            return grid;
        }
    }
}
