using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLY.SF.Project.Themes.CustromControl
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl.TabControl"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl.TabControl;assembly=XLY.SF.Project.Themes.CustromControl.TabControl"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:TabControlWithButton/>
    ///
    /// </summary>
    public class TabControlWithButton : TabControl
    {
        static TabControlWithButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControlWithButton), new FrameworkPropertyMetadata(typeof(TabControlWithButton)));
        }


        #region 自定义元素列表


        public ObservableCollection<FrameworkElement> CustomButtonGroup { get; set; } = new ObservableCollection<FrameworkElement>();

        //{
        //    get { return (ObservableCollection<FrameworkElement>)GetValue(CustomButtonGroupProperty); }
        //    set { SetValue(CustomButtonGroupProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for CustomButtonGroup.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CustomButtonGroupProperty =
        //    DependencyProperty.Register("CustomButtonGroup", typeof(ObservableCollection<FrameworkElement>), typeof(TabControlWithButton), new PropertyMetadata(new ObservableCollection<FrameworkElement>()));
        #endregion



        #region 自定义元素对齐方式
        public HorizontalAlignment CustomButtonAlignment
        {
            get { return (HorizontalAlignment)GetValue(CustomButtonAlignmentProperty); }
            set { SetValue(CustomButtonAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomButtonAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomButtonAlignmentProperty =
            DependencyProperty.Register("CustomButtonAlignment", typeof(HorizontalAlignment), typeof(TabControlWithButton), new PropertyMetadata(HorizontalAlignment.Right));

        #endregion

        #region 在只有一个TabItem时是否隐藏标题栏

        public bool IsHideHeaderWhenEmpty
        {
            get { return (bool)GetValue(IsHideHeaderWhenEmptyProperty); }
            set { SetValue(IsHideHeaderWhenEmptyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHideTabItemWhenEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHideHeaderWhenEmptyProperty =
            DependencyProperty.Register("IsHideHeaderWhenEmpty", typeof(bool), typeof(TabControlWithButton), new PropertyMetadata(true));


        #endregion

        private DockPanel _headBar;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            try
            {
                _headBar = GetTemplateChild("PART_HeaderBar") as DockPanel;
                _headBar.Visibility = IsHideHeaderWhenEmpty && Items.Count <= 1 ? Visibility.Collapsed : Visibility.Visible;
                this.SelectionChanged += TabControlWithButton_SelectionChanged;

                var panel = GetTemplateChild("PART_CustomBtns") as StackPanel;
                foreach (FrameworkElement item in CustomButtonGroup)
                {
                    item.VerticalAlignment = VerticalAlignment.Center;
                    panel.Children.Add(item);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void TabControlWithButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _headBar.Visibility = IsHideHeaderWhenEmpty && Items.Count <= 1 ? Visibility.Collapsed : Visibility.Visible;
        }

    }
}
