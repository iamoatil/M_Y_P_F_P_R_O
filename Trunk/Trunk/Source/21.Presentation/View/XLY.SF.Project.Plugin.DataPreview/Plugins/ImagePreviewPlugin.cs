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
    /// 图片文件预览视图
    /// </summary>
    public class ImagePreviewPlugin : AbstractDataPreviewPlugin
    {
        public ImagePreviewPlugin()
        {
            var p = new DataPreviewPluginInfo() {
                Guid = "{C05DAE79-E969-4CA3-9FB0-BD05C071BC74}",
                Name = Languagekeys.ImageView,
                ViewType = new List<DataPreviewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataPreview,
            };
            p.ViewType.Add(new DataPreviewSupportItem() { PluginId = "*.jpg|*.png|*.ico|*.bmp|*.tif|*.tga|*.gif" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataPreviewPluginArgument arg)
        {
            ImageViewControl ctrl = new ImageViewControl();
            ctrl.DataContext = arg; 
            return ctrl;
        }
    }
}
