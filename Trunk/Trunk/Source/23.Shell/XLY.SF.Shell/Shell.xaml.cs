using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.Themes;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Shell
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    public partial class Shell : WindowEx, INotifyPropertyChanged
    {
        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性更新（不用给propertyName赋值）
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Propertes
        
        /// <summary>
        /// Alt状态
        /// </summary>
        private bool AltDown;

        #endregion

        /// <summary>
        /// 界面承载器
        /// </summary>
        public Shell()
        {
            InitializeComponent();
        }

        #region 展示内容元素

        /// <summary>
        /// 展示的内容
        /// </summary>
        public new UcViewBase Content
        {
            get
            {
                return base.Content as UcViewBase;
            }
            set
            {
                base.Content = value;
            }
        }

        #endregion

        #region 窗口关闭

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                AltDown = true;
            }
            else if (e.SystemKey == Key.F4 && AltDown)
            {
                e.Handled = true;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                AltDown = false;
            }
        }

        #endregion

        #region 窗口操作

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //最大化
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var a = sender as ToggleButton;
            this.WindowState = a.IsChecked.HasValue && a.IsChecked.Value ? WindowState.Maximized : WindowState.Normal;
        }

        //最小化
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //关闭窗体【只针对弹出窗】
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #endregion

        #region 最大化最小化标识

        /// <summary>
        /// 是否显示最大化和还原
        /// </summary>
        public bool ShowMaxsize { get; set; }

        /// <summary>
        /// 是否显示最小化
        /// </summary>
        public bool ShowMinsize { get; set; }

        #endregion

        #region 普通窗口和返回窗口
        
        /// <summary>
        /// 是否为返回窗口
        /// </summary>
        public bool IsBackWindow { get; set; }

        public bool CollapsedCloseBtn { get { return !IsBackWindow; } }

        #endregion

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MsgAggregation.Instance.SendGeneralMsg(new GeneralArgs<object>(ExportKeys.DeviceWindowClosedMsg)
            {
                Parameters = this.Tag
            });
        }
    }
}
