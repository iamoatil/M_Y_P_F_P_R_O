using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Themes.AttachHelper
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/26 14:59:02
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 附加属性管理等功能
    /// </summary>
    public class AttachHelper
    {
        #region 树控件附加属性

        #region 是否带有复选框

        public static bool GetIsShowCheckBox(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowCheckBoxProperty);
        }

        public static void SetIsShowCheckBox(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowCheckBoxProperty, value);
        }

        /// <summary>
        /// 是否带有复选框
        /// </summary>
        public static readonly DependencyProperty IsShowCheckBoxProperty =
            DependencyProperty.RegisterAttached("IsShowCheckBox", typeof(bool), typeof(AttachHelper), new PropertyMetadata(false));
        #endregion

        #region 是否隐藏未勾选的子节点

        public static bool GetIsHideUnCheckedNode(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHideUnCheckedNodeProperty);
        }

        public static void SetIsHideUnCheckedNode(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHideUnCheckedNodeProperty, value);
        }

        /// <summary>
        /// 是否隐藏未勾选的子节点
        /// </summary>
        public static readonly DependencyProperty IsHideUnCheckedNodeProperty =
            DependencyProperty.RegisterAttached("IsHideUnCheckedNode", typeof(bool), typeof(AttachHelper), new PropertyMetadata(false));

        #endregion

        #region 是否隐藏IsHideChildren=false的子节点

        public static bool GetIsHideChildren(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHideChildrenProperty);
        }

        public static void SetIsHideChildren(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHideChildrenProperty, value);
        }

        /// <summary>
        /// 是否隐藏IsHideChildren=false的子节点
        /// </summary>
        public static readonly DependencyProperty IsHideChildrenProperty =
            DependencyProperty.RegisterAttached("IsHideChildren", typeof(bool), typeof(AttachHelper), new PropertyMetadata(false));

        #endregion

        #region 是否在选择节点时显示句柄样式

        public static bool GetIsShowSelectedGripper(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowSelectedGripperProperty);
        }

        public static void SetIsShowSelectedGripper(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowSelectedGripperProperty, value);
        }

        /// <summary>
        /// 是否在选择节点时显示句柄样式
        /// </summary>
        public static readonly DependencyProperty IsShowSelectedGripperProperty =
            DependencyProperty.RegisterAttached("IsShowSelectedGripper", typeof(bool), typeof(AttachHelper), new PropertyMetadata(true));

        #endregion

        #endregion

        #region 表格控件附加属性
        #region 是否支持滚动翻页，为true则在滚动条滚动到顶部或底部时读取下一页数据

        public static bool GetIsScrollPage(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsScrollPageProperty);
        }

        public static void SetIsScrollPage(DependencyObject obj, bool value)
        {
            obj.SetValue(IsScrollPageProperty, value);
        }

        /// <summary>
        /// 是否支持滚动翻页
        /// </summary>
        public static readonly DependencyProperty IsScrollPageProperty =
            DependencyProperty.RegisterAttached("IsScrollPage", typeof(bool), typeof(AttachHelper), new PropertyMetadata(false));

        #endregion

        #endregion

        #region 辅助方法
        /// <summary>
        /// 获取某个树节点的level
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetTreeNodeLevel(TreeViewItem item)
        {
            TreeViewItem elem = item;
            while (GetParentNode(elem) != null)
            {
                var tvi = GetParentNode(elem);
                if (null != tvi)
                    return GetTreeNodeLevel(tvi) + 1;
                elem = GetParentNode(elem);
            }
            return 0;
        }

        private static TreeViewItem GetParentNode(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem))
            {
                if (parent == null) return null;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as TreeViewItem;
        }

        /// <summary>
        /// 获取控件下的所有子控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="isAllLevel"></param>
        /// <returns></returns>
        public static List<T> GetChildObjects<T>(DependencyObject obj, bool isAllLevel = true) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T)
                {
                    childList.Add((T)child);
                    if (!isAllLevel)
                        continue;
                }

                childList.AddRange(GetChildObjects<T>(child, isAllLevel));
            }

            return childList;

        }

        /// <summary>
        /// 获取父控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetParent<T>(DependencyObject obj) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(obj);
            while (!(parent is T))
            {
                if (parent == null)
                    return null;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }
        #endregion
    }
}
