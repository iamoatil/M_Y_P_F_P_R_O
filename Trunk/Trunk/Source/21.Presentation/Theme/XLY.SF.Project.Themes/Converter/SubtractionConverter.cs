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
    /// 左移转换器
    /// </summary>
    public class LeftOffsetConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result;
            int paramTmp;
            if (int.TryParse(value?.ToString(), out result) &&
                int.TryParse(parameter?.ToString(), out paramTmp))
            {
                result = result - result*2 - paramTmp;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
