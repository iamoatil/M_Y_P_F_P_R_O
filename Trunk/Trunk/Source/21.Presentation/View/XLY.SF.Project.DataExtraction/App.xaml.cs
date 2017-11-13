using GalaSoft.MvvmLight.Threading;
using System.Windows;

namespace XLY.SF.Project.DataExtraction
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
