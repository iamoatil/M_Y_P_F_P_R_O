using System;
using System.Collections;
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
    ///     <MyNamespace:ExpanderControl/>
    ///
    /// </summary>
    public class ExpanderControl : ItemsControl
    {
        static ExpanderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpanderControl), new FrameworkPropertyMetadata(typeof(ExpanderControl)));
        }

        #region 标题栏数据模板

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ExpanderControl), new PropertyMetadata(null));
        #endregion

        #region 内容数据模板

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(ExpanderControl), new PropertyMetadata(null));
        #endregion

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            LoadSource();
        }

        Grid _grid;
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            //LoadSource();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _grid = GetTemplateChild("PART_Layout") as Grid;
            LoadSource();
        }

        private void LoadSource()
        {
            if(_grid == null)
            {
                return;
            }
            CheckBox firstrow = null;
            _grid.Children.Clear();
            _grid.RowDefinitions.Clear();
            foreach (var item in Items)
            {
                CheckBox header = new CheckBox();
                header.Checked += Header_Checked;
                header.Unchecked += Header_Checked;
                header.Style = FindResource("expanderHeaderStyle") as Style;
                header.ContentTemplate = HeaderTemplate;
                header.Loaded += Header_Loaded;
                ContentControl cell = new ContentControl();
                cell.Loaded += Cell_Loaded;
                cell.ContentTemplate = ContentTemplate;
                cell.HorizontalAlignment = HorizontalAlignment.Stretch;
                cell.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                cell.VerticalAlignment = VerticalAlignment.Stretch;
                cell.VerticalContentAlignment = VerticalAlignment.Stretch;
                cell.SetBinding(ContentControl.VisibilityProperty, new Binding() { Path = new PropertyPath("IsChecked"), Source = header, Converter = new BooleanToVisibilityConverter() });
                cell.SizeChanged += Cell_SizeChanged;

                //设置每项的绑定数据源
                header.DataContext = item;
                cell.DataContext = item;

                _grid.Children.Add(header);
                header.SetValue(Grid.RowProperty, AddNewRow(true));
                _grid.Children.Add(cell);
                cell.SetValue(Grid.RowProperty, AddNewRow(false));

                if (firstrow == null)
                {
                    firstrow = header;
                }
            }
            if (firstrow != null)
                firstrow.IsChecked = true;
        }

        private void Cell_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ContentControl cell = sender as ContentControl;
            cell.ApplyTemplate();
            var cp = AttachHelper.GetChildObjects<ContentPresenter>(cell, false).FirstOrDefault();
            if (cp != null)
            {
                cp.DataContext = cell.DataContext;
            }
        }

        private void Cell_Loaded(object sender, RoutedEventArgs e)
        {
            ContentControl cell = sender as ContentControl;
            cell.ApplyTemplate();
            var cp = AttachHelper.GetChildObjects<ContentPresenter>(cell, false).FirstOrDefault();
            if (cp != null)
            {
                cp.DataContext = cell.DataContext;
            }
        }

        private void Header_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox header = sender as CheckBox;
            var cp = AttachHelper.GetChildObjects<ContentPresenter>(header, false).FirstOrDefault();
            if (cp != null)
            {
                cp.DataContext = header.DataContext;
            }
        }

        private int AddNewRow(bool isAuto)
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(1, isAuto ? GridUnitType.Auto : GridUnitType.Star);
            _grid.RowDefinitions.Add(row);
            return _grid.RowDefinitions.Count - 1;
        }

        /// <summary>
        /// 某一项的展开和折叠
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Header_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox d = sender as CheckBox;
            if (d.IsChecked == true)    //将展开的控件高度设置为1*，否则设置为隐藏
            {
                int row = (int)d.GetValue(Grid.RowProperty) + 1;
                int i = 0;
                foreach (var r in _grid.RowDefinitions)
                {
                    if (i != row)
                    {
                        r.Height = new GridLength(1, GridUnitType.Auto);
                    }
                    else
                    {
                        r.Height = new GridLength(1, GridUnitType.Star);
                    }
                    i++;
                }
            }
            List<CheckBox> ls = new List<CheckBox>();
            foreach (var cb in _grid.Children)
            {
                if (cb is CheckBox c)
                {
                    ls.Add(c);
                }
            }
            if (d.IsChecked == true)
            {
                foreach (var cb in ls)   //将其他项设置为折叠
                {
                    if (cb != d)
                        cb.IsChecked = false;
                }
            }
            else if (d.IsChecked == false)  //如果全部都折叠，则默认展开第一个
            {
                if (!ls.Any(c => c.IsChecked == true))
                {
                    ls[0].IsChecked = true;
                }
            }
        }
    }
}
