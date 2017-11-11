using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    [Export(ExportKeys.DeviceWindowContentViewModel, typeof(ViewModelBase))]
    public class DeviceWindowContentViewModel : ViewModelBase
    {
        #region Constructors

        public DeviceWindowContentViewModel()
        {
            Content = IocManagerSingle.Instance.GetViewPart(ExportKeys.DeviceHomePageView);
        }

        #endregion

        #region Properties

        public UcViewBase Content { get; }

        #region Item

        private DeviceExtraction _item;
        public DeviceExtraction Item
        {
            get => _item;
            private set
            {
                _item = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Protected

        protected override void LoadCore(object parameters)
        {
            Item = parameters as DeviceExtraction;
            Content?.DataSource.LoadViewModel(parameters);
        }

        protected override void Closed()
        {
            MessageAggregation.SendGeneralMsg(new GeneralArgs<DeviceExtraction>(ExportKeys.DeviceWindowClosedMsg) { Parameters = Item });
        }

        #endregion

        #endregion
    }
}
