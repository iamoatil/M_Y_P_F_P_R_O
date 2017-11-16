using System;
using System.Collections.Generic;
using System.Globalization;
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
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// ContactMessageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ContactMessageUserControl : UserControl
    {
        public ContactMessageUserControl()
        {
            InitializeComponent();
        }

        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(lsb1.Items.Count > 0)
            {
                lsb1.SelectedValue = lsb1.Items[0]; 
            }
        }

        private void lsb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataViewPluginArgument arg = lsb1.DataContext as DataViewPluginArgument;
            if (arg == null)
                return;
            WeChatFriendShow accout = lsb1.SelectedItem as WeChatFriendShow;
            TreeNode nodes = arg.CurrentData as TreeNode;
            if (accout == null || nodes == null)
            {
                return ;
            }
            var selNode = nodes.TreeNodes.FirstOrDefault(t => t.Text == accout.Nick);
            var views = DataViewPluginAdapter.Instance.GetView(arg.DataSource.PluginInfo.Guid, selNode.Type, new DataViewConfigure() { IsDefaultGridViewVisibleWhenMultiviews = true });
            tbdetail.Items.Clear();
            foreach (var v in views)
            {
                v.SelectedDataChanged += OnSelectedDataChanged;
                tbdetail.Items.Add(v.ToControl(new DataViewPluginArgument() { CurrentData = selNode, DataSource = arg.DataSource }));
            }
            tbdetail.SelectedIndex = tbdetail.HasItems ? 0 : -1;

            OnSelectedDataChanged?.Invoke(lsb1.SelectedValue);
        }
    }

    public class TreeNodeSelectMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            WeChatFriendShow accout = values[0] as WeChatFriendShow;
            TreeNode nodes = values[1] as TreeNode;
            if(accout == null || nodes == null)
            {
                return null;
            }
            return nodes.TreeNodes.FirstOrDefault(t => t.Text == accout.Nick);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
