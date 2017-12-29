using ProjectExtend.Context;
using System;
using System.Globalization;
using System.Windows.Data;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;

namespace XLY.SF.Project.Views.Extraction
{
    public class TaskStateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskState state = (TaskState)value;
            switch (state)
            {
                case TaskState.Waiting:
                    return SystemContext.LanguageManager[Languagekeys.ViewLanguage_Extraction_State_Waiting];
                case TaskState.Running:
                    return SystemContext.LanguageManager[Languagekeys.ViewLanguage_Extraction_State_Runing];
                case TaskState.Starting:
                    return SystemContext.LanguageManager[Languagekeys.ViewLanguage_Extraction_State_Starting];
                case TaskState.IsCancellationRequest:
                    return SystemContext.LanguageManager[Languagekeys.ViewLanguage_Extraction_State_IsCancellationRequest];
                case TaskState.Cancelled:
                    return SystemContext.LanguageManager[Languagekeys.ViewLanguage_Extraction_State_Cancelled];
                case TaskState.Completed:
                    return SystemContext.LanguageManager[Languagekeys.ViewLanguage_Extraction_State_Completed];
                case TaskState.Failed:
                    return SystemContext.LanguageManager[Languagekeys.ViewLanguage_Extraction_State_Failed];
                case TaskState.Idle:
                default:
                    return SystemContext.LanguageManager[Languagekeys.ViewLanguage_Extraction_State_Idle];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
