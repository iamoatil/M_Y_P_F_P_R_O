using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.IAOPPropertyChangedMonitor
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/3 14:12:16
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 标记的类将会监听属性变化，并提供属性变化事件。
    /// 当前主要用于数据更新后实时更新到数据库中
    /// </summary>
    public interface IAOPPropertyChangedMonitor
    {
        /// <summary>
        /// 属性改变时的事件
        /// </summary>
        event Action<object, string, object> OnPropertyValueChangedEvent;
        /// <summary>
        /// 属性值改变时调用，也可以主动调用该方法以触发事件
        /// </summary>
        /// <param name="propertyValue">该属性的新值</param>
        /// <param name="propertyName">该属性的名称，默认可以不填写</param>
        void OnPropertyValueChanged(object propertyValue, [CallerMemberName]string propertyName = null);
    }
}
