using System;
using System.Collections.Generic;
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

namespace XLY.SF.Project.Views.Management.Settings
{
    /// <summary>
    /// CaseTypeSettingsView.xaml 的交互逻辑
    /// </summary>
    public partial class CaseTypeSettingsView : UcViewBase
    {
        public CaseTypeSettingsView()
        {
            InitializeComponent();
            DataSource = IocManagerSingle.Instance.GetPart<ViewModelBase>(ExportKeys.SettingsCaseTypeViewModel);
        }
    }
}
