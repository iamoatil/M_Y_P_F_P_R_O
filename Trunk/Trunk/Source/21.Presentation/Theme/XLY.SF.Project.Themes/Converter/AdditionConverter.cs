using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XLY.SF.Project.Themes.Converter
{
    /// <summary>
    /// 加法转换器
    /// </summary>
    public class AdditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double tmp1;
            double tmp2;
            if (double.TryParse(value?.ToString(), out tmp1)
                && double.TryParse(parameter?.ToString(), out tmp2))
            {
                return tmp1 + tmp2;
            }
            return tmp1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
