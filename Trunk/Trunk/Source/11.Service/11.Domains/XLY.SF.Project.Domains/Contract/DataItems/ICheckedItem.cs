using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.ICheckedItem
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/12/8 9:52:32
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 包含了勾选状态的数据
    /// </summary>
    public interface ICheckedItem
    {
        /// <summary>
        /// 当前勾选的状态
        /// </summary>
        bool? IsChecked { get; set; }
        /// <summary>
        /// 当前节点的父节点
        /// </summary>
        ICheckedItem Parent { get; set; }
        string SourcePath { get; set; }
        /// <summary>
        /// 子节点列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICheckedItem> GetChildren();
    }
    
    public static class CheckedItemExtern
    {
        /// <summary>
        /// 设置勾选后的状态，如果有父节点或子节点，则会依次设置
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        /// <param name="OnPropertyChanged"></param>
        public static void SetCheckedState(this ICheckedItem item, bool? value, Action OnPropertyChanged)
        {
            if (item.IsChecked != value)
            {
                OnPropertyChanged();
                if (item.IsChecked == true) // 如果节点被选中
                {
                    if (item.GetChildren() != null)
                        foreach (var dt in item.GetChildren())
                            dt.IsChecked = true;
                    if (item.Parent != null)
                    {
                        Boolean bExistUncheckedChildren = false;
                        foreach (var dt in item.Parent.GetChildren())
                            if (dt.IsChecked != true)
                            {
                                bExistUncheckedChildren = true;
                                break; 
                            }
                        if (bExistUncheckedChildren)
                            item.Parent.IsChecked = null;
                        else
                            item.Parent.IsChecked = true;
                    }
                }
                else if (item.IsChecked == false)   // 如果节点未选中
                {
                    if (item.GetChildren() != null)
                        foreach (var dt in item.GetChildren())
                            dt.IsChecked = false;
                    if (item.Parent != null)
                    {
                        Boolean bExistCheckedChildren = false;
                        foreach (var dt in item.Parent.GetChildren())
                            if (dt.IsChecked != false)
                            {
                                bExistCheckedChildren = true;
                                break;
                            }
                        if (bExistCheckedChildren)
                            item.Parent.IsChecked = null;
                        else
                            item.Parent.IsChecked = false;
                    }
                }
                else
                {
                    if (item.Parent != null)
                        item.Parent.IsChecked = null;
                }
            }
        }

        /// <summary>
        /// 设置树节点的状态，如果有父节点或子节点，则会依次设置
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        /// <param name="OnPropertyChanged"></param>
        public static void SetTreeState(this ICheckedItem item, bool? value, Func<dynamic, bool?> getter, Action<dynamic, bool?> setter, Action OnPropertyChanged)
        {
            if (getter(item) != value)
            {
                OnPropertyChanged();
                if (getter(item) == true) // 如果节点被选中
                {
                    if (item.GetChildren() != null)
                        foreach (var dt in item.GetChildren())
                            setter(dt, true);
                    if (item.Parent != null)
                    {
                        Boolean bExistUncheckedChildren = false;
                        foreach (var dt in item.Parent.GetChildren())
                            if (getter(dt) != true)
                            {
                                bExistUncheckedChildren = true;
                                break;
                            }
                        if (bExistUncheckedChildren)
                            setter(item.Parent, null);
                        else
                            setter(item.Parent, true);
                    }
                }
                else if (getter(item) == false)   // 如果节点未选中
                {
                    if (item.GetChildren() != null)
                        foreach (var dt in item.GetChildren())
                            setter(dt, false);
                    if (item.Parent != null)
                    {
                        Boolean bExistCheckedChildren = false;
                        foreach (var dt in item.Parent.GetChildren())
                            if (getter(dt) != false)
                            {
                                bExistCheckedChildren = true;
                                break;
                            }
                        if (bExistCheckedChildren)
                            setter(item.Parent, null);
                        else
                            setter(item.Parent, false);
                    }
                }
                else
                {
                    if (item.Parent != null)
                        setter(item.Parent, null);
                }
            }
        }

        /// <summary>
        /// 遍历一棵树
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="action"></param>
        public static void TranverseTree(this ICheckedItem tree, Action<ICheckedItem> action)
        {
            if(tree == null)
            {
                return;
            }
            foreach (var item in tree.GetChildren())
            {
                if (item == null)
                    continue;
                action(item);
                TranverseTree(item, action);
            }
        }
    }
}
