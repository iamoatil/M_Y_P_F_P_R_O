using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main.DeviceMain
{
    [Export(ExportKeys.ExtractionViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceExtractionViewModel : ViewModelBase
    {
        #region Constructors

        public DeviceExtractionViewModel()
        {
            Content = IocManagerSingle.Instance.GetPart<UcViewBase>("XLY.SF.Project.DataExtraction.ExtractionView");
        }

        #endregion

        #region Properties

        public UcViewBase Content { get; }

        #endregion

        #region Methods

        #region Protected

        protected override void InitLoad(object parameters)
        {
            Content.DataSource.LoadViewModel(parameters);
        }

        #endregion

        #endregion
    }
}
