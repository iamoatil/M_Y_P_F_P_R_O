using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Framework.Core.Base.MessageBase.Navigation
{
    /// <summary>
    /// 导航元素，包含实际View
    /// </summary>
    internal class NavigationViewElement
    {
        internal NavigationViewElement(UcViewBase view)
        {
            View = view;
        }

        /// <summary>
        /// 界面视图
        /// </summary>
        public UcViewBase View { get; private set; }
    }
}
