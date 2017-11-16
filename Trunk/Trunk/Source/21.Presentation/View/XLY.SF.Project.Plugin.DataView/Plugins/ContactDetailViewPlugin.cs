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
* Assembly   ：	XLY.SF.Project.Plugin.DataView.ContactDetailViewPlugin
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/8/28 10:08:44
* ==============================================================================*/

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// 联系人详细信息视图
    /// </summary>
    public class ContactDetailViewPlugin : AbstractDataViewPlugin
    {
        public ContactDetailViewPlugin()
        {
            DataViewPluginInfo p = new DataViewPluginInfo() {
                Guid = "CCE3101E-F90C-4C5E-B5E9-51CC58CFAA76",
                Name = "联系人视图",
                ViewType = new List<DataViewSupportItem>(),
                OrderIndex = 1,
                PluginType = PluginType.SpfDataView,
                State = PluginState.Disabled
            };
            p.ViewType.Add(new DataViewSupportItem() { PluginId = "*", TypeName = "WeChatFriendShow" });
            PluginInfo = p;
        }

        public override FrameworkElement GetControl(DataViewPluginArgument arg)
        {
            ContactDetailControl ctrl = new ContactDetailControl();
            ctrl.DataContext = arg;
            ctrl.OnSelectedDataChanged += OnSelectedDataChanged;
            return ctrl;
        }
    }
}
