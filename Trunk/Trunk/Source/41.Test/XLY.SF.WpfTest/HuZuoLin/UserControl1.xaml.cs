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

namespace XLY.SF.WpfTest.HuZuoLin
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();



            List<string> listFrom = new List<string>();

            listFrom.Add(@"G:\Work\SPF-PRO\Trunk\Trunk\Source\11.Service\14.Plugin\XLY.SF.Project.Plugin.Android\bin\Debug\XLY.SF.Project.Plugin.Android.dll");
            listFrom.Add(@"G:\Work\SPF-PRO\Trunk\Trunk\Source\11.Service\14.Plugin\XLY.SF.Project.Plugin.Android\bin\Debug\XLY.SF.Project.Plugin.Android.pdb");
            listFrom.Add(@"G:\Work\SPF-PRO\Trunk\Trunk\Source\11.Service\14.Plugin\XLY.SF.Project.Plugin.IOS\bin\Debug\XLY.SF.Project.Plugin.IOS.dll");
            listFrom.Add(@"G:\Work\SPF-PRO\Trunk\Trunk\Source\11.Service\14.Plugin\XLY.SF.Project.Plugin.IOS\bin\Debug\XLY.SF.Project.Plugin.IOS.pdb");

            List<string> listTo = new List<string>();
            listTo.Add(@"G:\Work\SPF-PRO\Trunk\Trunk\Source\31.Build\Debug");
            listTo.Add(@"G:\Work\SPF-PRO\Trunk\Trunk\Source\31.Build\Debug\solution\DataExtractionService");

            foreach (var from in listFrom)
            {
                foreach (var to in listTo)
                {
                    File.Copy(from, System.IO.Path.Combine(to, new FileInfo(from).Name), true);
                }
            }
        }
    }
}
