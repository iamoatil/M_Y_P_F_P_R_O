/* ==============================================================================
* Description：ExtactionItemParser  
* Author     ：litao
* Create Date：2017/11/24 13:40:28
* ==============================================================================*/


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class DeviceDataParser
    {
        public ObservableCollection<DataExtactionItem> LoadDeviceData()
        {            
            string dir = @"C:\Users\litao\Desktop\迭代66\123_20171123[021724]\R7_20171123[021726]";           
            return LoadDeviceData(dir);
        }      
        /// <summary>
        /// 加载设备提取的数据，返回已经分组完成的数据集合
        /// </summary>
        /// <param name="devicePath"></param>
        /// <returns></returns>
        private ObservableCollection<DataExtactionItem> LoadDeviceData(string devicePath)
        {
            var dataList = new ObservableCollection<DataExtactionItem>();
            if (!Directory.Exists(devicePath))
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
                }
            }
            return dataList;
        }

        /// <summary>
        /// 加载单次提取的数据
        /// </summary>
        /// <param name="extactPath"></param>
        /// <returns></returns>
        private DataExtactionItem LoadSingleExtactData(string extactPath)
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
                            Text = ds.PluginInfo.Name,
                            Index = ds.PluginInfo == null ? 0 : ds.PluginInfo.OrderIndex,
                            Group = ds.PluginInfo.Group,
                            Data = ds,
                            TreeNodes = new ObservableCollection<DataExtactionItem>()
                        });
                    }
                }
                catch
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
}
