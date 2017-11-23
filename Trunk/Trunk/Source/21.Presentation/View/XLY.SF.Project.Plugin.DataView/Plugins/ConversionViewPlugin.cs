using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataView.ConversionViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:10:16
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// ConversionViewPlugin
    /// </summary>
    public class ConversionViewPlugin : AbstractDataViewPlugin
    {
        public ConversionViewPlugin()
        {
            var p = new DataViewPluginInfo() {
                Guid = "03987975-D89C-48B5-86D5-ABFE44EA3E71",
                Name = Languagekeys.ConversionMode,
                ViewType = new List<DataViewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataView,
                Icon = "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_view_conversion.png"
            };
            p.ViewType.Add(new DataViewSupportItem() { PluginId = "*", TypeName = "MessageCore" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataViewPluginArgument arg)
        {
            ConversionControl ctrl = new ConversionControl();
            ctrl.DataContext = arg;
            ctrl.OnSelectedDataChanged += OnSelectedDataChanged;
            return ctrl;
        }
    }
}
