using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.DataExtraction
{
    /// <summary>
    /// ExtractionView.xaml 的交互逻辑
    /// </summary>
    [Export("ExportKey_ExtractionView", typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ExtractionView : UcViewBase
    {
        public ExtractionView()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataSource = new ExtractionViewModel(); 
            }
        }

        internal Boolean IsSelftHost
        {
            get => ((ExtractionViewModel)DataSource).IsSelfHost;
            set=> ((ExtractionViewModel)DataSource).IsSelfHost = value;
        }
    }
}
