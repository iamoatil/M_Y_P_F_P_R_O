using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 16:23:15
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.ViewModel
{
    public abstract class UcViewBase : UserControl
    {
        public UcViewBase()
        {
            ViewID = Guid.NewGuid();
        }

        /// <summary>
        /// 绑定DataContext
        /// </summary>
        public virtual ViewModelBase DataSource
        {
            get
            {
                return this.DataContext as ViewModelBase;
            }
            set
            {
                if (value != null)
                    value.SetViewContainer(this);
                this.DataContext = value;
            }
        }

        #region 显示的标题

        /// <summary>
        /// 显示的标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(UcViewBase), new PropertyMetadata(""));

        #endregion

        #region 允许改变窗体大小

        /// <summary>
        /// 允许改变窗体大小【true：出现最大化按钮】
        /// </summary>
        public bool CanResize { get; set; }

        #endregion

        #region 是否最大化显示

        /// <summary>
        /// 是否最大化显示
        /// </summary>
        public bool IsMaxView { get; set; }

        #endregion

        #region 当前ViewID

        /// <summary>
        /// 当前ViewID
        /// </summary>
        public Guid ViewID { get; }

        #endregion
    }
}
