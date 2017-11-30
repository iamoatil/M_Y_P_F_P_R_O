using System.ComponentModel;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Management.Settings
{
    /// <summary>
    /// BasicSettingsView.xaml 的交互逻辑
    /// </summary>
    public partial class BasicSettingsView : UcViewBase
    {
        public BasicSettingsView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            DataSource = IocManagerSingle.Instance.GetPart<ViewModelBase>(ExportKeys.SettingsBasicViewModel);
        }
    }
}
