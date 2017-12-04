using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Plugin.Adapter;

namespace XLY.SF.Project.FileBrowingView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //加载插件列表
            PluginAdapter.Instance.Initialization(null, AssemblyHelper.Instance.PluginPath.ToArray());

            //IOC加载
            List<Assembly> asm = new List<Assembly>() { GetType().Assembly };
            asm.AddRange(AssemblyHelper.Instance.IocPath.Select(f => Assembly.LoadFile(f)));
            IocManagerSingle.Instance.LoadParts(asm.ToArray());
        }
    }
}
