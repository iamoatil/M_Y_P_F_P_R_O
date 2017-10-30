using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
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
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Main
{
    /// <summary>
    /// DeviceListView.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceListView : UserControl
    {
        public DeviceListView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            ViewModelBase vm = IocManagerSingle.Instance.GetPart<ViewModelBase>(ExportKeys.DeviceListViewModel);
            vm.LoadViewModel();
            DataContext = vm;
        }
    }
}
