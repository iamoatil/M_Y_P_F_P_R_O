using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace XLY.SF.Project.Themes.Behavior
{
    /// <summary>
    /// DataGrid行为
    /// </summary>
    public class DataGridBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.AddHandler(DataGridRow.MouseDoubleClickEvent, new MouseButtonEventHandler(RowDoubleClickCallback));
            base.AssociatedObject.AddHandler(DataGridRow.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(RowMouseLeftButtonDownCallback));
        }

        private void RowDoubleClickCallback(object sender, MouseButtonEventArgs e)
        {
            var feTmp = e.OriginalSource as FrameworkElement;
            var doubleClickRow = base.AssociatedObject.ItemContainerGenerator.ContainerFromItem(feTmp.DataContext);

            if (DataGridRowDoubleClickCommand != null && doubleClickRow != null)
                DataGridRowDoubleClickCommand.Execute(feTmp.DataContext);
        }

        private void RowMouseLeftButtonDownCallback(object sender, MouseButtonEventArgs e)
        {
            var feTmp = e.OriginalSource as FrameworkElement;
            var ClickRow = base.AssociatedObject.ItemContainerGenerator.ContainerFromItem(feTmp.DataContext);

            if (DataGridRowMouseLeftButtonDownCommand != null && ClickRow != null)
                DataGridRowMouseLeftButtonDownCommand.Execute(feTmp.DataContext);
        }

        #region 行双击命令

        public ICommand DataGridRowDoubleClickCommand
        {
            get { return (ICommand)GetValue(DataGridRowDoubleClickCommandProperty); }
            set { SetValue(DataGridRowDoubleClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataGridRowDoubleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataGridRowDoubleClickCommandProperty =
            DependencyProperty.Register("DataGridRowDoubleClickCommand", typeof(ICommand), typeof(DataGridBehavior), new PropertyMetadata(null));

        #endregion

        #region 行单击命令

        public ICommand DataGridRowMouseLeftButtonDownCommand
        {
            get { return (ICommand)GetValue(DataGridRowMouseLeftButtonDownCommandProperty); }
            set { SetValue(DataGridRowMouseLeftButtonDownCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseLeftButtonDownCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataGridRowMouseLeftButtonDownCommandProperty =
            DependencyProperty.Register("DataGridRowMouseLeftButtonDownCommand", typeof(ICommand), typeof(DataGridBehavior), new PropertyMetadata(null));

        #endregion

    }
}
