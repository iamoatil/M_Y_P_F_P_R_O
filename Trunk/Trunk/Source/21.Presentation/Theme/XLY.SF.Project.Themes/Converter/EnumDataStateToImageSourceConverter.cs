﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Themes.EnumDataStateToImageSourceConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/9 20:11:12
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 将数据状态值（正常、删除）转换为控件的ImageSource
    /// </summary>
    public class EnumDataStateToImageSourceConverter : IValueConverter
    {
        private static object _normalImage = null;
        private static object _deleteImage = null;
        /// <summary>
        /// 将数据状态值（正常、删除）转换为控件的ImageSource
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.ToString() == "Normal" || value.ToString() == "None")
            {
                if(_normalImage == null)
                {
                    _normalImage = new BitmapImage(new Uri("pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_state_normal.png", UriKind.RelativeOrAbsolute)); 
                }
                return _normalImage;
            }
            else
            {
                if (_deleteImage == null)
                {
                    _deleteImage = new BitmapImage(new Uri("pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_state_delete.png", UriKind.RelativeOrAbsolute));
                }
                return _deleteImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
