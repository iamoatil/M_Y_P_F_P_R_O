using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace XLY.SF.Project.Views.Converters
{
    public class SelectToForgroundConverter : IValueConverter
    {
        public static readonly Brush NonSelectBrush = new SolidColorBrush(Color.FromRgb(0x9b, 0xa5, 0xb9));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean? b = (Boolean?)value;
            if (b.HasValue && !b.Value) return NonSelectBrush;
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
