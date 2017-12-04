/* ==============================================================================
* Description：DetectionManager  
* Author     ：litao
* Create Date：2017/11/23 10:16:23
* ==============================================================================*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLY.SF.Project.DataDisplayView.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Models;

namespace XLY.SF.Project.EarlyWarningView
{
    internal class DetectionManager
    {
        /// <summary>
        /// 配置数据的过滤
        /// </summary>
        private readonly ConfigDataFilter ConfigDataFilter = new ConfigDataFilter();

        /// <summary>
        /// 检测的结果放于CategoryManager中
        /// </summary>
        public ExtactionCategoryCollectionManager CategoryManager
        {
            get { return _categoryManager; }
        }

        private readonly ExtactionCategoryCollectionManager _categoryManager =
            new ExtactionCategoryCollectionManager() {Name = "智能检视"};        
        
        public ConfigDataToDB ConfigDbManager { get { return _configDbManager; } }
        ConfigDataToDB _configDbManager = new ConfigDataToDB();


        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        public void Initialize(IRecordContext<Models.Entities.Inspection> setting)
        {
            if (IsInitialized)
            {
                return;
            }
            ConfigDataFilter.Initialize();
            ConfigDataFilter.Setting = setting;
            IsInitialized = true;
        }

        /// <summary>
        /// 检测
        /// </summary>
        public void Detect(string sourceDir)
        {
            if (!IsInitialized)
            {
                return;
            }
            //先根据设置（读取数据库获得设置选项），过滤配置文件中的内容
            ConfigDataFilter.UpdateValidateData();
            ConfigDbManager.GenerateDbFile(ConfigDataFilter.ValidateDataNodes.Select(it => it.SensitiveData));
            
            //读取提取的数据
            DeviceDataParser parser = new DeviceDataParser();
            parser.LoadDeviceData(sourceDir);
            List<DeviceDataSource> dataSources = parser.DataSources;
            //检测提取的数据
            foreach (var item in dataSources)
            {
                Match(item, ConfigDataFilter.ValidateDataNodes);
            }            
        }

        
        private void Match(DeviceDataSource ds, List<DataNode> dataNodes)
        {
            //读取数据库中JsonColumnName列，并且匹配
            IDataSource dataSource = ds.DataSource;
            if (dataSource.Items == null)
            {
                return;
            }

            //dataSource.GetType();
            string dir = Path.GetDirectoryName(Path.GetDirectoryName(ds.DsFilePath));
            string extactionName = dir.Substring(dir.LastIndexOf("\\") + 1);

            ExtactionCategoryCollection categoryCollection = (ExtactionCategoryCollection)_categoryManager.GetChild(extactionName);
            ExtactionCategory category = (ExtactionCategory)categoryCollection.GetChild(dataSource.PluginInfo.Group);
            ExtactionSubCategory subCategory = (ExtactionSubCategory)category.GetChild(dataSource.PluginInfo.Name);            

            foreach (DataNode dataNode in dataNodes)
            {
                string cmd = string.Format("{1} like '%{2}%'", dataSource.Items.DbTableName, SqliteDbFile.JsonColumnName, dataNode.SensitiveData.Value);
                IEnumerable<dynamic> result = dataSource.Items.FilterByCmd<dynamic>(cmd);
                foreach (AbstractDataItem item in result)
                {
                    item.SensitiveId = dataNode.SensitiveData.SensitiveId;
                }
            }           
        }        
    }
}
