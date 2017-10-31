using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.DataDisplayView.View.Controls;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataDisplayView.DefaultLayoutViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:07:41
* ==============================================================================*/

namespace XLY.SF.Project.DataDisplayView
{
    /// <summary>
    /// 手机布局视图
    /// </summary>
    [Export(PluginExportKeys.PluginKey, typeof(IPlugin))]
    public class PhoneLayoutViewPlugin : AbstractDataViewPlugin
    {
        public PhoneLayoutViewPlugin()
        {
            var p = new DataViewPluginInfo() { Guid = "432A5C38-4580-49BA-84CB-64C2BD98974A", Name = "手机布局", ViewType = new List<DataViewSupportItem>(), OrderIndex = 0, PluginType = PluginType.SpfDataView };
            p.ViewType.Add(new DataViewSupportItem() { PluginId = "微信", TypeName = "__LayOut" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataViewPluginArgument arg)
        {
            Grid grid = new Grid();
            grid.Background = System.Windows.Media.Brushes.Green;
            return grid;
        }
    }
}
