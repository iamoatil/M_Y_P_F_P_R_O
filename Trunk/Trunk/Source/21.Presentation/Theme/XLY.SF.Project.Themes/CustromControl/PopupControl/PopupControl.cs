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
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl;assembly=XLY.SF.Project.Themes.CustromControl"
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
    ///     <MyNamespace:PopupControl/>
    ///
    /// </summary>
    public class PopupControl : CheckBox
    {
        static PopupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupControl), new FrameworkPropertyMetadata(typeof(PopupControl)));
        }

        /// <summary>
        /// 控件显示的文本
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PopupControl), new PropertyMetadata(""));



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Popup pop = GetChildObjects<Popup>(this.Content as UIElement).FirstOrDefault();
            if (pop == null)
            {
                throw new Exception("No Popup control found");
            }

            Binding bind = new Binding("IsChecked");
            bind.Mode = BindingMode.TwoWay;
            bind.Source = this;
            pop.SetBinding(Popup.IsOpenProperty, bind);
        }

        public List<T> GetChildObjects<T>(DependencyObject obj) where T : FrameworkElement
        {

            DependencyObject child = null;
            List<T> childList = new List<T>();
            Type typename = typeof(T);
            if (obj.GetType() == typeof(T))
            {
                childList.Add((T)obj);
            }
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == typename))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child));
            }
            return childList;
        }
    }
}
