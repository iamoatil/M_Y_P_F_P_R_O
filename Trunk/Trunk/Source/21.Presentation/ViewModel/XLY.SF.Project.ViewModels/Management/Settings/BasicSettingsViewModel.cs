using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Management.Settings
{
    [Export(ExportKeys.SettingsBasicViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BasicSettingsViewModel : ViewModelBase
    {
        #region Fields

        private readonly IRecordContext<Basic> _dbService;

        #endregion

        #region Contructors

        [ImportingConstructor]
        public BasicSettingsViewModel(IRecordContext<Basic> dbService)
        {
            _dbService = dbService;
            Languages = new String[]
            {
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Settings_Chinese],
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Settings_English],
            };
            SelectPathCommand = new RelayCommand(SelectPath);
        }

        #endregion

        #region Properties

        public String Language
        {
            get => GetValue(SystemContext.LanguageKey);
            set => SetValue(SystemContext.LanguageKey, value);
        }

        public String[] Languages { get; }

        public String Path
        {
            get => GetValue(SystemContext.DefaultPathKey);
            private set
            {
                SetValue(SystemContext.DefaultPathKey, value);
                MessageAggregation.SendGeneralMsg(new GeneralArgs(GeneralKeys.SettingsChangedMsg));
                OnPropertyChanged();
            }
        }

        public Boolean EnableFilter
        {
            get
            {
                String str = GetValue(SystemContext.EnableFilterKey);
                if (Boolean.TryParse(str, out Boolean b))
                {
                    return b;
                }
                return false;
            }
            set
            {
                SetValue(SystemContext.EnableFilterKey, value.ToString());
                MessageAggregation.SendGeneralMsg(new GeneralArgs(GeneralKeys.SettingsChangedMsg));
            }
        }

        [Import(typeof(IPopupWindowService))]
        public IPopupWindowService PopupService { get; set; }

        public ICommand SelectPathCommand { get; }

        #endregion

        #region Methods

        #region Private

        private void SelectPath()
        {
            String str = PopupService.SelectFolderDialog();
            if (String.IsNullOrWhiteSpace(str)) return;
            Path = str;
        }

        private String GetValue(String key)
        {
            return ((ISettings)_dbService).GetValue(key);
        }

        private void SetValue(String key, String value)
        {
            ((ISettings)_dbService).SetValue(key, value);
        }

        #endregion

        #endregion
    }
}
