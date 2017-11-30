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
    /// Word文件预览视图
    /// </summary>
    public class WordPreviewPlugin : AbstractDataPreviewPlugin
    {
        public WordPreviewPlugin()
        {
            var p = new DataPreviewPluginInfo() {
                Guid = "{AE8F55D7-F270-4BC7-8EEB-0AC5CBD7BD7B}",
                Name = Languagekeys.OfficeView,
                ViewType = new List<DataPreviewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataPreview,
            };
            p.ViewType.Add(new DataPreviewSupportItem() { PluginId = "*.doc|*.docx|*.xls|*.xlsx|*.ppt|*.pptx|*.pdf" });
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
