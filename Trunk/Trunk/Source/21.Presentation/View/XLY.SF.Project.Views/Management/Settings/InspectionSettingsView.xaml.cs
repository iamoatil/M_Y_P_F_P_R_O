using System.ComponentModel;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Management.Settings
{
    /// <summary>
    /// InspectionSettingsView.xaml 的交互逻辑
    /// </summary>
    public partial class InspectionSettingsView : UcViewBase
    {
        public InspectionSettingsView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            DataSource = IocManagerSingle.Instance.GetPart<ViewModelBase>(ExportKeys.SettingsInspectionViewModel);
        }
    }
}
