using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Themes.StringToImageConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/12 10:30:14
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 字符串属性转换为图片资源
    /// </summary>
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;    
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
