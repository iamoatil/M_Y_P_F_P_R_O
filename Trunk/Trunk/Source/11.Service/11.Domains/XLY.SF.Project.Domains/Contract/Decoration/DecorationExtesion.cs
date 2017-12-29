using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：XLY.SF.Project.Domains.DecorationExtesion
* Author     ：Fhjun
* Create Date：2017/12/26 19:28:08
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据装饰属性扩展方法
    /// </summary>
    public static class DecorationExtesion
    {
        #region Public
        /// <summary>
        /// 设置对象的数据装饰属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        /// <param name="isTriggerByChild"></param>
        public static void SetValue(this object obj, DecorationProperty dp, object value)
        {
            dp.SetValue(obj, value, false);
        }

        /// <summary>
        /// 批量设置对象的数据装饰属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dp"></param>
        /// <param name="value"></param>
        public static void SetListValue(this IEnumerable<object> obj, DecorationProperty dp, object value)
        {
            dp.SetListValue(obj, value);
        }

        /// <summary>
        /// 获取对象的数据装饰属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        public static object GetValue(this object obj, DecorationProperty dp)
        {
            return dp.GetValue(obj);
        }

        /// <summary>
        /// 可勾选的数据装饰属性，支持树形结构
        /// </summary>
        public static CheckDecorationProperty CheckTreeProperty = new CheckDecorationProperty(false, null, OnTreeCheckStateChanged);
        /// <summary>
        /// 可勾选的数据装饰属性，支持单条数据
        /// </summary>
        public static CheckDecorationProperty CheckItemProperty = new CheckDecorationProperty(false, null, null);
        /// <summary>
        /// 可标记的数据装饰属性，支持单条数据
        /// </summary>
        public static BookmarkDecorationProperty BookmarkItemProperty = new BookmarkDecorationProperty(-1, null, null);
        #endregion

        #region Private

        public static void SetValueByChild(this object obj, DecorationProperty dp, object value)
        {
            dp.SetValue(obj, value, true);
        }

        /// <summary>
        /// 树形结构的勾选状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTreeCheckStateChanged(object sender, DecorationArgs e)
        {
            dynamic item = sender;
            bool? ischecked = (bool?)sender.GetValue(CheckTreeProperty);
            if (ischecked == null)
            {
                if (item.Parent != null)
                    SetValueByChild(item.Parent, CheckTreeProperty, null);
            }
            else if (ischecked == true) // 如果节点被选中
            {
                if(!e.TriggerByChild)
                {
                    var children = item.GetChildren();
                    if (children != null)
                        SetListValue(children, CheckTreeProperty, true);
                }
                
                if (item.Parent != null)
                {
                    Boolean bExistUncheckedChildren = false;
                    foreach (var dt in item.Parent.GetChildren())
                        if (GetValue(dt, CheckTreeProperty) != true)
                        {
                            bExistUncheckedChildren = true;
                            break;
                        }
                    if (bExistUncheckedChildren)
                        SetValueByChild(item.Parent, CheckTreeProperty, null);
                    else
                        SetValueByChild(item.Parent, CheckTreeProperty, true);
                }
            }
            else if (ischecked == false)   // 如果节点未选中
            {
                if (!e.TriggerByChild)
                {
                    var children = item.GetChildren();
                    if (children != null)
                        SetListValue(children, CheckTreeProperty, false);
                }
                if (item.Parent != null)
                {
                    Boolean bExistCheckedChildren = false;
                    foreach (var dt in item.Parent.GetChildren())
                        if (GetValue(dt, CheckTreeProperty) != false)
                        {
                            bExistCheckedChildren = true;
                            break;
                        }
                    if (bExistCheckedChildren)
                        SetValueByChild(item.Parent, CheckTreeProperty, null);
                    else
                        SetValueByChild(item.Parent, CheckTreeProperty, false);
                }
            }
        }
        #endregion

    }
}
