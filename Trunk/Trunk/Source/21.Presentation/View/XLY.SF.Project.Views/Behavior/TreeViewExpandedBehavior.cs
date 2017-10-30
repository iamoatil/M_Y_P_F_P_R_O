using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace XLY.SF.Project.Views.Behavior
{
    public class TreeViewCommandExBehavior : Behavior<TreeView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(ExpandedItemCallback));
            base.AssociatedObject.AddHandler(TreeView.SelectedItemChangedEvent, new RoutedPropertyChangedEventHandler<object>(SelectedItemChangedCallback));
        }

        private void ExpandedItemCallback(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            if (ExpandedCommand != null && item != null && ExpandedCommand.CanExecute(item.DataContext))
            {
                ExpandedCommand.Execute(item.DataContext);
            }
        }

        private void SelectedItemChangedCallback(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectionChangedCommand != null && SelectionChangedCommand.CanExecute(e.NewValue))
            {
                SelectionChangedCommand.Execute(e.NewValue);
            }
        }

        #region Commands

        public ICommand ExpandedCommand
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("ExpandedCommand", typeof(ICommand), typeof(TreeViewCommandExBehavior), new PropertyMetadata(null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as TreeViewCommandExBehavior;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(CommandProperty, e.NewValue);
            }
        }

        public ICommand SelectionChangedCommand
        {
            get { return (ICommand)GetValue(SelectionChangedCommandProperty); }
            set { SetValue(SelectionChangedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionChangedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionChangedCommandProperty =
            DependencyProperty.Register("SelectionChangedCommand", typeof(ICommand), typeof(TreeViewCommandExBehavior), new PropertyMetadata(null, OnSelectionChangedCommandCallback));

        private static void OnSelectionChangedCommandCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as TreeViewCommandExBehavior;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(SelectionChangedCommandProperty, e.NewValue);
            }
        }

        #endregion
    }
}
