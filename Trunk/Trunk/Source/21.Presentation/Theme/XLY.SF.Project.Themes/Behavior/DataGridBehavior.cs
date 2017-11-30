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
    /// DataGrid行双击行为
    /// </summary>
    public class DataGridBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.AddHandler(DataGridRow.MouseDoubleClickEvent, new MouseButtonEventHandler(RowDoubleClickCallback));
        }

        private void RowDoubleClickCallback(object sender, MouseButtonEventArgs e)
        {
            var feTmp = e.OriginalSource as FrameworkElement;
            var doubleClickRow = base.AssociatedObject.ItemContainerGenerator.ContainerFromItem(feTmp.DataContext);

            if (DataGridRowDoubleClickCommand != null && doubleClickRow != null)
                DataGridRowDoubleClickCommand.Execute(feTmp.DataContext);
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
    }
}
