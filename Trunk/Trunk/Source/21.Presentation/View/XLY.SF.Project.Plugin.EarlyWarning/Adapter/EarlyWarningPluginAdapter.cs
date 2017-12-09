using System.Collections.Generic;
using System.Linq;
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
    public class EarlyWarningPluginAdapter
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 此适配器支持的所有插件
        /// </summary>
        internal IEnumerable<AbstractEarlyWarningPlugin> Plugins { get; private set; }

        /// <summary>
        /// 关键字预警插件
        /// </summary>
        internal AbstractEarlyWarningPlugin KeyWordPlugin { get; private set; }

        /// <summary>
        /// 配置数据的过滤
        /// </summary>
        private readonly ConfigDataFilter ConfigDataFilter = new ConfigDataFilter();          

        public void Initialize(IRecordContext<Models.Entities.Inspection> setting)
        {
            if(IsInitialized)
            {
                return;
            }

            Plugins = PluginAdapter.Instance.GetPluginsByType<EarlyWarningPluginInfo>(PluginType.SpfEarlyWarning).ToList().ConvertAll(p => (AbstractEarlyWarningPlugin)p.Value);
            KeyWordPlugin = Plugins.Where(it => it.PluginInfo is KeyWordEarlyWarningPluginInfo).FirstOrDefault();

            ConfigDataFilter.Initialize(setting);
            //先根据设置（读取数据库获得设置选项），过滤配置文件中的内容
            ConfigDataFilter.UpdateValidateData();  

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

            DbFromConfigData configDbManager = new DbFromConfigData();
            //ConfigDataToDB
            configDbManager.Initialize(sourceDir);
            //把过滤后的配置写到数据库文件中
            configDbManager.GenerateDbFile(ConfigDataFilter.ValidateDataNodes.Select(it => it.SensitiveData));

            //读取提取的数据
            DeviceDataParser parser = new DeviceDataParser();
            parser.LoadDeviceData(sourceDir);
            List<DeviceDataSource> dataSources = parser.DataSources;
            //检测提取的数据
            foreach (var item in dataSources)
            {
                Match(item, configDbManager);
            }
        }

        private void Match(DeviceDataSource ds, DbFromConfigData configDbManager)
        {
            IDataSource dataSource = ds.DataSource;           

            //寻找恰当的插件处理
            AbstractEarlyWarningPlugin plugin = Plugins.Where(p => ((EarlyWarningPluginInfo)p.PluginInfo).Match(dataSource)).FirstOrDefault(); 
            if(plugin != null)
            {
                EarlyWarningPluginArgument arg = new EarlyWarningPluginArgument(ds, configDbManager);
                plugin.Execute(arg,null);
            }
            //所有数据都经过关键字处理
            if(KeyWordPlugin != null)
            {
                EarlyWarningPluginArgument arg = new EarlyWarningPluginArgument(ds, configDbManager);
                KeyWordPlugin.Execute(arg,null);
            }
            return;            
        }
    }
}
