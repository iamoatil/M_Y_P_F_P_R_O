using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace XLY.SF.Project.Views.Converters
{
    class StringToImageSourceConverter : IValueConverter
    {
    

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;
            if (!string.IsNullOrEmpty(path))
            {
                path = "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/PhoneIcons/" + path;
                return new BitmapImage(new Uri(path, UriKind.Absolute));
            }
            else
            {
                return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    }
