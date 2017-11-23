/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/11/23 10:18:56 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Globalization;
using System.Windows.Data;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.FileBrowingView.Converter
{
    public class FileTypeToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ss = (EnumFileType)value;
            switch (ss)
            {
                case EnumFileType.Directory:
                    return "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_filebrowing_icon1.png";
                case EnumFileType.Txt:
                    return "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_filebrowing_icon2.png";
                case EnumFileType.Image:
                    return "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_filebrowing_icon3.png";
                case EnumFileType.Voice:
                    return "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_filebrowing_icon5.png";
                case EnumFileType.Video:
                    return "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_filebrowing_icon6.png";
                case EnumFileType.Rar:
                    return "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_filebrowing_icon4.png";
                default:
                    return "pack://application:,,,/XLY.SF.Project.Themes;component/Resources/Images/data_filebrowing_icon7.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
