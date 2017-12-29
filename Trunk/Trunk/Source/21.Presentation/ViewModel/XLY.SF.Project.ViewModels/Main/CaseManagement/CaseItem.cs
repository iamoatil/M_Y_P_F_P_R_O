using System;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.CaseManagement;
using XLY.SF.Project.Models.Logical;
using System.Linq;

namespace XLY.SF.Project.ViewModels.Main.CaseManagement
{
    public class CaseItem : NotifyPropertyBase
    {
        #region Constructors

        public CaseItem(RecentCaseEntityModel caseInfo,Int32 index)
        {
            CaseInfo = caseInfo;
            Index = index;
            Case @case = Case.Open(caseInfo.CaseProjectFile);
            if (@case == null) return;
            DeviceCount = @case.DeviceExtractions.Count() ;
        }

        #endregion

        #region Properties

        public RecentCaseEntityModel CaseInfo { get; }

        public Int32 Index { get; }

        public Int32 DeviceCount { get; }

        #endregion
    }
}
