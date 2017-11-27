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
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            this.DataContext = new Vm();
        }
        
    }

    public class Vm
    {
        public Vm()
        {
            listItem = new List<propertyNodeItem>();
            propertyNodeItem mainNode = new propertyNodeItem()
            {
                DisplayName = "功能菜单",
                Name = "主目录--功能菜单"
            };

            propertyNodeItem systemNode = new propertyNodeItem()
            {
                DisplayName = "系统设置",
                Name = "当前菜单--系统设置"
            };
            propertyNodeItem pwdTag = new propertyNodeItem()
            {
                DisplayName = "密码修改",
                Name = "当前选项--密码修改"
            };
            systemNode.Children.Add(pwdTag.Name, pwdTag);
            mainNode.Children.Add(systemNode.Name, systemNode);
            listItem.Add(mainNode);
        }

        public List<propertyNodeItem> listItem { get; set; }
    }

    public class propertyNodeItem
    {
        public string Icon { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public Dictionary<string,propertyNodeItem> Children { get; set; }
        public propertyNodeItem()
        {
            Children = new Dictionary<string, propertyNodeItem>();
        }
    }
   
}
