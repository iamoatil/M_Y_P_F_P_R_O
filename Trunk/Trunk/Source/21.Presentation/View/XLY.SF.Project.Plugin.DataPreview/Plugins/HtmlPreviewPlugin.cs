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
    /// 网页文件预览视图
    /// </summary>
    public class HtmlPreviewPlugin : AbstractDataPreviewPlugin
    {
        public HtmlPreviewPlugin()
        {
            var p = new DataPreviewPluginInfo() {
                Guid = "{3B753E7E-9D37-4FD1-989B-445A092BABBA}",
                Name = Languagekeys.HtmlView,
                ViewType = new List<DataPreviewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataPreview,
            };
            p.ViewType.Add(new DataPreviewSupportItem() { PluginId = "*.html|*.Xml|*.mht|*.json" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataPreviewPluginArgument arg)
        {
            HtmlViewControl grid = new HtmlViewControl();
            grid.DataContext = arg; 
            return grid;
        }
    }
}
