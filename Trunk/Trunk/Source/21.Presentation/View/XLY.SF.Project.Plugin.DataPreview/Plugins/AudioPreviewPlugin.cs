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
    /// 语音文件预览视图
    /// </summary>
    public class AudioPreviewPlugin : AbstractDataPreviewPlugin
    {
        public AudioPreviewPlugin()
        {
            var p = new DataPreviewPluginInfo() {
                Guid = "{3227A747-0EEA-4A29-8318-20E2B462DC15}",
                Name = Languagekeys.AudioView,
                ViewType = new List<DataPreviewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataPreview,
            };
            p.ViewType.Add(new DataPreviewSupportItem() { PluginId = "*.mp3|*.wma|*.ape|*.flac|*.aac|*.ac3|*.mmf|*.amr|*.m4a|*.m4r|*.ogg|*.wav|*.mp2" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataPreviewPluginArgument arg)
        {
            MediaPlayerControl grid = new MediaPlayerControl();
            grid.IsAudioFormat = true;
            grid.DataContext = arg; 
            return grid;
        }
    }
}
