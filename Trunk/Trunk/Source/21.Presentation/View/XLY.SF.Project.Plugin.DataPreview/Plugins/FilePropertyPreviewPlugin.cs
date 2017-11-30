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
    /// 文件属性视图
    /// </summary>
    public class FilePropertyPreviewPlugin : AbstractDataPreviewPlugin
    {
        public FilePropertyPreviewPlugin()
        {
            var p = new DataPreviewPluginInfo() {
                Guid = "{8B3CC8DF-70C0-4076-812C-ACDD27155685}",
                Name = Languagekeys.BasicView,
                ViewType = new List<DataPreviewSupportItem>(),
                OrderIndex = 0,
                PluginType = PluginType.SpfDataPreview,
            };
            p.ViewType.Add(new DataPreviewSupportItem() { PluginId = "*.*" });
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
