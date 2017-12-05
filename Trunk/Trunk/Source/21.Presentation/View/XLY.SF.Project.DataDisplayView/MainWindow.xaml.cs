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
using XLY.SF.Framework.BaseUtility;
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
            //devicePath = @"E:\XLY\SPFData\默认案例_20171130[034313]\R7007_20171130[034315]";
            //var inspection = new ViewModel.InspectionConfig(){ Config = new List<ViewModel.Inspection>() {
            //    new ViewModel.Inspection(){ ID = 1, CategoryCn = "涉及国安", CategoryEn = "GuoAn"},
            //    new ViewModel.Inspection(){ ID = 2, CategoryCn = "涉及经济", CategoryEn = "Guojj"},
            //    new ViewModel.Inspection(){ ID = 3, CategoryCn = "涉及周期", CategoryEn = "Guozq"},
            //    new ViewModel.Inspection(){ ID = 4, CategoryCn = "涉及地方", CategoryEn = "Guodf"},
            //},
            //    DevicePath = @"E:\XLY\SPFData\默认案例_20171130[034313]\R7007_20171130[034315]"
            //};
            //devicePath = Serializer.JsonSerializerIO(inspection);
            devicePath = @"Inspection;E:\XLY\SPFData\默认案例_20171130[034313]\R7007_20171130[034315]";
            //var obj = Serializer.JsonDeserializeIO<ViewModel.InspectionConfig>(devicePath);

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
            content.Content = view;
        }
    }
}
