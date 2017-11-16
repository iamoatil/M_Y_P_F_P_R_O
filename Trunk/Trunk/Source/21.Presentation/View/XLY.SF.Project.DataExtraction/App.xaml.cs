using GalaSoft.MvvmLight.Threading;
using System.Windows;
using XLY.SF.Project.DataExtraction.Language;

namespace XLY.SF.Project.DataExtraction
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            LanguageHelper.LanguageManager.Switch(Framework.Language.LanguageType.Cn);
            DispatcherHelper.Initialize();
        }
    }
}
