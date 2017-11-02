using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Main.DeviceHomePage
{
    [Export(ExportKeys.DeviceHomePageViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DeviceHomePageViewModel: ViewModelBase
    {
        public DeviceHomePageViewModel()
        {

        }
    }
}
