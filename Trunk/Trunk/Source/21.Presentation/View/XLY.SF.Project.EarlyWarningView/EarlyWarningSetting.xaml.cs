using System;
using System.Collections.Generic;
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

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// EarlyWarningSetting.xaml 的交互逻辑
    /// </summary>
    public partial class EarlyWarningSetting : UserControl
    {
        public EarlyWarningSetting()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            return;
            RadioButton radioButton = sender as RadioButton;
            if(radioButton != null)
            {
                EarlyWarningCollection data=radioButton.DataContext as EarlyWarningCollection;
                if(data != null)
                {
                    ((EarlyWarningViewModel)grid.DataContext).CurrentSelected = data;
                }
            }
        }
    }
}
