/* ==============================================================================
* Description：DetectionManager  
* Author     ：litao
* Create Date：2017/11/23 10:16:23
* ==============================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    internal class DetectionManager
    {
        #region 单例

        private DetectionManager()
        {
            BaseDataManager.Initialize();
            BaseDataManager.UpdateValidateData();
        }

        private static DetectionManager _instance = new DetectionManager();

        public static DetectionManager Instance
        {
            get { return _instance; }
        }

        #endregion

        /// <summary>
        /// 检测的结果放于CategoryManager中
        /// </summary>
        public ExtactionCategoryCollectionManager CategoryManager
        {
            get { return _categoryManager; }
        }

        private readonly ExtactionCategoryCollectionManager _categoryManager =
            new ExtactionCategoryCollectionManager() {Name = "智能检视"};

        /// <summary>
        /// 基础数据管理
        /// </summary>
        public readonly ConfigDataManager BaseDataManager = new ConfigDataManager();

        /// <summary>
        /// 检测
        /// </summary>
        public void Detect()
        {
            DeviceDataParser parser = new DeviceDataParser();
            ObservableCollection<DataExtactionItem> deviceData = parser.LoadDeviceData();
            BaseDataManager.UpdateValidateData();
            DetectResultList(deviceData, BaseDataManager.ValidateDataNodes);
        }

        private bool OnDetect(string content, IEnumerable<DataNode> validateDataNodes)
        {
            return validateDataNodes.Any(item => item.Data.Value == content);
        }

        private void DetectResultList(IEnumerable<DataExtactionItem> resultList,List<DataNode> validateDataNodes)
        {
            foreach (var dirItem in resultList)
            {
                var categoryCollection =(ExtactionCategoryCollection) CategoryManager.GetChild(dirItem.Text);

                foreach (var typeItem in dirItem.TreeNodes)
                {
                    var category = (ExtactionCategory) categoryCollection.GetChild(typeItem.Text);

                    foreach (var subItem in typeItem.TreeNodes)
                    {
                        var dataSource = (AbstractDataSource) subItem.Data;
                        if (dataSource.Total < 1)
                        {
                            continue;
                        }

                        var subCategory = (ExtactionSubCategory) category.GetChild(subItem.Text);

                        if (dataSource.Items != null)
                        {
                            PropertyInfo[] allPropertyInfos = ((Type) dataSource.Type).GetProperties();
                            List<PropertyInfo> propertyInfos = allPropertyInfos.Where(propertyInfo => propertyInfo.PropertyType == typeof (string)).ToList();

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
                                        bool ret = OnDetect(content, validateDataNodes);
                                        if (ret)
                                        {
                                            ExtactionItem extactionItem = subCategory.AddItem(subItem.Text);
                                            extactionItem.SetActualData(dataItem);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DetectResultList2(ObservableCollection<DataExtactionItem> resultList,
            List<DataNode> validateDataNodes)
        {
            foreach (var dirItem in resultList)
            {
                string dirItemPath = dirItem.Text;
                if (!Directory.Exists(dirItemPath))
                {
                    Directory.CreateDirectory(dirItemPath);
                }
                ExtactionCategoryCollection categoryCollection =
                    (ExtactionCategoryCollection) CategoryManager.GetChild(dirItem.Text);

                foreach (var typeItem in dirItem.TreeNodes)
                {
                    string typeItemPath = dirItemPath + @"\" + typeItem.Text;
                    if (!Directory.Exists(typeItemPath))
                    {
                        Directory.CreateDirectory(typeItemPath);
                    }

                    ExtactionCategory category = (ExtactionCategory) categoryCollection.GetChild(typeItem.Text);

                    foreach (var subItem in typeItem.TreeNodes)
                    {
                        string subItemPath = typeItemPath + @"\" + subItem.Text + @".txt";
                        AbstractDataSource dataSource = (AbstractDataSource) subItem.Data;
                        if (dataSource.Total < 1)
                        {
                            continue;
                        }

                        ExtactionSubCategory subCategory = (ExtactionSubCategory) category.GetChild(subItem.Text);

                        using (FileStream fs = new FileStream(subItemPath, FileMode.Create))
                        {
                            StreamWriter streamWriter = new StreamWriter(fs);
                            if (dataSource.Items != null)
                            {
                                PropertyInfo[] allPropertyInfos = ((Type) dataSource.Type).GetProperties();
                                List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
                                foreach (var propertyInfo in allPropertyInfos)
                                {
                                    if (propertyInfo.PropertyType == typeof (string))
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
                                            bool ret = OnDetect(content, validateDataNodes);
                                            if (ret)
                                            {
                                                ExtactionItem extactionItem = subCategory.AddItem(subItem.Text);
                                                extactionItem.SetActualData(dataItem);
                                            }

                                            streamWriter.WriteLine(content);
                                        }
                                    }
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }
            }
        }
    }
}
