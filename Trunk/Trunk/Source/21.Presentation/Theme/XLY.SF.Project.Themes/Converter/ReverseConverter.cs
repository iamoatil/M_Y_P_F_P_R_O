/* ==============================================================================
* Description：ReverseConverter  
*              1,定义了bool类型的反转类
* Author     ：litao
* Create Date：2017/11/14 16:40:12
* ==============================================================================*/


using System;
using System.Globalization;
using System.Windows.Data;

namespace XLY.SF.Project.Themes
{
    /// <summary>
    ///  用于把一个Bool类型的值反转，即是true变成false，false变成true
    /// </summary>
    public class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool actualValue = (bool)value;
            return !actualValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool actualValue = (bool)value;
            return !actualValue;
        }
    }

}
