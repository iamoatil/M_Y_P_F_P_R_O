using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;

namespace XLY.SF.Project.CameraView
{
    public class BrushConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;
            if(!File.Exists(path))
            {
                return new ImageBrush();
            }
            Bitmap bitmap = new Bitmap(path);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            bitmap.Dispose();
            ImageBrush imageBrush = new ImageBrush();
            ImageSourceConverter imageSourceConverter = new ImageSourceConverter();            
            imageBrush.ImageSource = (ImageSource)imageSourceConverter.ConvertFrom(stream);
            return imageBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
