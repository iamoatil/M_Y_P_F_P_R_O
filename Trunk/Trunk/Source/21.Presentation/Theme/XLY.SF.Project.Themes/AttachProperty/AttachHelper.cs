using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
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
    public static class AttachHelper
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

        #region 带标题控件

        #region 水平对齐方式

        /// <summary>
        /// 标题水平对齐附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleAlignmentProperty =
            DependencyProperty.RegisterAttached("TitleAlignment", typeof(TextAlignment), typeof(AttachHelper), new PropertyMetadata(TextAlignment.Justify));

        public static TextAlignment GetTitleAlignment(DependencyObject obj)
        {
            return (TextAlignment)obj.GetValue(TitleAlignmentProperty);
        }

        public static void SetTitleAlignment(DependencyObject obj, TextAlignment value)
        {
            obj.SetValue(TitleAlignmentProperty, value);
        }

        #endregion

        #region 标题

        /// <summary>
        /// 标题附加依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.RegisterAttached("Title", typeof(String), typeof(AttachHelper), new PropertyMetadata(null));

        public static String GetTitle(DependencyObject obj)
        {
            return obj.GetValue(TitleProperty) as String;
        }

        public static void SetTitle(DependencyObject obj, String value)
        {
            obj.SetValue(TitleProperty, value);
        }

        #endregion

        #endregion

        #region 动态绑定选择的树节点，设置后可以选择并展开当前节点

        public static object GetSelectedTreeNode(DependencyObject obj)
        {
            return (object)obj.GetValue(SelectedTreeNodeProperty);
        }

        public static void SetSelectedTreeNode(DependencyObject obj, object value)
        {
            obj.SetValue(SelectedTreeNodeProperty, value);
        }

        /// <summary>
        /// 动态绑定选择的树节点，设置后可以选择并展开当前节点
        /// </summary>
        public static readonly DependencyProperty SelectedTreeNodeProperty =
            DependencyProperty.RegisterAttached("SelectedTreeNode", typeof(object), typeof(AttachHelper), new PropertyMetadata(null, OnSelectedTreeNodeChanged));

        private static void OnSelectedTreeNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView tv = d as TreeView;
            if (tv == null)
                return;
            var path = GetTreePath(e.NewValue);
            if(path != null && path.Count > 0)
            {
                if((bool)tv.GetValue(IsIgnoreTopNodeProperty))      //忽略最顶层的节点
                    path.RemoveAt(0);
                FindTreeItemByData(tv, path, true, ti => { ti.IsSelected = true; ti.IsExpanded = true; });
            }
        }

        /// <summary>
        /// 获取数据的层级路径，数据必须包含Parent属性
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<dynamic> GetTreePath(dynamic data)
        {
            if(data == null)
            {
                return null;
            }
            List<dynamic> path = new List<dynamic>();
            path.Add(data);
            if (data.Parent != null)
            {
                path.InsertRange(0, GetTreePath(data.Parent));
            }
            return path;
        }

        /// <summary>
        /// 通过数据查找对应的TreeViewItem
        /// </summary>
        /// <param name="tv">当前操作的容器</param>
        /// <param name="data">从根节点开始的数据路径</param>
        /// <param name="isTriggerPath">true则依次触发路径上的所有节点，false则只触发最后选择的节点</param>
        /// <param name="action"></param>
        private static void FindTreeItemByData(ItemsControl tv, List<dynamic> data, bool isTriggerPath, Action<TreeViewItem> action)
        {
            if (data == null || data.Count == 0)
                return;
            if (tv.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                if (tv.IsLoaded)
                {
                    tv.UpdateLayout();
                    if (tv.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                    {
                        FindTreeItemByData(tv, data, isTriggerPath, action);
                    }
                    else
                    {
                        tv.ItemContainerGenerator.StatusChanged += (sd, ex) =>
                        {
                            if (tv.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                            {
                                FindTreeItemByData(tv, data, isTriggerPath, action);
                            }
                        };
                    }
                }
                else
                {
                    tv.Loaded += (sd, ex) => {
                        FindTreeItemByData(tv, data, isTriggerPath, action);
                    };
                }
            }
            else
            {
                var ti = tv.ItemContainerGenerator.ContainerFromItem(data[0]) as TreeViewItem;
                if (ti != null)
                {
                    data.RemoveAt(0);
                    if (isTriggerPath || data.Count == 0)
                        action(ti);
                    FindTreeItemByData(ti, data, isTriggerPath, action);
                }
            }
        }
        #endregion

        #region 动态绑定选择的树节点时，是否忽略最顶层的节点

        public static bool GetIsIgnoreTopNode(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsIgnoreTopNodeProperty);
        }

        public static void SetIsIgnoreTopNode(DependencyObject obj, bool value)
        {
            obj.SetValue(IsIgnoreTopNodeProperty, value);
        }

        /// <summary>
        /// 动态绑定选择的树节点时，是否忽略最顶层的节点
        /// </summary>
        public static readonly DependencyProperty IsIgnoreTopNodeProperty =
            DependencyProperty.RegisterAttached("IsIgnoreTopNode", typeof(bool), typeof(AttachHelper), new PropertyMetadata(false));

        #endregion

        #region 树控件默认展开的节点层数

        public static int GetTreeItemDefaultExpandDepth(DependencyObject obj)
        {
            return (int)obj.GetValue(TreeItemDefaultExpandDepthProperty);
        }

        public static void SetTreeItemDefaultExpandDepth(DependencyObject obj, int value)
        {
            obj.SetValue(TreeItemDefaultExpandDepthProperty, value);
        }

        /// <summary>
        /// 树控件默认展开的节点层数
        /// </summary>
        public static readonly DependencyProperty TreeItemDefaultExpandDepthProperty =
            DependencyProperty.RegisterAttached("TreeItemDefaultExpandDepth", typeof(int), typeof(AttachHelper), new PropertyMetadata(-1, OnTreeItemExpandedDepthChanged));

        private static void OnTreeItemExpandedDepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int depth = (int)d.GetValue(TreeItemDefaultExpandDepthProperty);
            if(depth >= 0 && d is TreeView tv)
            {
                TranverseTreeItemByDepth(tv, depth, ti => ti.IsExpanded = true);
            }
        }

        /// <summary>
        /// 根据树的深度遍历树节点
        /// </summary>
        /// <param name="container"></param>
        /// <param name="depth"></param>
        /// <param name="action"></param>
        private static void TranverseTreeItemByDepth(ItemsControl container, int depth, Action<TreeViewItem> action)
        {
            if (depth < 0)
                return;
            if(!container.IsLoaded)
            {
                container.Loaded += (sender, e) => { TranverseTreeItemByDepth(container, depth, action); };
            }
            else
            {
                foreach (var item in container.Items)
                {
                    var dp = container.ItemContainerGenerator.ContainerFromItem(item);
                    if(dp is TreeViewItem ti)
                    {
                        action(ti);
                        TranverseTreeItemByDepth(ti, depth - 1, action);
                    }
                }
            }
        }

        #endregion



        #endregion

        #region 滚动翻页附加属性
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
            DependencyProperty.RegisterAttached("IsScrollPage", typeof(bool), typeof(AttachHelper), new PropertyMetadata(false, OnScrollPageChanged));

        private static void OnScrollPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scroll && (bool)scroll.GetValue(IsScrollPageProperty))
            {
                var behaviors = Interaction.GetBehaviors(scroll);
                behaviors.Add(new ScrollPageBehavior());
            }
        }
        #endregion

        #region 滚动翻页时每页数据条数，默认50

        public static int GetScrollPageSize(DependencyObject obj)
        {
            return (int)obj.GetValue(ScrollPageSizeProperty);
        }

        public static void SetScrollPageSize(DependencyObject obj, int value)
        {
            obj.SetValue(ScrollPageSizeProperty, value);
        }

        /// <summary>
        /// 滚动翻页时每页数据条数，默认50
        /// </summary>
        public static readonly DependencyProperty ScrollPageSizeProperty =
            DependencyProperty.RegisterAttached("ScrollPageSize", typeof(int), typeof(AttachHelper), new PropertyMetadata(50));

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
