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
        /// 是否播放了关闭动画
        /// </summary>
        private bool _isPlayCloseStoryboard;
        /// <summary>
        /// 动画播放完毕
        /// </summary>
        private bool _isCloseStoryboardCompleted;
        /// <summary>
        /// Alt状态
        /// </summary>
        private bool AltDown;
        /// <summary>
        /// 设置大小区域
        /// </summary>
        private int _resizeZoom = 7;

        public new ResizeMode ResizeMode { get; }

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

        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if (!_isPlayCloseStoryboard)
        //    {
        //        //动画播放开始
        //        _isPlayCloseStoryboard = true;
        //        //播放动画
        //        e.Cancel = true;

        //        #region 创建关闭动画

        //        Storyboard _closeWindow = new Storyboard();

        //        DoubleAnimationUsingKeyFrames keyFrame3 = new DoubleAnimationUsingKeyFrames();
        //        EasingDoubleKeyFrame ea = new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)));
        //        BackEase be = new BackEase();
        //        be.EasingMode = EasingMode.EaseIn;
        //        ea.EasingFunction = be;
        //        keyFrame3.KeyFrames.Add(ea);
        //        Storyboard.SetTarget(keyFrame3, Win_Main);
        //        Storyboard.SetTargetProperty(keyFrame3, new PropertyPath("(UIElement.Opacity)"));
        //        _closeWindow.Children.Add(keyFrame3);

        //        DoubleAnimationUsingKeyFrames keyFrame5 = new DoubleAnimationUsingKeyFrames();
        //        ea = new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.4)));
        //        ea = new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.6)));
        //        be = new BackEase();
        //        be.EasingMode = EasingMode.EaseIn;
        //        ea.EasingFunction = be;
        //        keyFrame5.KeyFrames.Add(ea);
        //        Storyboard.SetTarget(keyFrame5, this);
        //        Storyboard.SetTargetProperty(keyFrame5, new PropertyPath("(UIElement.Opacity)"));
        //        _closeWindow.Children.Add(keyFrame5);

        //        #endregion

        //        _closeWindow.Completed += _closeWindow_Completed;
        //        _closeWindow.Begin();
        //    }
        //    else
        //    {
        //        e.Cancel = !_isCloseStoryboardCompleted;
        //    }
        //}

        void _closeWindow_Completed(object sender, EventArgs e)
        {
            this._isCloseStoryboardCompleted = true;
            this.Close();
        }

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

        #region 窗口大小改变

        private double _beforeValue;

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.ResizeMode == ResizeMode.CanResize)
            {
                var curPosition = e.GetPosition(this);
                if ((curPosition.X > this.ActualWidth - _resizeZoom && curPosition.X < this.ActualWidth) && (curPosition.Y > this.ActualHeight - _resizeZoom && curPosition.Y < this.ActualHeight))
                {
                    //右下调整
                    this.Cursor = Cursors.SizeNWSE;
                }
                else if ((curPosition.X > 0 && curPosition.X < _resizeZoom))
                {
                    this.CaptureMouse();
                    //左调整
                    this.Cursor = Cursors.SizeWE;
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        var a = curPosition.X - _beforeValue;
                        _beforeValue = curPosition.X;
                        ResizeWindowByLeftOrRight(true, a);
                    }
                }
                else if (curPosition.X > this.ActualWidth - _resizeZoom && curPosition.X < this.ActualWidth)
                {
                    this.CaptureMouse();
                    //右调整
                    this.Cursor = Cursors.SizeWE;
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        var a = curPosition.X - _beforeValue;
                        _beforeValue = curPosition.X;
                        ResizeWindowByLeftOrRight(false, a);
                    }
                }
                else if ((curPosition.Y > 0 && curPosition.Y < _resizeZoom))
                {
                    this.CaptureMouse();
                    //上调整
                    this.Cursor = Cursors.SizeNS;
                }
                else if (curPosition.Y > this.ActualHeight - _resizeZoom && curPosition.Y < this.ActualHeight)
                {
                    this.CaptureMouse();
                    //下调整
                    this.Cursor = Cursors.SizeNS;
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                    _beforeValue = 0;
                }
                System.Diagnostics.Debug.WriteLine(string.Format("{0}，{1}", curPosition.X, curPosition.Y));
            }
        }

        /// <summary>
        /// 改变窗体大小（左右）
        /// </summary>
        private void ResizeWindowByLeftOrRight(bool isLeft, double curSize)
        {
            if (isLeft)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0}，左移动", curSize));
            }
            else
                System.Diagnostics.Debug.WriteLine(string.Format("{0}，右移动", curSize));
        }

        /// <summary>
        /// 改变窗体大小（上下）
        /// </summary>
        private void ResizeWindowByTopOrDown()
        {

        }

        /// <summary>
        /// 改变窗体大小（右下）
        /// </summary>
        private void ResizeWindowByRightOrDown()
        {

        }

        private void SetWindowSize(Point curSize)
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //((UIElement)e.Source).CaptureMouse();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //((UIElement)e.Source).ReleaseMouseCapture();
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
