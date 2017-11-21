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
    ///     <MyNamespace:TextBoxEx/>
    ///
    /// </summary>
    public class TextBoxEx : TextBox
    {
        /// <summary>
        /// 水印
        /// </summary>
        private TextBlock _tbWatermark;

        static TextBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxEx), new FrameworkPropertyMetadata(typeof(TextBoxEx)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _tbWatermark = this.Template.FindName("tb_Watermark", this) as TextBlock;
            TextChanged += TextBoxEx_TextChanged;
        }

        #region Title

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TextBoxEx), new PropertyMetadata(string.Empty));

        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(TextBoxEx), new PropertyMetadata(Brushes.Black));

        public Thickness TitlePadding
        {
            get { return (Thickness)GetValue(TitlePaddingProperty); }
            set { SetValue(TitlePaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitlePadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitlePaddingProperty =
            DependencyProperty.Register("TitlePadding", typeof(Thickness), typeof(TextBoxEx), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        #endregion

        #region Icon

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TextBoxEx), new PropertyMetadata(null));

        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(TextBoxEx), new PropertyMetadata(16d));

        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(TextBoxEx), new PropertyMetadata(16d));

        #endregion

        #region 水印

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(TextBoxEx), new PropertyMetadata(string.Empty));

        #endregion

        #region 文字改变判断是否显示水印

        private void TextBoxEx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Text))
                //显示水印
                _tbWatermark.Visibility = Visibility.Visible;
            else
                //关闭水印
                _tbWatermark.Visibility = Visibility.Hidden;
        }

        #endregion
    }
}
