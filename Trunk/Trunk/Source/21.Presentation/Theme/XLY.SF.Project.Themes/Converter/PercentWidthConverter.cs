using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Themes.PercentWidthConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/12 10:31:13
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 百分比宽度转换器
    /// </summary>
    public class PercentWidthConverter : IValueConverter
    {
        /// <summary>
        /// 转换百分比值
        /// </summary>
        /// <param name="value">原始宽度</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">百分比，默认50%</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double percent = 0.5d;
            if(!double.TryParse(parameter?.ToString(), out percent))
            {
                percent = 0.5d;
            }
            if (double.TryParse(value?.ToString(), out double w))
            {
                return w * percent;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
