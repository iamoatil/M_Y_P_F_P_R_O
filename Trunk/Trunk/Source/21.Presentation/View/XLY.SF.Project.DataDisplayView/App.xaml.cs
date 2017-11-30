using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Language;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataDisplayView
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /*
         
        命令行启动需要以下参数：
        Language：界面语言，其中"Cn"为中文, "En"为英文；
        DevicePath：设备存储位置，为待展示数据的设备的文件夹路径；

        */
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AssemblyHelper.Instance.Load();

            var lang = StartupArgment.Instance.Get("Language", "Cn");
            if(lang == "Cn")
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
