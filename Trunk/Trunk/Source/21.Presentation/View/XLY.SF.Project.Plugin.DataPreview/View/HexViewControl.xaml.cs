using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace XLY.SF.Project.Plugin.DataPreview.View
{
    /// <summary>
    /// HexViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class HexViewControl : UserControl, IDataPreviewRelease
    {
        public HexViewControl()
        {
            InitializeComponent();
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
                            hexViewer.Open(fileName);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        public void Release()
        {
            hexViewer.FileName = null;
        }
    }
}
