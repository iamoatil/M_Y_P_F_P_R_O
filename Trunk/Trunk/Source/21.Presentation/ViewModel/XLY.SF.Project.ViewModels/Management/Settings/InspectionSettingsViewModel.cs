using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.ViewDomain.MefKeys;
using ProjectExtend.Context;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.MessageAggregation;

namespace XLY.SF.Project.ViewModels.Management.Settings
{
    [Export(ExportKeys.SettingsInspectionViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class InspectionSettingsViewModel : ViewModelBase
    {
        #region Fields

        private readonly ISettings _dbService;

        #endregion

        #region Contructors

        [ImportingConstructor]
        public InspectionSettingsViewModel(ISettings dbService)
        {
            _dbService = dbService;
            Inspections = ((IRecordContext<Inspection>)dbService).Records.ToArray().Select(x => new InspectionModel(x, dbService));
        }

        #endregion

        #region Nested Type

        public class InspectionModel : NotifyPropertyBase
        {
            #region Fields

            private readonly Inspection _inspection;

            private readonly IRecordContext<Inspection> _dbService;

            #endregion

            #region Constructors

            public InspectionModel(Inspection inspeciton, IRecordContext<Inspection> dbService)
            {
                _inspection = inspeciton;
                _dbService = dbService;
            }

            #endregion

            #region Properties

            public String Category
            {
                get
                {
                    switch (SystemContext.LanguageManager.Type)
                    {
                        case LanguageType.En:
                            return _inspection.CategoryEn;
                        default:
                            return _inspection.CategoryCn;
                    }
                }
            }
        
            public Boolean IsSelect
            {
                get => _inspection.SelectedToken > 0;
                set
                {
                    _inspection.SelectedToken = value ? 1 : 0;
                    if (_dbService.Update(_inspection))
                    {
                        OnPropertyChanged();
                    }
                }
            }

            /// <summary>
            /// 此选项所处的分类路径
            /// </summary>
            public string Path
            {
                get
                {
                    return _inspection.ConfigFile;
                }
            }
            #endregion
        }

        #endregion

        #region Properties

        public Boolean EnableInspection
        {
            get
            {
                String str = _dbService.GetValue(SystemContext.EnableInspectionKey);
                if (Boolean.TryParse(str, out Boolean b))
                {
                    return b;
                }
                return false;
            }
            set
            {
                _dbService.SetValue(SystemContext.EnableInspectionKey, value.ToString());
                MessageAggregation.SendGeneralMsg(new GeneralArgs(GeneralKeys.SettingsChangedMsg));

            }
        }

        public IEnumerable<InspectionModel> Inspections { get; }

        #endregion

        #region Methods

        #endregion
    }
}
