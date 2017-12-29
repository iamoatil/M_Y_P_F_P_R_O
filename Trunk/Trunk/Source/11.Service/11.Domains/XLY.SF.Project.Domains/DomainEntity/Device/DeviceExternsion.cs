using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.ViewModel;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Domains.DeviceExternsion
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/22 10:07:46
* ==============================================================================*/

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 设备操作扩展方法
    /// </summary>
    public static class DeviceExternsion
    {
        public const string XLY_BinKey = "device";
        public const string XLY_IdKey = "ID";
        public const string XLY_NameKey = "Name";

        /// <summary>
        /// 加载从配置文件中读取的属性配置，并生成Device设备
        /// </summary>
        /// <param name="dicPropertys"></param>
        /// <returns></returns>
        public static IDevice Load(Dictionary<string, string> dicPropertys)
        {
            if (dicPropertys == null || !dicPropertys.ContainsKey(XLY_IdKey) || !dicPropertys.ContainsKey(XLY_BinKey))
                return null;
            try
            {
                return Serializer.DeSerializeFromBinary<IDevice>(dicPropertys[XLY_BinKey].ToByteArray());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 生成配置文件所需属性配置项
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Save(this IDevice device)
        {
            Dictionary<string, string> dicPropertys = new Dictionary<string, string>();
            dicPropertys[XLY_IdKey] = device.ID;
            dicPropertys[XLY_NameKey] = device.Name;
            dicPropertys[XLY_BinKey] = Serializer.SerializeToBinary(device).ToHex();
            return dicPropertys;
        }

        /// <summary>
        /// 加载设备提取的数据，返回已经分组完成的数据集合
        /// </summary>
        /// <param name="devicePath"></param>
        /// <returns></returns>
        public static ObservableCollection<DataExtactionItem> LoadDeviceData(string devicePath)
        {
            var dataList = new ObservableCollection<DataExtactionItem>();
            if (devicePath == null || !Directory.Exists(devicePath))
            {
                return dataList;
            }
            //遍历每次提取的结果集
            foreach (var dir in Directory.GetDirectories(devicePath))
            {
                var extact = LoadSingleExtactData(dir);
                if (extact != null)
                {
                    dataList.Add(extact);
                    extact.SourcePath = devicePath;
                }
            }
            return dataList;
        }

        /// <summary>
        /// 加载单次提取的数据
        /// </summary>
        /// <param name="extactPath"></param>
        /// <returns></returns>
        public static DataExtactionItem LoadSingleExtactData(string extactPath)
        {
            if (!Directory.Exists(Path.Combine(extactPath, "Result")))     //如果包含了Result文件夹，则认为是测试数据
            {
                return null;
            }
            DirectoryInfo d = new DirectoryInfo(extactPath);
            DataExtactionItem extact = new DataExtactionItem() { Text = d.Name, TreeNodes = new ObservableCollection<DataExtactionItem>() };

            List<DataExtactionItem> ls = new List<DataExtactionItem>();
            foreach (var bin in Directory.GetFiles(Path.Combine(extactPath, "Result"), "*.ds"))        //ds为IDataSource二进制序列化包
            {
                try
                {
                    IDataSource ds = Serializer.DeSerializeFromBinary<IDataSource>(bin);
                    if (ds.PluginInfo == null)
                        continue;
                    ds.SetCurrentPath(extactPath);     //修改数据中的当前任务路径，因为原始数据中存储的是绝对路径
                    if (ds != null)
                    {
                        ds.Filter<dynamic>();
                        var de = new DataExtactionItem()
                        {
                            Text = ds.PluginInfo?.Name,
                            Index = ds.PluginInfo.OrderIndex,
                            Group = ds.PluginInfo?.Group,
                            GroupIndex = ds.PluginInfo.GroupIndex,
                            Data = ds,
                            Total = ds.Total,
                            DeleteTotal = ds.DeleteTotal,
                            TreeNodes = new ObservableCollection<DataExtactionItem>()
                        };
                        ds.Parent = de;
                        ds.BuildParent();
                        ls.Add(de);
                    }
                }
                catch 
                {

                }
            }

            //所有应用数据的分组和排序
            foreach (var group in ls.OrderBy(f => f.GroupIndex).GroupBy(g => g.Group))
            {
                DataExtactionItem g = new DataExtactionItem() { Text = group.Key, TreeNodes = new ObservableCollection<DataExtactionItem>() };
                g.TreeNodes.AddRange(group.ToList().OrderBy(p => p.Index));       //添加该分组的所有插件，并按照序号排序
                g.Total = group.ToList().Sum(p => p.Total);
                g.DeleteTotal = group.ToList().Sum(p => p.DeleteTotal);
                extact.TreeNodes.Add(g);
                extact.Total += g.Total;
                extact.DeleteTotal += g.DeleteTotal;
            }

            return extact;
        }

        /// <summary>
        /// 实现设备数据的拷贝
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<DataExtactionItem> CopyDataExtactionItem(this IEnumerable<DataExtactionItem> source)
        {
            List<DataExtactionItem> lst = new List<DataExtactionItem>();
            if (source == null)
            {
                return lst;
            }
            foreach (var item in source)
            {
                var c = new DataExtactionItem() { Text = item.Text, Group = item.Group, GroupIndex = item.GroupIndex, Index = item.Index, IsChecked = true, IsVisible = true, Data=null, TreeNodes = new ObservableCollection<DataExtactionItem>() };
                lst.Add(c);
                c.TreeNodes.AddRange(CopyDataExtactionItem(item.TreeNodes));
                foreach (var tn in c.TreeNodes)
                {
                    tn.Parent = c;
                }
            }
            
            return lst;
        }

        public static DataExtactionItem FindExtactionItemFromTree(this IEnumerable<DataExtactionItem> source, DataExtactionItem item)
        {
            if (item == null)
                return null;
            List<DataExtactionItem> path = new List<DataExtactionItem>();       //从根节点到当前节点的路径
            while(item != null)
            {
                path.Insert(0, item);
                item = item.Parent as DataExtactionItem;
            }
            IEnumerable<DataExtactionItem> nodes = source;
            DataExtactionItem result = null;
            foreach (var node in path)
            {
                result = nodes.FirstOrDefault(t => t.Text == node.Text);
                if(result == null)
                {
                    return null;
                }
                nodes = result.TreeNodes;
            }
            return result;
        }
    }

    /// <summary>
    /// 用于设备数据的读取
    /// </summary>
    public class DataExtactionItem: NotifyPropertyBase, ICheckedItem, IDecoration
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 应用分组
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 应用分组序号，用于排序
        /// </summary>
        public int GroupIndex { get; set; }
        /// <summary>
        /// 应用序号，用于排序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 包含的IDataSource，如果是分组节点则为空
        /// </summary>
        public object Data { get; set; }

        public bool IsHideChildren { get; set; }

        #region 数据总数
        private int _Total = 0;

        /// <summary>
        /// 数据总数
        /// </summary>	
        public int Total
        {
            get { return _Total; }
            set
            {
                _Total = value;
                OnPropertyChanged();
            }
        }

        private int _DeleteTotal = 0;

        /// <summary>
        /// 删除数据总数
        /// </summary>	
        public int DeleteTotal
        {
            get { return _DeleteTotal; }
            set
            {
                _DeleteTotal = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region 是否被勾选
        private bool? _isChecked = false;
        public new bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                this.SetCheckedState(value, ()=> { this._isChecked = value; OnPropertyChanged(); });
                if(_isChecked != null && Data is IDataSource ds)
                {
                    ds.Parent = this;
                    ds.IsChecked = value;
                }
                CheckedChanged?.Invoke(this, null);
            }
        }
        #endregion

        #region 是否可见
        private bool? _IsVisible = true;

        /// <summary>
        /// 是否可见
        /// </summary>	
        public bool? IsVisible
        {
            get { return _IsVisible; }
            set
            {
                this.SetTreeState(value, (item)=> item.IsVisible, (item,v)=> item.IsVisible = v, () => { this._IsVisible = value; OnPropertyChanged(); });
                VisibleChanged?.Invoke(this, null);
            }
        }
        #endregion

        /// <summary>
        /// 子节点列表
        /// </summary>
        public ObservableCollection<DataExtactionItem> TreeNodes { get; set; }

        public ICheckedItem Parent { get; set; }

        public event EventHandler VisibleChanged;
        public event EventHandler CheckedChanged;

        public void BuildParent()
        {
            if (TreeNodes != null)
            {
                foreach (var item in TreeNodes)
                {
                    item.Parent = this;
                    item.BuildParent();
                }
            }
        }

        public IEnumerable<ICheckedItem> GetChildren()
        {
            return Data == null ? TreeNodes : (Data as IDataSource)?.GetChildren();
        }

        public object GetMetaData(DecorationProperty dp)
        {
            return SourcePath;
        }

        public string GetKey(DecorationProperty dp)
        {
            return $"DataExtactionItem_{Text}";
        }

        private string _sourcePath = null;
        public string SourcePath { get => _sourcePath ?? Parent?.SourcePath; set => _sourcePath = value; }
    }

}
