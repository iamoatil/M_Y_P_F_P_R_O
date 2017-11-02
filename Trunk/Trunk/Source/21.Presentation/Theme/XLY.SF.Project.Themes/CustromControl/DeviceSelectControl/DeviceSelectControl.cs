using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLY.SF.Project.Themes
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
    ///     <MyNamespace:DeviceSelectControl/>
    ///
    /// </summary>
    public class DeviceSelectControl : System.Windows.Controls.Primitives.Selector
    {
        static DeviceSelectControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DeviceSelectControl), new FrameworkPropertyMetadata(typeof(DeviceSelectControl)));
        }

        #region 公共属性

        #region 最大可见的项数

        /// <summary>
        /// 最大可见的项数，默认为3
        /// </summary>
        public int MaxVisibleItemCount
        {
            get { return (int)GetValue(MaxVisibleItemCountProperty); }
            set { SetValue(MaxVisibleItemCountProperty, value); }
        }

        public static readonly DependencyProperty MaxVisibleItemCountProperty =
            DependencyProperty.Register("MaxVisibleItemCount", typeof(int), typeof(DeviceSelectControl), new PropertyMetadata(3));

        #endregion

        #region 每项的宽度
        /// <summary>
        /// 每项的宽度
        /// </summary>
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(DeviceSelectControl), new PropertyMetadata(60d));

        #endregion

        #region 容器宽度
        public double ItemContainerWidth
        {
            get { return (double)GetValue(ItemContainerWidthProperty); }
            set { SetValue(ItemContainerWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemContainerWidthProperty =
            DependencyProperty.Register("ItemContainerWidth", typeof(double), typeof(DeviceSelectControl), new PropertyMetadata(177d));

        #endregion

        #region 是否显示导航按钮


        public Visibility NavButtonVisibility
        {
            get { return (Visibility)GetValue(NavButtonVisibilityProperty); }
            set { SetValue(NavButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavButtonVisibilityProperty =
            DependencyProperty.Register("NavButtonVisibility", typeof(Visibility), typeof(DeviceSelectControl), new PropertyMetadata(Visibility.Visible));


        #endregion

        #region 自动适应大小

        /// <summary>
        /// 自动适应大小，根据内容个数自动改变大小，默认为true
        /// </summary>
        public bool AutoSize
        {
            get { return (bool)GetValue(AutoSizeProperty); }
            set { SetValue(AutoSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoSizeProperty =
            DependencyProperty.Register("AutoSize", typeof(bool), typeof(DeviceSelectControl), new PropertyMetadata(true));

        #endregion

        #endregion

        #region 私有方法

        Button _btnPre;
        Button _btnNext;
        Canvas _canvas;
        StackPanel _panel;
        Storyboard _storyboard = new Storyboard();
        int _index = 0;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _btnPre = GetTemplateChild("PART_Pre") as Button;
            _btnNext = GetTemplateChild("PART_Next") as Button;
            _canvas = GetTemplateChild("PART_Container") as Canvas;
            _panel = GetTemplateChild("PART_Panel") as StackPanel;

            _btnPre.Click += _btnPre_Click;
            _btnNext.Click += _btnNext_Click;
            _canvas.SizeChanged += _canvas_SizeChanged;

            ((INotifyCollectionChanged)this.Items).CollectionChanged += DeviceSelectControl_CollectionChanged;

            BindingItems();
            SetAutoSize();
            ScrollToItem();
            //this.Loaded += DeviceSelectControl_Loaded;
        }

        private void DeviceSelectControl_Loaded(object sender, RoutedEventArgs e)
        {
            //BindingItems();
            //SetAutoSize();
            //ScrollToItem();
        }

        private void DeviceSelectControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BindingItems();
            SetAutoSize();
            ScrollToItem();
        }

        /// <summary>
        /// 将数据列表绑定并生成子控件列表
        /// </summary>
        private void BindingItems()
        {
            _panel.Children.Clear();
            NavButtonVisibility = this.Items.Count > MaxVisibleItemCount ? Visibility.Visible : Visibility.Collapsed;
            foreach (var item in this.Items)
            {
                DeviceSelectItemControl pc = new DeviceSelectItemControl();
                _panel.Children.Add(pc);
                pc.DataContext = item;
                pc.MouseDown += OnDeviceItemClick;

                Binding binding = new Binding() { Source = this, Path = new PropertyPath("ItemWidth"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
                BindingOperations.SetBinding(pc, Canvas.WidthProperty, binding);

                Binding binding2 = new Binding() { Source = _canvas, Path = new PropertyPath("ActualHeight"), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
                BindingOperations.SetBinding(pc, Canvas.HeightProperty, binding2);
            }
        }

        /// <summary>
        /// 点击了某个设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceItemClick(object sender, MouseButtonEventArgs e)
        {
            DeviceSelectItemControl pc = sender as DeviceSelectItemControl;
            this.SelectedItem = this.SelectedValue = pc.DataContext;        //设置当前选择的项
        }

        /// <summary>
        /// 大小改变时重新滚动到当前项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollToItem();
        }

        /// <summary>
        /// 向后滑动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnNext_Click(object sender, RoutedEventArgs e)
        {
            _index++;
            ScrollToItem();
        }

        /// <summary>
        /// 向前滑动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnPre_Click(object sender, RoutedEventArgs e)
        {
            _index--;
            ScrollToItem();
        }

        /// <summary>
        /// 滚动动画
        /// </summary>
        private void ScrollToItem()
        {
            if (_index > _panel.Children.Count - MaxVisibleItemCount)
            {
                _index = _panel.Children.Count - MaxVisibleItemCount;
            }
            if (_index < 0)
            {
                _index = 0;
            }
            if (_storyboard.Children.Count > 0)
            {
                _storyboard.Stop();
                _storyboard.Children.Clear();
            }
            double pos = 0;
            for (int i = 0; i < _index; i++)
            {
                pos += (_panel.Children[i] as FrameworkElement).ActualWidth;
            }
            DoubleAnimation doubleAnimation = new DoubleAnimation(-pos, new Duration(TimeSpan.FromMilliseconds(300)));
            Storyboard.SetTarget(doubleAnimation, _panel);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
            _storyboard.Children.Add(doubleAnimation);
            _storyboard.Begin();
        }

        /// <summary>
        /// 设置自适应大小
        /// </summary>
        private void SetAutoSize()
        {
            if (!AutoSize)
                return;
            //double totalWidth = _btnNext.ActualWidth + _btnPre.ActualWidth;
            //totalWidth += Items.Count >= MaxVisibleItemCount ? MaxVisibleItemCount * ItemWidth : Items.Count * ItemWidth;
            //this.Width = totalWidth;

            ItemContainerWidth = Items.Count >= MaxVisibleItemCount ? MaxVisibleItemCount * ItemWidth : Items.Count * ItemWidth;
        }
        #endregion

    }
}
