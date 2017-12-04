using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace XLY.SF.Project.Plugin.DataPreview.View
{
    /// <summary>
    /// ImageViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewControl : UserControl, IDataPreviewRelease
    {
        public ImageViewControl()
        {
            InitializeComponent();
        }

        public void Release()
        {
            img.Source = null;
        }

        private bool IsUserControl_Loaded = false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsUserControl_Loaded)
            {//只加载一次
                IsUserControl_Loaded = true;

                DataPreviewPluginArgument arg = this.DataContext as DataPreviewPluginArgument;
                if (arg != null && arg.CurrentData is string fileName)
                {
                    if (File.Exists(fileName))
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
}
