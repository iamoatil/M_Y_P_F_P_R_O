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
* Assembly   ：	XLY.SF.Project.Plugin.DataView.ContactMessageViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:09:20
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// ContactMessageViewPlugin
    /// </summary>
    public class ContactMessageViewPlugin : AbstractDataViewPlugin
    {
        public ContactMessageViewPlugin()
        {
            var p = new DataViewPluginInfo() {
                Guid = "110EDD88-5019-4FC5-99FE-2D6EAC1B370D",
                Name = Languagekeys.MessageView,
                ViewType = new List<DataViewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataView
            };
            p.ViewType.Add(new DataViewSupportItem() { PluginId = "*", TypeName = "WeChatFriendShowX" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataViewPluginArgument arg)
        {
            ContactMessageUserControl ctrl = new ContactMessageUserControl();
            ctrl.DataContext = arg;
            ctrl.OnSelectedDataChanged += OnSelectedDataChanged;
            return ctrl;
        }
    }
}
