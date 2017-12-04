using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Plugin.DataView;

/* ==============================================================================
* Description：DataExtactionItemCollection  
* Author     ：litao
* Create Date：2017/11/29 14:10:48
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    class DataExtactionItemCollection
    {
        private Dictionary<string, DataExtactionItem> _categoryCollections = new Dictionary<string, DataExtactionItem>();
        private Dictionary<string, DataExtactionItem> _categorys = new Dictionary<string, DataExtactionItem>();
        private Dictionary<string, DataExtactionItem> _subCategorys = new Dictionary<string, DataExtactionItem>();

        public ObservableCollection<DataExtactionItem> DataList
        {
            get { return _dataList; }
        }

        public ObservableCollection<object> LayoutViewItems { get; private set; }
        public object SelectedLayoutViewItem { get; private set; }
        public bool HasData { get; private set; }

        private ObservableCollection<DataExtactionItem> _dataList = new ObservableCollection<DataExtactionItem>();



        public DataExtactionItem GetCategoryCollection(string name)
        {
            if(!_categoryCollections.Keys.Contains(name))
            {
                var item = new DataExtactionItem() { Text = name, IsItemStyle = true, TreeNodes = new ObservableCollection<DataExtactionItem>() };
                _categoryCollections.Add(name, item);
                DataList.Add(item);
            }
            return _categoryCollections[name];
        }

        public DataExtactionItem GetCategory(string name, DataExtactionItem owner)
        {
            if (!_categorys.Keys.Contains(name))
            {
                var item = new DataExtactionItem() { Text = name, IsItemStyle = false, TreeNodes = new ObservableCollection<DataExtactionItem>() };
                _categorys.Add(name, item);
                owner.TreeNodes.Add(item);
            }
            return _categorys[name];
        }

        public DataExtactionItem GetSubCategoryn(string name,Object data, DataExtactionItem owner)
        {
            if (!_subCategorys.Keys.Contains(name))
            {
                var item = new DataExtactionItem() { Text = name, IsItemStyle = false, Data=data,TreeNodes = new ObservableCollection<DataExtactionItem>() };
                _subCategorys.Add(name, item);
                owner.TreeNodes.Add(item);
            }
            return _subCategorys[name];
        }

        public void SelectDefaultNode()
        {
            SelectDefaultNode(DataList);
        }

        private bool SelectDefaultNode(ObservableCollection<DataExtactionItem> nodes)
        {
            if (nodes == null)
            {
                return false;
            }
            foreach (var item in nodes)
            {
                if (item != null && item.Data != null)
                {
                    item.IsSelected = true;
                    DoSelecedAppChanged(item);
                    return true;
                }
                if (item.TreeNodes != null)
                {
                    if (SelectDefaultNode(item.TreeNodes))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 选择了某个APP的数据命令
        /// </summary>
        private void DoSelecedAppChanged(object app)
        {
            if (app == null)
                return;
            if (app is DataExtactionItem treeItem && treeItem.Data != null)
            {
                LayoutViewItems = new ObservableCollection<object>();
                foreach (var item in DataViewPluginAdapter.Instance.GetView(treeItem.Text, DataViewConfigure.XLY_LAYOUT_KEY))
                {
                    LayoutViewItems.Add(item.ToControl(new DataViewPluginArgument() { CurrentData = null, DataSource = treeItem.Data as IDataSource }));
                }
                SelectedLayoutViewItem = LayoutViewItems.FirstOrDefault();
            }
            HasData = SelectedLayoutViewItem != null;
        }
    }
}
