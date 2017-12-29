using ProjectExtend.Context;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.ViewDomain.MefKeys;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Project.ViewModels.Management.Giude
{
    [Export(ExportKeys.AboutUsViewModel, typeof(ViewModelBase))]
    public class AboutUsViewModel : ViewModelBase
    {
        private string _SecretTime;
        public string SecretTime
        {
            get { return _SecretTime; }
            set {
                _SecretTime = value;
                OnPropertyChanged();
            }
        }
        bool flag;
        private AboutUsViewModel() {

            CancellationCommand = new RelayCommand(ExecuteCancellationCommand);

        }
        [Import(typeof(IMessageBox))]
        private IMessageBox MessageBox
        {
            get;
            set;
        }
        private void ExecuteCancellationCommand()
        {
            if (!flag)
            {
                if (MessageBox.ShowDialogWarningMsg(SystemContext.LanguageManager[Languagekeys.AboutUsLanguage_AboutUs_cancellatioOfAuthorization]))
                {
                    SecretCoreDll.Cancellation();
                }
            }
        }
        protected override void InitLoad(object parameters)
        {
            base.InitLoad(parameters);
            
            var infos = SecretCoreDll.GetSentinelInfos();
            flag = infos.Any(s => s.FeatureIdList.Contains("4100"));
            if (!flag)
            {
                DateTime dt = new DateTime(2000, 1, 1);
                dt = dt.AddDays(SecretCoreDll.CheckModule("EXPIRE_DATE"));
                SecretTime = dt.ToString("yyyy-MM-dd");
            }
            else {
                SecretTime = SystemContext.LanguageManager[Languagekeys.AboutUsLanguage_AboutUs_Permanent];
            }
        }

        public ICommand CancellationCommand { get; set; }
    }
}
