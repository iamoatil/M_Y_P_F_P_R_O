using System.Collections.Generic;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.Export
{
    /// <summary>
    /// ExportData.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.ExportDataView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ExportData : UcViewBase
    {

        public ExportData()
        {
            
            InitializeComponent();
           
        }

        [Import(ExportKeys.ExportDataViewViewModel, typeof(ViewModelBase))]
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
