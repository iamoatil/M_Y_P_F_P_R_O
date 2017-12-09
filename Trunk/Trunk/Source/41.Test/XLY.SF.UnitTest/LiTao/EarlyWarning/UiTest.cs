using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Plugin.Adapter;
using XLY.SF.Project.ViewDomain.MefKeys;

/* ==============================================================================
* Description：UiTest  
* Author     ：litao
* Create Date：2017/12/8 10:22:52
* ==============================================================================*/

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class UiTest
    {
        [TestMethod]
        public void ShowUi()
        {
            return;
            Window win = new Window();
            string devicePath = StartupArgment.Instance.Get("DevicePath", "");     
            devicePath = @"Inspection;D:\XLY\SpfData\手里全部提取_20171204[111147]\H60-L01_20171204[111149]\";

            //加载插件列表
            PluginAdapter.Instance.Initialization(null, AssemblyHelper.Instance.PluginPath.ToArray());

            //IOC加载
            List<Assembly> asm = new List<Assembly>() { GetType().Assembly };
            asm.AddRange(AssemblyHelper.Instance.IocPath.Select(f => Assembly.LoadFile(f)));
            IocManagerSingle.Instance.LoadParts(asm.ToArray());

            //IOC获取主界面并导入显示

            bool isInspection = devicePath.StartsWith("Inspection;");

            var view = IocManagerSingle.Instance.GetViewPart(!isInspection ? ExportKeys.DataDisplayView : ExportKeys.AutoWarningView);
            view.DataSource.LoadViewModel(devicePath);
            view.DataSource.ReceiveParameters(devicePath);
            win.Content = view;
            win.Show();
        }
    }
}
