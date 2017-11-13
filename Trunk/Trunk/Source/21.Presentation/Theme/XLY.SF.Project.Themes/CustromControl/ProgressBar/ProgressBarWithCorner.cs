using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

/* ==============================================================================
* Description：ProgressBar  
* Author     ：litao
* Create Date：2017/11/13 14:31:46
* ==============================================================================*/

namespace XLY.SF.Project.Themes.CustromControl
{
    public class ProgressBarWithCorner: ProgressBar
    {
        static ProgressBarWithCorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressBarWithCorner), new FrameworkPropertyMetadata(typeof(ProgressBarWithCorner)));
        }

        /// <summary>
        ///  圆角的大小
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ProgressBarWithCorner), new PropertyMetadata(new CornerRadius()));

        /// <summary>
        /// 背景色的透明度
        /// </summary>
        public double BackgroundOpacity
        {
            get { return (double)GetValue(BackgroundOpacityProperty); }
            set { SetValue(BackgroundOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundOpacityProperty =
            DependencyProperty.Register("BackgroundOpacity", typeof(double), typeof(ProgressBarWithCorner), new PropertyMetadata(1.0));

    }
}
