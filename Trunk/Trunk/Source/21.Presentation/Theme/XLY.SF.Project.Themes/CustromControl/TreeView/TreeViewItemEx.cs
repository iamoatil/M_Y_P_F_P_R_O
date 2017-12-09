using Microsoft.Expression.Media.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XLY.SF.Project.Themes.CustromControl
{
    public class TreeViewItemEx : TreeViewItem
    {

        private static List<TreeViewItemEx> list = new List<TreeViewItemEx>();
        private static ColorToneEffect _ct;

        public static ColorToneEffect ct
        {
            get
            {
                if (_ct == null)
                {
                    _ct = new ColorToneEffect() { DarkColor = Colors.White, LightColor = Colors.White, ToneAmount = 1 };
                }
                return _ct;
            }
            set
            {
                _ct = value;
            }
        }
        public TreeViewItemEx()
        {
            this.Selected += TreeViewItemEx_Selected;
            this.Expanded += TreeViewItemEx_Expanded;
        }

        private void TreeViewItemEx_Expanded(object sender, RoutedEventArgs e)
        {
            if(!IsExpanded)
            {
                IsExpanded = true;
            }
        }

        private void TreeViewItemEx_Selected(object sender, RoutedEventArgs e)
        {
            if (this.IsSelected)
            {
                this.Tag = true;
                //设置当前选中样式
                this.Foreground = new SolidColorBrush(Colors.White);
                UpdateSelectStyle(this, ct, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#19000000")), new SolidColorBrush(Color.FromRgb(55, 155, 230)));
                //取消处理上一轮样式
                foreach (var item in list)
                {
                    if (item != this)
                    {
                        item.Foreground = new SolidColorBrush(Color.FromRgb(155, 165, 185));
                        if (item.Tag is bool == true)
                        {
                            UpdateSelectStyle(item, null, new SolidColorBrush(Colors.Transparent), new SolidColorBrush(Colors.Transparent));
                        }
                        else
                        {
                            UpdateSelectBaseStyle(item, null);
                        }
                        this.Tag = false;
                    }
                }

                list.Clear();

                this.IsExpanded = true;
            }
            else
            {
                this.Foreground = new SolidColorBrush(Colors.White);
                UpdateSelectBaseStyle(this, ct);
            }

            list.Add(this);
        }

        /// <summary>
        /// 设置当前节点的选中样式
        /// </summary>
        private void UpdateSelectStyle(TreeViewItemEx ex, ColorToneEffect ct, SolidColorBrush backColor, SolidColorBrush fillColor)
        {
            var grid = ex.Template.FindName("gr", ex) as Grid;
            var rec = ex.Template.FindName("moveBackGround", ex) as Rectangle;
            var img = ex.Template.FindName("img", ex) as Image;
            var img1 = ex.Template.FindName("img1", ex) as Image;
            var img2 = ex.Template.FindName("img2", ex) as Image;
            if (grid != null)
            {
                grid.Background = backColor;
            }
            if (rec != null)
            {
                rec.Fill = fillColor;
            }
            if (img != null)
            {
                img.Effect = ct;
            }
            if (img1 != null)
            {
                img1.Effect = ct;
            }
            if (img2 != null)
            {
                img2.Effect = ct;
            }
        }

        /// <summary>
        /// 设置当前节点的所有父节点样式
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="ct"></param>
        private void UpdateSelectBaseStyle(TreeViewItemEx ex, ColorToneEffect ct)
        {
            var img = ex.Template.FindName("img", ex) as Image;
            var img1 = ex.Template.FindName("img1", ex) as Image;
            var img2 = ex.Template.FindName("img2", ex) as Image;
            if (img != null)
            {
                img.Effect = ct;
            }
            if (img1 != null)
            {
                img1.Effect = ct;
            }
            if (img2 != null)
            {
                img2.Effect = ct;
            }

        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItemEx();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeViewItemEx;
        }

        public bool IsItemStyle
        {
            get { return (bool)GetValue(IsItemStyleProperty); }
            set { SetValue(IsItemStyleProperty, value); }
        }

        // 控制TreeViewItemEx的前台两种展现方式
        public static readonly DependencyProperty IsItemStyleProperty =
            DependencyProperty.Register("IsItemStyle", typeof(bool), typeof(TreeViewItemEx), new PropertyMetadata(false));

    }
}
