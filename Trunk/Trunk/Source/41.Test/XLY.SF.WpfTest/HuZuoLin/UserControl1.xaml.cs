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

namespace XLY.SF.WpfTest.HuZuoLin
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public FFFF DDDD { get; private set; }

        public UserControl1()
        {
            InitializeComponent();
            DDDD = new FFFF();
            DDDD.Name = null;
            this.DataContext = this;
        }
    }

    public class FFFF : ValidateBase
    {
        private string _name;

        [System.ComponentModel.DataAnnotations.StringLength(3,ErrorMessage ="错误提示")]
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                this._name = value;
                base.OnPropertyChanged();
            }
        }
    }
}
