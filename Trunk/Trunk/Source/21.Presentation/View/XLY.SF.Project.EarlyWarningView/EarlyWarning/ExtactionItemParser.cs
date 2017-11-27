/* ==============================================================================
* Description：ExtactionItemParser  
* Author     ：litao
* Create Date：2017/11/24 13:40:28
* ==============================================================================*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class ExtactionItemParser
    {
        public event Func<string,bool> DetectAction;
        
        public ExtactionCategoryCollectionManager CategoryManager { get { return _categoryManager; } }
        ExtactionCategoryCollectionManager _categoryManager = new ExtactionCategoryCollectionManager() { Name="智能检视"};

        public void Detect()
        {            
            string dir = @"C:\Users\litao\Desktop\迭代66\123_20171123[021724]\R7_20171123[021726]";
            if(!Directory.Exists(dir))
            {
                return;
            }
            ObservableCollection<DataExtactionItem> resultList = LoadDeviceData(dir);
            GenerateTextFile(resultList);
        }        
        
        private bool OnDetect(string content)
        {
            if(DetectAction != null)
            {
               return DetectAction(content);
            }

            return false;
        }

        private void GenerateTextFile(ObservableCollection<DataExtactionItem> resultList)
        {
            foreach (var dirItem in resultList)
            {
                string dirItemPath = dirItem.Text;
                if (!Directory.Exists(dirItemPath))
                {
                    Directory.CreateDirectory(dirItemPath);
                }
                ExtactionCategoryCollection categoryCollection = (ExtactionCategoryCollection)CategoryManager.GetChild(dirItem.Text);

                foreach (var typeItem in dirItem.TreeNodes)
                {
                    string typeItemPath = dirItemPath + @"\" + typeItem.Text;
                    if (!Directory.Exists(typeItemPath))
                    {
                        Directory.CreateDirectory(typeItemPath);
                    }

                    ExtactionCategory category= (ExtactionCategory)categoryCollection.GetChild(typeItem.Text);

                    foreach (var subItem in typeItem.TreeNodes)
                    {
                        string subItemPath = typeItemPath + @"\" + subItem.Text + @".txt";
                        AbstractDataSource dataSource = (AbstractDataSource)subItem.Data;
                        if (dataSource.Total < 1)
                        {
                            continue;
                        }                       

                        ExtactionSubCategory subCategory = (ExtactionSubCategory)category.GetChild(typeItem.Text);

                        using (FileStream fs = new FileStream(subItemPath, FileMode.Create))
                        {
                            //StreamWriter streamWriter = new StreamWriter(fs);
                            if (dataSource.Items != null)
                            {
                                PropertyInfo[] allPropertyInfos = ((Type)dataSource.Type).GetProperties();
                                List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
                                foreach (var propertyInfo in allPropertyInfos)
                                {
                                    if (propertyInfo.PropertyType == typeof(string))
                                    {
                                        propertyInfos.Add(propertyInfo);
                                    }
                                }

                                IEnumerable view = dataSource.Items.View;
                                foreach (AbstractDataItem dataItem in view)
                                {
                                    foreach (var propertyInfo in propertyInfos)
                                    {
                                        object ob = propertyInfo.GetValue(dataItem);
                                        if (ob == null)
                                        {
                                            continue;
                                        }
                                        string content = ob.ToString();
                                        if (!string.IsNullOrEmpty(content))
                                        {
                                            bool ret = OnDetect(content);
                                            if(ret)
                                            {
                                                ExtactionItem extactionItem = (ExtactionItem)subCategory.GetChild(subItem.Text);
                                                extactionItem.SetActualData(dataItem);
                                            }                                            

                                            //streamWriter.WriteLine(content);
                                        }
                                    }
                                }
                                //streamWriter.Close();
                            }
                        }
                    }
                }
            }
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
