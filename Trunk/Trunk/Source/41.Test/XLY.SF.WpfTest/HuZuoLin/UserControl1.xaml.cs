using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using XLY.SF.Framework.Core.Base.ValidationBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Themes.CustromControl;

namespace XLY.SF.WpfTest.HuZuoLin
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public FFFF Time { get; set; }

        public UserControl1()
        {
            InitializeComponent();
            this.Loaded += UserControl1_Loaded;
        }

        private void UserControl1_Loaded(object sender, RoutedEventArgs e)
        {
            Time = new FFFF();
            Time.WbSource = new Uri("http://www.baidu.com");
            this.DataContext = Time;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Time.WbSource = new Uri("http://www.qq.com");
        }
    }

    public class FFFF : NotifyPropertyBase
    {

        private Uri _time;

        public Uri WbSource
        {
            get
            {
                return this._time;
            }

            set
            {
                this._time = value;
                base.OnPropertyChanged();
            }
        }
    }
}
