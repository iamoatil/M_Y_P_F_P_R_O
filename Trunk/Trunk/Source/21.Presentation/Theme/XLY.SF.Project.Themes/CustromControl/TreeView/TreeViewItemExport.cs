using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XLY.SF.Project.Themes.CustromControl
{
  public  class TreeViewItemExport: TreeViewItem
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItemExport();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeViewItemExport;
        }

        public bool IsFirstStyle
        {
            get { return (bool)GetValue(IsFirstStyleProperty); }
            set { SetValue(IsFirstStyleProperty, value); }
        }

        // 控制TreeViewItemEx的前台两种展现方式
        public static readonly DependencyProperty IsFirstStyleProperty =
            DependencyProperty.Register("IsFirstStyle", typeof(bool), typeof(TreeViewItemExport), new PropertyMetadata(false));


        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // 控制TreeViewItemEx的前台两种展现方式
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(TreeViewItemExport), new PropertyMetadata(true));
    }
}
