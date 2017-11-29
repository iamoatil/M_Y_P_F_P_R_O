/* ==============================================================================
* Description：ExtactionCategory 
*           此文件中定义了4个关于Category的类，他们都继承于AbstractCategory类
*           以下左边的是右边的集合类           
*               ExtactionCategoryCollectionManager -------》ExtactionCategoryCollection
*               ExtactionCategoryCollection        -------》 ExtactionCategory
*               ExtactionCategory                  -------》 ExtactionSubCategory
*               ExtactionSubCategory               -------》 ExtactionItem
*           
* Author     ：litao
* Create Date：2017/11/24 15:34:06
* ==============================================================================*/



using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.DataView;

namespace XLY.SF.Project.EarlyWarningView
{
    /* ==============================================================================
*           此文件中定义了4个关于Category的类，他们都继承于AbstractCategory类
*           以下左边的是右边的集合类           
*               ExtactionCategoryCollectionManager -------》ExtactionCategoryCollection
*               ExtactionCategoryCollection        -------》 ExtactionCategory
*               ExtactionCategory                  -------》 ExtactionSubCategory
*               ExtactionSubCategory               -------》 ExtactionItem
*           

* ==============================================================================*/

    class ExtactionCategoryCollectionManager : AbstractCategory,INotifyPropertyChanged
    {
        protected override void Add(string name)
        {
            Children.Add(name, new ExtactionCategoryCollection() { Name = name });
            SelectedItemShowCommand = new RelayCommand<object>(SelectedItemShow);
        }

        /// <summary>
        /// 选项显示命令
        /// </summary>
        public ICommand SelectedItemShowCommand { get; private set; }

        private List<AbstractDataItem> _items;

        public List<AbstractDataItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }
            

        private void SelectedItemShow(object o)
        {
            ExtactionSubCategory subCategory=o as ExtactionSubCategory;
            if (subCategory != null)
            {
                //Items = subCategory.Items;
                //item.ToControl(new DataViewPluginArgument() { CurrentData = null, DataSource = Items.Data as IDataSource })
            }
        }

        private ObservableCollection<object> _layoutViewItems;

        /// <summary>
        /// 布局视图集合
        /// </summary>	
        public ObservableCollection<object> LayoutViewItems
        {
            get { return _layoutViewItems; }
            set
            {
                _layoutViewItems = value;
                OnPropertyChanged();
            }
        }

        private object _selectedLayoutViewItem;

        /// <summary>
        /// 当前选择的布局视图
        /// </summary>	
        public object SelectedLayoutViewItem
        {
            get { return _selectedLayoutViewItem; }
            set
            {
                _selectedLayoutViewItem = value;
                OnPropertyChanged();
            }
        }
    }

    class ExtactionCategoryCollection : AbstractCategory
    {
        protected override void Add(string name)
        {
            Children.Add(name, new ExtactionCategory() { Name = name });
        }
    }

    class ExtactionCategory : AbstractCategory
    {
        protected override void Add(string name)
        {
            Children.Add(name, new ExtactionSubCategory() { Name = name });
        }
    }

    /// <summary>
    /// 此为核心。其下是数据，比如它的名字为“安装应用（10）”，其下的数据就为安装了的APP，比如QQ，微信等
    /// </summary>
    class ExtactionSubCategory : AbstractCategory
    {
        public void AddItem(AbstractDataItem item)
        {
            if(!Items.Contains(item))
            {
                Items.Add(item);
            }            
        }

        protected override void Add(string name)
        {
            throw new System.NotImplementedException();
        }

        public override int ChildrenCount
        {
            get { return Items.Count; }
        }

        public List<AbstractDataItem> Items { get { return _items; } }
        readonly List<AbstractDataItem> _items = new List<AbstractDataItem>();
    }

    class ExtactionItem : IName
    {
        private AbstractDataItem _dataItem;
        
        public string Name { get; set; }        
        
        
        internal void SetActualData(AbstractDataItem dataItem)
        {
            _dataItem = dataItem;
        }        
    }
}
