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
    /// 16进制文件预览视图
    /// </summary>
    public class HexPreviewPlugin : AbstractDataPreviewPlugin
    {
        public HexPreviewPlugin()
        {
            var p = new DataPreviewPluginInfo() {
                Guid = "{E9BE30AA-BD7A-49EE-8CAB-37A0317F2C6D}",
                Name = Languagekeys.Hex,
                ViewType = new List<DataPreviewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataPreview,
            };
            p.ViewType.Add(new DataPreviewSupportItem() { PluginId = "*.*" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataPreviewPluginArgument arg)
        {
            HexViewControl grid = new HexViewControl();
            grid.DataContext = arg; 
            return grid;
        }
    }
}
