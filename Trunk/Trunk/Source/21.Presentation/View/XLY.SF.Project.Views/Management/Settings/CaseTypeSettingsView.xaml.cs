using System.ComponentModel;
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
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            DataSource = IocManagerSingle.Instance.GetPart<ViewModelBase>(ExportKeys.SettingsCaseTypeViewModel);
        }
    }
}
