using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Plugin.Adapter;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.DataPreview
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            PluginAdapter.Instance.Initialization(null);

            IocManagerSingle.Instance.LoadParts(GetType().Assembly);
            var view = IocManagerSingle.Instance.GetViewPart(ExportKeys.DataPreView);

            string devicePath = StartupArgment.Instance.Get("PreviewObject", "");      //从命令行中读取传入的文件路径

            view.DataSource.LoadViewModel(devicePath);
            view.DataSource.ReceiveParameters(devicePath);
            content.Content = view;
        }
    }
}
