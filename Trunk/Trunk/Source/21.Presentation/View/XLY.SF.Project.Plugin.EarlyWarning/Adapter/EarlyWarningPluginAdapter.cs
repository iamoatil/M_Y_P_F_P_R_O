using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLY.SF.Project.DataDisplayView.ViewModel;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Models;
using XLY.SF.Project.Plugin.Adapter;

/* ==============================================================================
* Description：EarlyWarningPluginAdapter  
* Author     ：litao
* Create Date：2017/12/2 10:14:15
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    class EarlyWarningPluginAdapter
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 此适配器支持的所有插件
        /// </summary>
        public IEnumerable<AbstractEarlyWarningPlugin> Plugins { get; private set; }

        /// <summary>
        /// 关键字预警插件
        /// </summary>
        public AbstractEarlyWarningPlugin KeyWordPlugin { get; private set; }

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
            new ExtactionCategoryCollectionManager() { Name = "智能检视" };

        public ConfigDataToDB ConfigDbManager { get { return _configDbManager; } }
        ConfigDataToDB _configDbManager = new ConfigDataToDB();

        public void Initialize(IRecordContext<Models.Entities.Inspection> setting)
        {
            if(IsInitialized)
            {
                return;
            }

            Plugins = PluginAdapter.Instance.GetPluginsByType<EarlyWarningPluginInfo>(PluginType.SpfEarlyWarning).ToList().ConvertAll(p => (AbstractEarlyWarningPlugin)p.Value);
            KeyWordPlugin = Plugins.Where(it => it.PluginInfo is KeyWordEarlyWarningPluginInfo).FirstOrDefault();

            ConfigDataFilter.Initialize();
            ConfigDataFilter.Setting = setting;

            ConfigDbManager.Initialize();

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
            //把过滤后的配置写到数据库文件中
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

            AbstractEarlyWarningPlugin plugin = Plugins.Where(p => ((EarlyWarningPluginInfo)p.PluginInfo).Match(dataSource)).FirstOrDefault(); 
            if(plugin != null)
            {
                EarlyWarningPluginArgument arg = new EarlyWarningPluginArgument(ds, dataNodes);
                plugin.Execute(arg,null);
            }
            if(KeyWordPlugin != null)
            {
                EarlyWarningPluginArgument arg = new EarlyWarningPluginArgument(ds, dataNodes);
                KeyWordPlugin.Execute(arg,null);
            }
            return;            
        }

        /// <summary>
        /// 采用两表联合查询的方式验证预警数据
        /// </summary>
        /// <param name="ds"></param>
        private void Match(DeviceDataSource ds)
        {
            //读取数据库中JsonColumnName列，并且匹配
            IDataSource dataSource = ds.DataSource;

            AbstractEarlyWarningPlugin plugin = Plugins.Where(p => ((EarlyWarningPluginInfo)p.PluginInfo).Match(dataSource)).FirstOrDefault();
            if (plugin != null)
            {
                plugin.Execute(dataSource);
            }
            if (KeyWordPlugin != null)
            {
                KeyWordPlugin.Execute(dataSource);
            }
            return;
        }
    }
}
