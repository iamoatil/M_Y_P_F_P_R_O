/* ==============================================================================
* Description：AbstractCategory  
* Author     ：litao
* Create Date：2017/11/24 15:43:05
* ==============================================================================*/

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 表示类型的抽象类
    /// 在ExtactionCategory.cs中定义了4个由它派生的类，这4个类依次呈梯度关系
    /// </summary>
    abstract class AbstractCategory : ICategory,INotifyPropertyChanged
    {
        public Dictionary<string, IName> Children { get { return _children; } }
        private readonly Dictionary<string, IName> _children = new Dictionary<string, IName>();

        /// <summary>
        /// 此Category的名字。可显示到界面 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 孩子的个数。可显示到界面 
        /// </summary>
        public virtual int ChildrenCount { get { return Children.Count; } }

        /// <summary>
        /// 界面是否选择的是这个类型
        /// </summary>
        public bool IsChecked { get; set; }

        internal virtual IName GetChild(string name)
        {
            if (!Contain(name))
            {
                Add(name);
            }
            return Children[name];
        }

        internal bool Contain(string name)
        {
            bool ret = Children.ContainsKey(name);
            return ret;
        }

        protected abstract void Add(string name);

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性更新（不用给propertyName赋值）
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
