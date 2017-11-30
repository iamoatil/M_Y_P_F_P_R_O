using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLY.SF.Project.Plugin.DataPreview.View
{
    /// <summary>
    /// ImageViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewControl : UserControl
    {
        public ImageViewControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataPreviewPluginArgument arg = this.DataContext as DataPreviewPluginArgument;
            if(arg != null && arg.CurrentData is string fileName)
            {
                if(File.Exists(fileName))
                {
                    try
                    {
                        img.Source = new BitmapImage(new Uri(fileName));
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}
