using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Themes.DateTimeToDateStringConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/12 10:28:46
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 日期转字符串
    /// </summary>
    public class DateTimeToDateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && DateTime.TryParse(value.ToString(), out DateTime t))
            {
                return t.ToString("yyyy-MM-dd"); //以这种格式进行分组
            }
            return "Unkown Time";    //以这种格式进行分组
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
