using System;
using System.Collections.Generic;
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
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes;assembly=XLY.SF.Project.Themes"
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
    ///     <MyNamespace:MenuButton/>
    ///
    /// </summary>
    public class MenuButton : ComboBox
    {
        private Button _btnOpen;
        private Popup _poMenu;

        static MenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuButton), new FrameworkPropertyMetadata(typeof(MenuButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SelectionChanged += MenuButton_SelectionChanged;
            _btnOpen = this.Template.FindName("btn_Open", this) as Button;
            _poMenu = this.Template.FindName("PART_Popup", this) as Popup;
            _btnOpen.Click += _btnOpen_Click;
        }

        private void _btnOpen_Click(object sender, RoutedEventArgs e)
        {
            _poMenu.IsOpen = false;
            ClickCommand?.Execute(null);
        }

        //由于目前用的是ComboBox来做的菜单，所以需要每次选择后将SelectedIndex重置
        private void MenuButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedIndex = -1;
        }

        #region 按钮内容

        public string BtnTest
        {
            get { return (string)GetValue(BtnTestProperty); }
            set { SetValue(BtnTestProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BtnTest.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BtnTestProperty =
            DependencyProperty.Register("BtnTest", typeof(string), typeof(MenuButton), new PropertyMetadata(""));

        #endregion

        #region 点击命令

        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(MenuButton), new PropertyMetadata(null));

        #endregion
    }
}
