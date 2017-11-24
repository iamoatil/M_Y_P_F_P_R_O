/* ==============================================================================
* Description：AbstractCategory  
* Author     ：litao
* Create Date：2017/11/24 15:43:05
* ==============================================================================*/

using System.Collections.Generic;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 表示类型的抽象类
    /// 在ExtactionCategory.cs中定义了4个由它派生的类，这4个类依次呈梯度关系
    /// </summary>
    abstract class AbstractCategory : ICategory
    {
        protected Dictionary<string, IName> Children = new Dictionary<string, IName>();

        /// <summary>
        /// 此Category的名字。可显示到界面 
        /// </summary>
        public string Name { get ; set; }

        /// <summary>
        /// 孩子的个数。可显示到界面 
        /// </summary>
        public int ChildrenCount { get { return Children.Count; } }

        /// <summary>
        /// 界面是否选择的是这个类型
        /// </summary>
        public bool IsChecked { get; set; }

        internal IName GetChild(string name)
        {
            if(!Contain(name))
            {
               Add(name);
            }
            return Children[name];
        }

        internal bool Contain(string name)
        {
            bool ret=Children.ContainsKey(name);
            return ret;
        }

        protected abstract void Add(string name);
    }
}
