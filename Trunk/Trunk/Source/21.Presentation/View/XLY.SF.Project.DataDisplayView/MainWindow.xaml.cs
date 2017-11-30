using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.Adapter;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.DataDisplayView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string devicePath = StartupArgment.Instance.Get("DevicePath", "");      //从命令行中读取传入的设备路径

            //加载插件列表
            PluginAdapter.Instance.Initialization(null, AssemblyHelper.Instance.PluginPath.ToArray());

            //IOC加载
            List<Assembly> asm = new List<Assembly>() { GetType().Assembly };
            asm.AddRange(AssemblyHelper.Instance.IocPath.Select(f => Assembly.LoadFile(f)));
            IocManagerSingle.Instance.LoadParts(asm.ToArray());

            //IOC获取主界面并导入显示
            var view = IocManagerSingle.Instance.GetViewPart(ExportKeys.DataDisplayView);
            view.DataSource.LoadViewModel(devicePath);
            view.DataSource.ReceiveParameters(devicePath);
            content.Content = view;
        }
    }
}
