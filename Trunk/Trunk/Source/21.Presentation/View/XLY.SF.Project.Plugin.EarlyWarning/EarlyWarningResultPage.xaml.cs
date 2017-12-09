using System.ComponentModel.Composition;
using System.Windows.Controls;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.DataDisplayView;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// EarlyWarningMainPage.xaml 的交互逻辑
    /// </summary>
   //[Export(ExportKeys.AutoWarningView, typeof(UcViewBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    public partial class EarlyWarningResultPage : UcViewBase
    {
        public EarlyWarningResultPage()
        {
            InitializeComponent();          
        }

        /// <summary>
        ///通过导入获取setting对象。其用于读取数据库中的数据
        /// </summary>
        [Import(typeof(IRecordContext<Inspection>))]
        public IRecordContext<Inspection> Setting { get; set; }
        
        private void UcViewBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            EarlyWarningPluginAdapter adapter = new EarlyWarningPluginAdapter();
            adapter.Initialize(Setting);
            adapter.Detect(@"D:\XLY\SpfData\手里全部提取_20171204[111147]\H60-L01_20171204[111149]\");
        }
    }
}
