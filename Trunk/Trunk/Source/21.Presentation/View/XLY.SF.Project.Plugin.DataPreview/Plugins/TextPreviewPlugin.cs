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
    /// 文本文件预览视图
    /// </summary>
    public class TextPreviewPlugin : AbstractDataPreviewPlugin
    {
        public TextPreviewPlugin()
        {
            var p = new DataPreviewPluginInfo() {
                Guid = "{608A2446-D033-4FB7-9EB0-32147E754330}",
                Name = Languagekeys.TextView,
                ViewType = new List<DataPreviewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataPreview,
            };
            p.ViewType.Add(new DataPreviewSupportItem() { PluginId = "*.txt|*.ini|*.log" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataPreviewPluginArgument arg)
        {
            TextViewControl grid = new TextViewControl();
            grid.DataContext = arg; 
            return grid;
        }
    }
}
