using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataView.HtmlViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:10:56
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// Html数据视图插件，为通过脚本编辑器自定义的视图
    /// </summary>
    [PluginContainer(PluginType.SpfDataView)]
    public class HtmlViewPlugin : AbstractDataViewPlugin
    {
        public override FrameworkElement GetControl(DataViewPluginArgument arg)
        {
            var plugin = PluginInfo as DataViewPluginInfo;
            HtmlViewControl ctrl = new HtmlViewControl(plugin.ScriptObject, arg);
            ctrl.OnSelectedDataChanged -= OnSelectedDataChanged;
            ctrl.OnSelectedDataChanged += OnSelectedDataChanged;
            return ctrl;
        }

        public override string ToString()
        {
            return $"自定义Html视图插件：{PluginInfo.Name}";
        }
    }
}
