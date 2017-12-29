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

        /// <summary>
        /// 选择了某个联系人，在右侧显示对话
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lsb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = (ListBox)sender;
            DataViewPluginArgument arg = lb.DataContext as DataViewPluginArgument;
            if (arg == null)
                return;
           
            TreeNode accout = lb.SelectedItem as TreeNode;
            if (accout == null)
            {
                return;
            }
            var views = DataViewPluginAdapter.Instance.GetView(arg.DataSource.PluginInfo.Guid, accout.Type, new DataViewConfigure() { IsDefaultGridViewVisibleWhenMultiviews = true });
            tbdetail.Items.Clear();
            foreach (var v in views)    //生成消息列表显示视图列表
            {
                v.SelectedDataChanged -= OnSelectedDataChanged;
                v.SelectedDataChanged += OnSelectedDataChanged;
                tbdetail.Items.Add(v.ToControl(new DataViewPluginArgument() { CurrentData = accout, DataSource = arg.DataSource }));
            }
            tbdetail.SelectedIndex = tbdetail.HasItems ? 0 : -1;

            OnSelectedDataChanged?.Invoke(lb.SelectedValue);
        }
    }
}
