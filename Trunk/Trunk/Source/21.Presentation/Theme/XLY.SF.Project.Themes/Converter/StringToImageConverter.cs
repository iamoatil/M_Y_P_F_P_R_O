using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        /// <summary>
        /// 如果图片为空，则返回默认的图片
        /// </summary>
        public ImageSource DefaultImage { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DefaultImage;
            string file = value.ToString();
            if(System.IO.File.Exists(file))
            {
                return new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute));
            }
            return DefaultImage;    
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
