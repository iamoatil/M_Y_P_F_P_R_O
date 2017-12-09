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
    ///     <MyNamespace:CreateNewFolderRadioButton/>
    ///
    /// </summary>
    public class CreateNewFolderRadioButton : RadioButton
    {
        private Button img_Icon;
        private ToggleButton inputStatus;

        /// <summary>
        /// 重置文件，文件夹名回调
        /// </summary>
        private Func<string, bool> ResetNameCallback;

        static CreateNewFolderRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CreateNewFolderRadioButton), new FrameworkPropertyMetadata(typeof(CreateNewFolderRadioButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.AddHandler(RadioButton.ClickEvent, new RoutedEventHandler(SelectedItemCallback));
            img_Icon = this.Template.FindName("img_Icon", this) as Button;
            inputStatus = this.Template.FindName("tb", this) as ToggleButton;
            if (img_Icon != null)
                img_Icon.MouseDoubleClick += Img_Icon_MouseDoubleClick;
            this.AddHandler(ToggleButton.KeyUpEvent, new KeyEventHandler(ResetFolderNameCallback));
        }

        private void ResetFolderNameCallback(object sender, KeyEventArgs e)
        {
            if (inputStatus.IsChecked.HasValue && inputStatus.IsChecked.Value)
            {
                if (e.Key == Key.Enter)
                {
                    var a = e.OriginalSource as TextBox;
                    if (ResetNameCallback != null && a != null)
                    {
                        if (ResetNameCallback(a.Text))
                        {
                            //修改成功
                            inputStatus.IsChecked = false;
                            ChangedItemNameCommand?.Execute(ChangedItemNameCommandParameter);
                        }
                        else
                        {
                            //还原文件夹或文件名
                            inputStatus.SetValue(ToggleButton.ContentProperty, ItemName);
                            inputStatus.IsChecked = false;
                        }
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    //还原文件夹或文件名
                    inputStatus.SetValue(ToggleButton.ContentProperty, ItemName);
                    inputStatus.IsChecked = false;
                }
            }
        }

        private void SelectedItemCallback(object sender, RoutedEventArgs e)
        {
            this.IsChecked = true;
            Command?.Execute(CommandParameter);
        }

        private void Img_Icon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoubleClickCommand?.Execute(DoubleClickCommandParameter);
        }

        #region 修改文件夹，文件名称

        /// <summary>
        /// 添加修改文件夹，文件名称回调
        /// </summary>
        /// <param name="resetNameCallback"></param>
        public void AddResetNameCallback(Func<string, bool> resetNameCallback)
        {
            ResetNameCallback = resetNameCallback;
        }

        #endregion

        #region 文件类型

        public bool IsFolder
        {
            get { return (bool)GetValue(IsFolderProperty); }
            set { SetValue(IsFolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFolderProperty =
            DependencyProperty.Register("IsFolder", typeof(bool), typeof(CreateNewFolderRadioButton), new PropertyMetadata(true));

        #endregion

        #region 文件夹名

        public string ItemName
        {
            get { return (string)GetValue(FolderNameProperty); }
            set { SetValue(FolderNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderNameProperty =
            DependencyProperty.Register("ItemName", typeof(string), typeof(CreateNewFolderRadioButton), new PropertyMetadata(null));

        #endregion

        #region Commands

        public ICommand DoubleClickCommand
        {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DoubleClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(CreateNewFolderRadioButton), new PropertyMetadata(null));

        public ICommand ChangedItemNameCommand
        {
            get { return (ICommand)GetValue(ChangedItemNameCommandProperty); }
            set { SetValue(ChangedItemNameCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangedItemNameCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangedItemNameCommandProperty =
            DependencyProperty.Register("ChangedItemNameCommand", typeof(ICommand), typeof(CreateNewFolderRadioButton), new PropertyMetadata(null));

        #endregion

        #region CommandParameters

        public object DoubleClickCommandParameter
        {
            get { return (object)GetValue(DoubleClickCommandParameterProperty); }
            set { SetValue(DoubleClickCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DoubleClickCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoubleClickCommandParameterProperty =
            DependencyProperty.Register("DoubleClickCommandParameter", typeof(object), typeof(CreateNewFolderRadioButton), new PropertyMetadata(null));
        
        public object ChangedItemNameCommandParameter
        {
            get { return (object)GetValue(ChangedItemNameParameterProperty); }
            set { SetValue(ChangedItemNameParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangedItemNameCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangedItemNameParameterProperty =
            DependencyProperty.Register("ChangedItemNameCommandParameter", typeof(object), typeof(CreateNewFolderRadioButton), new PropertyMetadata(null));
        
        #endregion
    }
}
