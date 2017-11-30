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
            Inspections = LoadData((IRecordContext<Inspection>)dbService);
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

            public String Name
            {
                get
                {
                    switch (SystemContext.LanguageManager.Type)
                    {
                        case LanguageType.En:
                            return _inspection.NameEn;
                        default:
                            return _inspection.NameCn;
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
                        //通知父节点子节点的选中属性改变
                        Parent.IsSelect = null;
                    }
                }
            }

            internal InspectionGroup Parent { get; set; }

            #endregion
        }

        public class InspectionGroup : NotifyPropertyBase
        {
            #region Fields

            private readonly MsgAggregation _messageAggregation;

            #endregion

            #region Constructors

            public InspectionGroup(String category, IEnumerable<InspectionModel> items,MsgAggregation messageAggregation)
            {
                _messageAggregation = messageAggregation;
                Category = category;
                Items = items ?? new InspectionModel[0];
                foreach (var item in Items)
                {
                    item.Parent = this;
                }
            }

            #endregion

            #region Properties

            public IEnumerable<InspectionModel> Items { get; }

            public String Category { get; }

            public Boolean? IsSelect
            {
                get
                {
                    if (Items.All(x => x.IsSelect))
                    {
                        return true;
                    }
                    else if (Items.All(x => !x.IsSelect))
                    {
                        return false;
                    }
                    else
                    {
                        return null;
                    }
                }
                set
                {
                    //通过界面改变改属性的值时，不可能为null
                    if (value.HasValue)
                    {
                        foreach (var item in Items)
                        {
                            item.IsSelect = value.Value;
                        }
                    }
                    OnPropertyChanged();
                    _messageAggregation.SendGeneralMsg(new GeneralArgs(GeneralKeys.SettingsChangedMsg));
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

        public InspectionGroup[] Inspections { get; }

        #endregion

        #region Methods

        #region Private

        private InspectionGroup[] LoadData(IRecordContext<Inspection> inspectionService)
        {
            IEnumerable<InspectionModel> models = inspectionService.Records.ToArray().Select(x => new InspectionModel(x, inspectionService));
            var temp = models.GroupBy(x => x.Category).ToDictionary(x => x.Key, y => y.ToArray());
            InspectionGroup[] groups = new InspectionGroup[temp.Count];
            Int32 i = 0;
            foreach (String category in temp.Keys)
            {
                groups[i++] = new InspectionGroup(category, temp[category], MessageAggregation);
            }
            return groups;
        }

        #endregion

        #endregion
    }
}
