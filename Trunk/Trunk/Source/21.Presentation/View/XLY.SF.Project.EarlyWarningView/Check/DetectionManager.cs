/* ==============================================================================
* Description：DetectionManager  
* Author     ：litao
* Create Date：2017/11/23 10:16:23
* ==============================================================================*/

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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


        private static string EarlyWarning = "EarlyWarning";
        /// <summary>
        /// 检测的结果放于CategoryManager中
        /// </summary>
        public ExtactionCategoryCollectionManager CategoryManager
        {
            get { return _categoryManager; }
        }

        private readonly ExtactionCategoryCollectionManager _categoryManager =
            new ExtactionCategoryCollectionManager() {Name = "智能检视"};

        public DataExtactionItemCollection DataExtactionItemCollection
        {
            get { return _dataExtactionItemCollection; }
        }

        DataExtactionItemCollection _dataExtactionItemCollection = new DataExtactionItemCollection();

        /// <summary>
        /// 基础数据管理
        /// </summary>
        public readonly ConfigDataManager BaseDataManager = new ConfigDataManager();

        /// <summary>
        /// 检测
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Detect()
        {
            DeviceDataParser parser = new DeviceDataParser();
            parser.LoadDeviceData();
            List<DeviceDataSource> dataSources =parser.DataSources;

            BaseDataManager.UpdateValidateData();
            foreach (var item in dataSources)
            {
                Match(item, BaseDataManager.ValidateDataNodes);
            }

            DataExtactionItemCollection.SelectDefaultNode();
        }

        
        private void Match(DeviceDataSource ds, List<DataNode> dataNodes)
        {
            //SqliteDbFile dbFile = SqliteDbFile.GetSqliteDbFile(EarlyWarning);
            //读取数据库中JsonColumnName列，并且匹配
            IDataSource dataSource = ds.DataSource;
            if (dataSource.Items == null)
            {
                return;
            }

            string dir = Path.GetDirectoryName(Path.GetDirectoryName(ds.DsFilePath));
            string extactionName = dir.Substring(dir.LastIndexOf("\\") + 1);
            
            ExtactionCategoryCollection categoryCollection = (ExtactionCategoryCollection)_categoryManager.GetChild(extactionName);
            ExtactionCategory category = (ExtactionCategory)categoryCollection.GetChild(dataSource.PluginInfo.Group);
            ExtactionSubCategory subCategory = (ExtactionSubCategory)category.GetChild(dataSource.PluginInfo.Name);

            DataExtactionItem cateColl=_dataExtactionItemCollection.GetCategoryCollection(extactionName);
            DataExtactionItem cate = _dataExtactionItemCollection.GetCategory(dataSource.PluginInfo.Group, cateColl);
            DataExtactionItem subCate = _dataExtactionItemCollection.GetSubCategoryn(dataSource.PluginInfo.Name, dataSource, cate);

            foreach (DataNode dataNode in dataNodes)
            {
                string cmd = string.Format("{1} like '%{2}%'", dataSource.Items.DbTableName, SqliteDbFile.JsonColumnName, dataNode.Data.Value);
                IEnumerable<dynamic> result = dataSource.Items.FilterByCmd<dynamic>(cmd);
                foreach (AbstractDataItem a in result)
                {
                    subCategory.AddItem(a);
                }
            }           
        }

        //private void Match2(DeviceDataSource ds, List<DataNode> dataNodes)
        //{
        //    //读取数据库中JsonColumnName列，并且匹配
        //    IDataSource dataSource = ds.DataSource;
        //    if (dataSource.Items == null)
        //    {
        //        return;
        //    }
        //    string dir = Path.GetDirectoryName(Path.GetDirectoryName(ds.DsFilePath));
        //    string extactionName = dir.Substring(dir.LastIndexOf("\\") + 1);
        //    ExtactionCategoryCollection categoryCollection = (ExtactionCategoryCollection)_categoryManager.GetChild(extactionName);
        //    ExtactionCategory category = (ExtactionCategory)categoryCollection.GetChild(dataSource.PluginInfo.Group);
        //    ExtactionSubCategory subCategory = (ExtactionSubCategory)category.GetChild(dataSource.PluginInfo.Name);

        //    SqliteDbFile sqliteDbFile = dataSource.Items.DbInstance;
        //    string connectString = sqliteDbFile.DbConnectionStr;
        //    string tableName = dataSource.Items.DbTableName;
        //    string cmdText = string.Format("select * from {0}", tableName);
        //    IEnumerable view = dataSource.Items.View;

        //    using (SQLiteConnection connect = new SQLiteConnection(connectString))
        //    {
        //        connect.Open();
        //        using (var command = new SQLiteCommand(connect))
        //        {
        //            command.CommandText = cmdText;
        //            SQLiteDataReader reader = command.ExecuteReader();
        //            try
        //            {
        //                foreach (DbDataRecord dataRecord in reader)
        //                {
        //                    if (dataRecord.FieldCount > 0)
        //                    {
        //                        string jsonContent = (string)dataRecord[SqliteDbFile.JsonColumnName];
        //                        foreach (DataNode item in dataNodes)
        //                        {
        //                            bool ret = jsonContent.Contains(item.Data.Value);
        //                            if (ret)
        //                            {
        //                                //ExtactionItem extactionItem = subCategory.AddItem(jsonContent);

        //                                // extactionItem.SetActualData(dataSource);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.InnerException);
        //            }
        //        }
        //    }
        //}

        //private void Match3(DeviceDataSource ds, List<DataNode> dataNodes)
        //{
        //    //读取数据库中JsonColumnName列，并且匹配
        //    IDataSource dataSource = ds.DataSource;
        //    if (dataSource.Items == null)
        //    {
        //        return;
        //    }
        //    string dir = Path.GetDirectoryName(Path.GetDirectoryName(ds.DsFilePath));
        //    string extactionName = dir.Substring(dir.LastIndexOf("\\") + 1);
        //    ExtactionCategoryCollection categoryCollection = (ExtactionCategoryCollection)_categoryManager.GetChild(extactionName);
        //    ExtactionCategory category = (ExtactionCategory)categoryCollection.GetChild(dataSource.PluginInfo.Group);
        //    ExtactionSubCategory subCategory = (ExtactionSubCategory)category.GetChild(dataSource.PluginInfo.Name);

        //    AbstractDataSource abstractDataSource = (AbstractDataSource)dataSource;
        //    if (dataSource.Total < 1)
        //    {
        //        return;
        //    }

        //    PropertyInfo[] allPropertyInfos = ((Type)abstractDataSource.Type).GetProperties();
        //    List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
        //    foreach (var propertyInfo in allPropertyInfos)
        //    {
        //        //if (propertyInfo.Attributes == typeof(string))
        //        //{
        //        //    propertyInfos.Add(propertyInfo);
        //        //}
        //    }

        //    IEnumerable view = dataSource.Items.View;
        //    foreach (AbstractDataItem dataItem in view)
        //    {
        //        foreach (var propertyInfo in propertyInfos)
        //        {
        //            object ob = propertyInfo.GetValue(dataItem);
        //            if (ob == null)
        //            {
        //                continue;
        //            }
        //            string content = ob.ToString();
        //            if (!string.IsNullOrEmpty(content))
        //            {

        //                //bool ret = OnDetect(content, validateDataNodes);
        //                //if (ret)
        //                //{
        //                //    ExtactionItem extactionItem = subCategory.AddItem(subItem.Text);
        //                //    extactionItem.SetActualData(dataItem);
        //                //}
        //            }
        //        }
        //    }
        //}
    

        //private bool OnDetect(string content, IEnumerable<DataNode> validateDataNodes)
        //{
        //    return validateDataNodes.Any(item => item.Data.Value == content);
        //}
        
        //private void DetectResultList2(ObservableCollection<DataExtactionItem> resultList,
        //    List<DataNode> validateDataNodes)
        //{
        //    foreach (var dirItem in resultList)
        //    {
        //        string dirItemPath = dirItem.Text;
        //        if (!Directory.Exists(dirItemPath))
        //        {
        //            Directory.CreateDirectory(dirItemPath);
        //        }
        //        ExtactionCategoryCollection categoryCollection =
        //            (ExtactionCategoryCollection) CategoryManager.GetChild(dirItem.Text);

        //        foreach (var typeItem in dirItem.TreeNodes)
        //        {
        //            string typeItemPath = dirItemPath + @"\" + typeItem.Text;
        //            if (!Directory.Exists(typeItemPath))
        //            {
        //                Directory.CreateDirectory(typeItemPath);
        //            }

        //            ExtactionCategory category = (ExtactionCategory) categoryCollection.GetChild(typeItem.Text);

        //            foreach (var subItem in typeItem.TreeNodes)
        //            {
        //                string subItemPath = typeItemPath + @"\" + subItem.Text + @".txt";
        //                AbstractDataSource dataSource = (AbstractDataSource) subItem.Data;
        //                if (dataSource.Total < 1)
        //                {
        //                    continue;
        //                }

        //                ExtactionSubCategory subCategory = (ExtactionSubCategory) category.GetChild(subItem.Text);

        //                using (FileStream fs = new FileStream(subItemPath, FileMode.Create))
        //                {
        //                    StreamWriter streamWriter = new StreamWriter(fs);
        //                    if (dataSource.Items != null)
        //                    {
        //                        PropertyInfo[] allPropertyInfos = ((Type) dataSource.Type).GetProperties();
        //                        List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
        //                        foreach (var propertyInfo in allPropertyInfos)
        //                        {
        //                            if (propertyInfo.PropertyType == typeof (string))
        //                            {
        //                                propertyInfos.Add(propertyInfo);
        //                            }
        //                        }

        //                        IEnumerable view = dataSource.Items.View;
        //                        foreach (AbstractDataItem dataItem in view)
        //                        {
        //                            foreach (var propertyInfo in propertyInfos)
        //                            {
        //                                object ob = propertyInfo.GetValue(dataItem);
        //                                if (ob == null)
        //                                {
        //                                    continue;
        //                                }
        //                                string content = ob.ToString();
        //                                if (!string.IsNullOrEmpty(content))
        //                                {
        //                                    bool ret = OnDetect(content, validateDataNodes);
        //                                    if (ret)
        //                                    {
        //                                        //ExtactionItem extactionItem = subCategory.AddItem(subItem.Text);
        //                                        //extactionItem.SetActualData(dataItem);
        //                                    }

        //                                    streamWriter.WriteLine(content);
        //                                }
        //                            }
        //                        }
        //                        streamWriter.Close();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
