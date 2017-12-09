using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using XLY.SF.Project.Themes.CustromControl;

namespace XLY.SF.Project.Themes.Behavior
{
    public class ResetFileOrFolderNameBehavior : Behavior<CreateNewFolderRadioButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.AddResetNameCallback(ResetNameCallback);
        }

        private bool ResetNameCallback(string obj)
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(obj) && !string.IsNullOrWhiteSpace(FullPath))
            {
                if (IsFolder && Directory.Exists(FullPath))
                {
                    var parentTmp = Directory.GetParent(FullPath);
                    if (parentTmp != null)
                    {
                        try
                        {
                            Directory.Move(FullPath, Path.Combine(parentTmp.FullName, obj));
                            result = true;
                        }
                        catch
                        {

                        }
                    }
                }
                else if (!IsFolder && File.Exists(FullPath))
                {
                    var parentTmp = Directory.GetParent(FullPath);
                    if (parentTmp != null)
                    {
                        string fileFullPath = Path.Combine(parentTmp.FullName, obj);
                        try
                        {
                            File.Move(FullPath, fileFullPath);
                            result = true;
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return result;
        }

        public bool IsFolder
        {
            get { return (bool)GetValue(IsFolderProperty); }
            set { SetValue(IsFolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFolderProperty =
            DependencyProperty.Register("IsFolder", typeof(bool), typeof(ResetFileOrFolderNameBehavior), new PropertyMetadata(false));

        public string FullPath
        {
            get { return (string)GetValue(FullNameProperty); }
            set { SetValue(FullNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FullPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FullNameProperty =
            DependencyProperty.Register("FullPath", typeof(string), typeof(ResetFileOrFolderNameBehavior), new PropertyMetadata(null));
    }
}
