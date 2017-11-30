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
    /// 手机布局视图
    /// </summary>
    public class PhoneLayoutViewPlugin : AbstractDataViewPlugin
    {
        public PhoneLayoutViewPlugin()
        {
            var p = new DataViewPluginInfo() {
                Guid = "8EEB83BF-FF24-45F5-BFBC-EBB306A16F33",
                Name = Languagekeys.PhoneLayout,
                ViewType = new List<DataViewSupportItem>(),
                OrderIndex = 0,
                PluginType = PluginType.SpfDataView,
                State = PluginState.Disabled
            };
            p.ViewType.Add(new DataViewSupportItem() { PluginId = Languagekeys.WeChat, TypeName = DataViewConfigure.XLY_LAYOUT_KEY });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataViewPluginArgument arg)
        {
            PhoneLayoutControl grid = new PhoneLayoutControl();
            grid.DataContext = arg;
            return grid;
        }
    }
}
