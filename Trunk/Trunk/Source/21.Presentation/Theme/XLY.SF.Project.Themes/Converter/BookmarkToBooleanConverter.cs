using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Themes.BookmarkToBooleanConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/13 14:46:12
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// BookmarkToBooleanConverter
    /// </summary>
    public class BookmarkToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 将书签序号转换为bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(int.TryParse(value?.ToString(), out int bmk))
            {
                return bmk >= 0;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return -1;
            }
            if(bool.TryParse(value.ToString(), out bool bmk))
            {
                return bmk ? 0 : -1;
            }
            else
            {
                return -1;
            }
        }
    }
}
