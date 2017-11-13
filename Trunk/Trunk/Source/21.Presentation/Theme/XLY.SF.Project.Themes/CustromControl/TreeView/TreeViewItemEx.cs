using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XLY.SF.Project.Themes.CustromControl
{
    public class TreeViewItemEx : TreeViewItem
    {

        public TreeViewItemEx()
        {

            this.PreviewMouseLeftButtonDown += TreeViewItem_PreviewMouseLeftButtonDown;
        }
        public static TreeViewItemEx LastTreeViewItem { get; set; }
        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var item = e.Source as TreeViewItemEx;
            if (item == LastTreeViewItem)
            {
                return;
            }
            var parent = item.Parent;
            if (LastTreeViewItem != null)
            {
                SetPatentStyle(LastTreeViewItem, 0);   //清空上个节点的所有样式
            }
            item.IsSelectedStyle = 1; //设置成选中样式
            LastTreeViewItem = item;
            bool result = parent is TreeViewItemEx;
            if (result)
            {
                SetPatentStyle((TreeViewItemEx)parent, 2);
            }
        }
        /// <summary>
        /// 设置选中节点的所有父节点为需要显示样式  
        /// </summary>
        /// <param name="item">节点</param>
        ///  <param name="SelectedStyle">需要改变的那种样式</param>
        private void SetPatentStyle(TreeViewItemEx item, int SelectedStyle)
        {

            item.IsSelectedStyle = SelectedStyle;
            bool result = item.Parent is TreeViewItemEx;
            if (result)
            {
                SetPatentStyle((TreeViewItemEx)item.Parent, SelectedStyle);
            }

        }

        public bool IsItemStyle
        {
            get { return (bool)GetValue(IsItemStyleProperty); }
            set { SetValue(IsItemStyleProperty, value); }
        }

        // 控制TreeViewItemEx的前台两种展现方式
        public static readonly DependencyProperty IsItemStyleProperty =
            DependencyProperty.Register("IsItemStyle", typeof(bool), typeof(TreeViewItemEx), new PropertyMetadata(false));



        public int IsSelectedStyle
        {
            get { return (int)GetValue(IsSelectedStyleProperty); }
            set { SetValue(IsSelectedStyleProperty, value); }
        }

        // 1.=选中样式
        // 2.=选中后，鼠标移出，选中了它的子节点
        // 0.=恢复默认样子
        public static readonly DependencyProperty IsSelectedStyleProperty =
            DependencyProperty.Register("IsSelectedStyle", typeof(int), typeof(TreeViewItemEx), new PropertyMetadata(0));
    }
}
