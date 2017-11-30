using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Language;

namespace XLY.SF.Project.DataPreview
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /*

       命令行启动需要以下参数：
       Language：界面语言，其中"Cn"为中文, "En"为英文；
       PreviewObject：浏览对象，可以为文件全路径；

       */
        private void Application_Startup(object sender, StartupEventArgs e)
        {
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
