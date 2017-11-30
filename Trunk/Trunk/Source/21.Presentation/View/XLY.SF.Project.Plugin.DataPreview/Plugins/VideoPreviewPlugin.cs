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
    /// 视频文件预览视图
    /// </summary>
    public class VideoPreviewPlugin : AbstractDataPreviewPlugin
    {
        public VideoPreviewPlugin()
        {
            var p = new DataPreviewPluginInfo() {
                Guid = "{27C4750D-6A45-4F58-A502-0E1E5C189295}",
                Name = Languagekeys.VedioView,
                ViewType = new List<DataPreviewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataPreview,
            };
            p.ViewType.Add(new DataPreviewSupportItem() { PluginId = "*.avi|*.rmvb|*.rm|*.mp4|*.mkv|*.webM|*.3gp|*.WMV|*.MPG|*.vob|*.mov|*.flv|*.swf" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataPreviewPluginArgument arg)
        {
            MediaPlayerControl grid = new MediaPlayerControl();
            grid.IsAudioFormat = false;
            grid.DataContext = arg; 
            return grid;
        }
    }
}
