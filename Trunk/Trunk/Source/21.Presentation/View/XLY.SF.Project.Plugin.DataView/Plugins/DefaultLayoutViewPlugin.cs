using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Plugin.DataView.View.Controls;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Plugin.DataView.DefaultLayoutViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:07:41
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// 默认布局视图
    /// </summary>
    public class DefaultLayoutViewPlugin : AbstractDataViewPlugin
    {
        public DefaultLayoutViewPlugin()
        {
            var p = new DataViewPluginInfo() {
                Guid = "432A5C38-4580-49BA-84CB-64C2BD98974A",
                Name = Languagekeys.DefaultLayout,
                ViewType = new List<DataViewSupportItem>(),
                OrderIndex = 0,
                PluginType = PluginType.SpfDataView
            };
            p.ViewType.Add(new DataViewSupportItem() { PluginId = "*", TypeName = DataViewConfigure.XLY_LAYOUT_KEY });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataViewPluginArgument arg)
        {
            DefaultLayoutViewControl grid = new DefaultLayoutViewControl(arg);
            return grid;
        }
    }
}
