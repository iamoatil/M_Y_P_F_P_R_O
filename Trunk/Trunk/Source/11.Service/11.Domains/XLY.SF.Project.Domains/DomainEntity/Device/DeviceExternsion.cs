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
                //object obj = Activator.CreateInstance(typeof(IDevice).Assembly.FullName, dicPropertys[XLY_TypeKey]);
                //foreach (var item in dicPropertys)
                //{
                //    if(item.Key != XLY_TypeKey)
                //    {
                //        obj.Setter(item.Key, item.Value.ChangeType(obj.GetType().GetProperty(item.Key).PropertyType));
                //    }
                //}
                //return obj as IDevice;

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
            //dicPropertys[XLY_TypeKey] = device.GetType().FullName;
            //foreach(var pi in device.GetType().GetProperties())
            //{
            //    var dp = pi.GetCustomAttribute(typeof(DPConfigAttribute));
            //    if(dp != null)
            //    {
            //        dicPropertys[pi.Name] = pi.GetValue(device).ToSafeString();
            //    }
            //}
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
                if(extact != null)
                {
                    dataList.Add(extact);
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
                    ds.SetCurrentPath(extactPath);     //修改数据中的当前任务路径，因为原始数据中存储的是绝对路径
                    if (ds != null)
                    {
                        ds.Filter<dynamic>();
                        ls.Add(new DataExtactionItem()
                        {
                            Text = ds.PluginInfo?.Name,
                            Index = ds.PluginInfo == null ? 0 : ds.PluginInfo.OrderIndex,
                            Group = ds.PluginInfo?.Group,
                            Data = ds,
                            TreeNodes = new ObservableCollection<DataExtactionItem>()
                        });
                    }
                }
                catch(Exception e)
                {

                }
            }
            //所有应用数据的分组和排序
            foreach (var group in ls.GroupBy(g => g.Group))
            {
                DataExtactionItem g = new DataExtactionItem() { Text = group.Key, TreeNodes = new ObservableCollection<DataExtactionItem>() };
                g.TreeNodes.AddRange(group.ToList().OrderBy(p => p.Index));       //添加该分组的所有插件，并按照序号排序
                extact.TreeNodes.Add(g);
            }

            return extact;
        }
    }


    /// <summary>
    /// 用于设备数据的读取
    /// </summary>
    public class DataExtactionItem: NotifyPropertyBase
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
        /// 应用序号，用于排序
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 包含的IDataSource，如果是分组节点则为空
        /// </summary>
        public object Data { get; set; }

        public bool IsHideChildren { get; set; }
        public bool IsItemStyle { get; set; }
        public int Total { get; set; }

        private bool? isChecked = true;
        public bool? IsChecked
        {
            get { return isChecked; }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    OnPropertyChanged();
                    if (this.isChecked == true) // 如果节点被选中
                    {
                        if (this.TreeNodes != null)
                            foreach (var dt in this.TreeNodes)
                                dt.IsChecked = true;
                        if (this.Parent != null)
                        {
                            Boolean bExistUncheckedChildren = false;
                            foreach (var dt in this.Parent.TreeNodes)
                                if (dt.IsChecked != true)
                                {
                                    bExistUncheckedChildren = true;
                                    break;
                                }
                            if (bExistUncheckedChildren)
                                this.Parent.IsChecked = null;
                            else
                                this.Parent.IsChecked = true;
                        }
                    }
                    else if (this.isChecked == false)   // 如果节点未选中
                    {
                        if (this.TreeNodes != null)
                            foreach (var dt in this.TreeNodes)
                                dt.IsChecked = false;
                        if (this.Parent != null)
                        {
                            Boolean bExistCheckedChildren = false;
                            foreach (var dt in this.Parent.TreeNodes)
                                if (dt.IsChecked != false)
                                {
                                    bExistCheckedChildren = true;
                                    break;
                                }
                            if (bExistCheckedChildren)
                                this.Parent.IsChecked = null;
                            else
                                this.Parent.IsChecked = false;
                        }
                    }
                    else
                    {
                        if (this.Parent != null)
                            this.Parent.IsChecked = null;
                    }
                }
            }
        }
        public bool IsSelected { get; set; }
        public DataExtactionItem Parent { get; set; }

        /// <summary>
        /// 子节点列表
        /// </summary>
        public ObservableCollection<DataExtactionItem> TreeNodes { get; set; }

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
    }

}
