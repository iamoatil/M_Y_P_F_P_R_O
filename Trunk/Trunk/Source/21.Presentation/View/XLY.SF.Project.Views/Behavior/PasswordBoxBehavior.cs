using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace XLY.SF.Project.Views.Behavior
{
  public  class PasswordBoxBehavior:Behavior<PasswordBox>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }

        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var PassWord = e.Source as PasswordBox;
            if (PassWord != null)
            {
                PasswordString = PassWord.Password;
            }
        }

        public string PasswordString
        {
            get { return (string)GetValue(PasswordStringProperty); }
            set { SetValue(PasswordStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordStringProperty =
            DependencyProperty.Register("PasswordString", typeof(string), typeof(PasswordBoxBehavior), new PropertyMetadata(string.Empty));

        #region PropertyChangedCallback

        //private static void PasswordStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    PasswordBoxBehavior sender = d as PasswordBoxBehavior;
        //    if (sender != null)
        //    {
        //        if (e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString())) {
        //            sender.PasswordString = e.NewValue.ToString();
        //        }
        //    }
        //}

        #endregion
    }
}
