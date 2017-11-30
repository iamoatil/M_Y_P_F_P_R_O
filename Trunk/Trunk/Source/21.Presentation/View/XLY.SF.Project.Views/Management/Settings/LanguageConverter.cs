using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XLY.SF.Project.Views.Management.Settings
{
    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String str = value as String;
            if (String.IsNullOrWhiteSpace(str)) return "中文";
            switch (str)
            {
                case "en":
                    return "英文";
                default:
                    return "中文";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String str = (String)value;
            switch (str)
            {
                case "英文":
                    return "en";
                default:
                    return "cn";
            }
        }
    }
}
