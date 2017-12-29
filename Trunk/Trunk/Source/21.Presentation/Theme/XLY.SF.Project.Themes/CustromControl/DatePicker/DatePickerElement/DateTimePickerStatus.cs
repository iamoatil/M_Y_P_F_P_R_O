using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Themes.CustromControl
{
    /// <summary>
    /// 日期控件的当前状态
    /// </summary>
    internal enum DateTimePickerStatus
    {
        /// <summary>
        /// 当前界面展示的是日期
        /// </summary>
        Day,
        /// <summary>
        /// 当前界面展示的是月份
        /// </summary>
        Month,
        /// <summary>
        /// 当前界面展示的是年
        /// </summary>
        Year
    }
}
