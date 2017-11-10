using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Themes.BooleanToCollapseConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/10/24 20:16:32
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 将bool值转换为控件的Visibility，如果parameter=true，表示显示方式相反
    /// </summary>
    public class BooleanToCollapseConverter : IValueConverter
    {
        /// <summary>
        /// 将bool值转换为控件的Visibility，如果parameter=true，表示显示方式相反
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invert = false;

            if (parameter != null)
            {
                invert = Boolean.Parse(parameter.ToString());
            }

            var booleanValue = (bool)value;

            return ((booleanValue && !invert) || (!booleanValue && invert))
              ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

   
}
