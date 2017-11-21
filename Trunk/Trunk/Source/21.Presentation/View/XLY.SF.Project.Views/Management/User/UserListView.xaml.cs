using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Management.User
{
    /// <summary>
    /// UserListView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.ManagementUserListView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UserListView : UcViewBase
    {
        public UserListView()
        {
            InitializeComponent();
        }

        [Import(ExportKeys.ManagementUserListViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;
            }
        }
    }
}
