using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
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
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.Views.PictureView
{
    /// <summary>
    /// PictureView.xaml 的交互逻辑
    /// </summary>
    [Export(ExportKeys.PictureView, typeof(UcViewBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class PictureView : UcViewBase
    {
        #region 拖动标识

        /// <summary>
        /// 拖动鼠标
        /// </summary>
        private Cursor dragCursor;

        private double _scaleHeight;
        private double _scaleWidth;
        private double _resetHeight;
        private double _resetWidth;

        private Window _parentWin;

        #endregion

        public PictureView()
        {
            InitializeComponent();
            dragCursor = new Cursor(System.IO.Path.Combine(Environment.CurrentDirectory, "DINOSAUR.ani"));
            img.SizeChanged += Img_SizeChanged;
        }

        private void Img_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            img.Cursor = gd.ActualHeight < img.Height || gd.ActualWidth < img.Width ? dragCursor : Cursors.Arrow;
        }

        [Import(ExportKeys.PictureViewModel, typeof(ViewModelBase))]
        public override ViewModelBase DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var widthTmp = img.Width += e.Delta > 0 ? _scaleWidth : -_scaleWidth;
            var heightTmp = img.Height += e.Delta > 0 ? _scaleHeight : -_scaleHeight;

            img.Height = heightTmp <= _resetHeight ? _resetHeight : heightTmp;
            img.Width = widthTmp <= _resetWidth ? _resetWidth : widthTmp;
        }

        private void UcViewBase_Loaded(object sender, RoutedEventArgs e)
        {
            //判断是否超过边界
            _resetHeight = img.ActualHeight;
            _resetWidth = img.ActualWidth;
            img.Height = gd.ActualHeight < img.ActualHeight ? gd.ActualHeight : img.ActualHeight;
            img.Width = gd.ActualWidth < img.ActualWidth ? gd.ActualWidth : img.ActualWidth;
            _scaleHeight = img.Height / 10;
            _scaleWidth = img.Width / 10;
            _parentWin = Window.GetWindow(this);
        }

        private Point lastPoint;

        private void gd_MouseMove(object sender, MouseEventArgs e)
        {
            var a = e.GetPosition(gd);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var move = a - lastPoint;
                gd.ScrollToHorizontalOffset(gd.HorizontalOffset - move.X);
                gd.ScrollToVerticalOffset(gd.VerticalOffset - move.Y);
            }
            lastPoint = a;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ro.Angle += 90;
        }

        //还原
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ro.Angle = 0;
            img.Height = _resetHeight;
            img.Width = _resetWidth;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _parentWin?.DragMove();
        }
    }
}
