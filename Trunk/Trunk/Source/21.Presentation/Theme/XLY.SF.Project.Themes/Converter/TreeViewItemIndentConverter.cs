using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Themes.Converter.TreeViewItemIndentConverter
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/26 15:39:57
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 树节点间距转换器
    /// </summary>
    public class TreeViewItemIndentConverter : IValueConverter
    {
        public double Indent { get; set; } = 10;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (null == item)
                return new Thickness(0);
            //double indent;
            //if (parameter == null || !double.TryParse(parameter.ToString(), out indent))
            //{
            //    indent = 10;
            //}
            return new Thickness(Indent * AttachHelper.GetTreeNodeLevel(item), 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 树节点间距反向转换器
    /// </summary>
    public class TreeViewItemIndentConverter2 : IValueConverter
    {
        public double Indent { get; set; } = 10;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (null == item)
                return new Thickness(0);
            //double indent;
            //if (parameter == null || !double.TryParse(parameter.ToString(), out indent))
            //{
            //    indent = 10;
            //}
            return new Thickness(-Indent * AttachHelper.GetTreeNodeLevel(item), 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
