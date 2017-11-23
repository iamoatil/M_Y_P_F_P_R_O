using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
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
        }

        #endregion

        #region Properties

        public String Language
        {
            get => GetValue("language");
            set => SetValue("language", value);
        }

        public String Path
        {
            get => GetValue("path");
            set => SetValue("path", value);
        }

        public Boolean EnableFilter
        {
            get
            {
                String str = GetValue("enableFilter");
                if (Boolean.TryParse(str, out Boolean b))
                {
                    return b;
                }
                return false;
            }
            set => SetValue("enableFilter", value.ToString());
        }

        public Boolean EnableInspection
        {
            get
            {
                String str = GetValue("enableInspection");
                if (Boolean.TryParse(str, out Boolean b))
                {
                    return b;
                }
                return false;
            }
            set => SetValue("enableInspection", value.ToString());
        }

        #endregion

        #region Methods

        #region Private

        private String GetValue(String key)
        {
            Basic b = _dbService.Records.FirstOrDefault(x => x.Key == key);
            return b?.Value;
        }

        private void SetValue(String key, String value)
        {
            Basic b = _dbService.Records.FirstOrDefault(x => x.Key == key);
            if (b != null)
            {
                b.Value = value;
                _dbService.Update(b);
            }
            else
            {
                b = new Basic
                {
                    Key = key,
                    Value = value
                };
                _dbService.Add(b);
            }
        }

        #endregion

        #endregion
    }
}
