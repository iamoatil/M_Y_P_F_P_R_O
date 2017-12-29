using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Language;

namespace XLY.SF.Project.AndroidImg9008
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AssemblyHelper.Instance.Load();

            var lang = StartupArgment.Instance.Get("Language", "Cn");
            if (lang == "Cn")
            {
                LanguageManager.SwitchAll(Framework.Language.LanguageType.Cn);
            }
            else
            {
                LanguageManager.SwitchAll(Framework.Language.LanguageType.En);
            }
        }
    }
}
