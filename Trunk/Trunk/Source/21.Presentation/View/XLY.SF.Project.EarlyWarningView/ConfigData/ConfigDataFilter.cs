/* ==============================================================================
* Description：BaseData  
* Author     ：litao
* Create Date：2017/11/27 15:19:30
* ==============================================================================*/

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 配置文件过滤
    /// 原型图上有5种RootNode，此处把这5种直接预定义与此类中，放在RootNodeManager中，并且只读取这5中类型的数据到相应的节点下configFile.GetAllData(_rootNodeManager)。
    /// 根据配置文件更新有效数据的逻辑为把RootNodeManager中的数据放到名为ValidateDataNodes的列表中
    /// </summary>
    class ConfigDataFilter
    {
        private readonly RootNodeManager _rootNodeManager = new RootNodeManager();       

        /// <summary>
        /// 预警配置文件的所在顶层目录
        /// </summary>
        private string _baseDir;

        /// <summary>
        /// 是否已经初始化。对象要初始化后才能使用
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 输出的有效节点列表
        /// </summary>
        public readonly List<DataNode> ValidateDataNodes = new List<DataNode>();

        /// <summary>
        /// 设置管理
        /// </summary>
        public SettingManager SettingManager { get { return _settingManager; } }
        private readonly SettingManager _settingManager = new SettingManager();

        /// <summary>
        /// 对象要初始化后才能使用
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            _isInitialized = false;

            _rootNodeManager.Children.Add(ConstDefinition.CountrySafety, new RootNode(ConstDefinition.CountrySafety));
            _rootNodeManager.Children.Add(ConstDefinition.PublicSafety, new RootNode(ConstDefinition.PublicSafety));
            _rootNodeManager.Children.Add(ConstDefinition.EconomySafety, new RootNode(ConstDefinition.EconomySafety));
            _rootNodeManager.Children.Add(ConstDefinition.Livehood, new RootNode(ConstDefinition.Livehood));
            _rootNodeManager.Children.Add(ConstDefinition.Custom, new RootNode(ConstDefinition.Custom));

            _baseDir = Path.GetFullPath(@"EarlyWarningConfig\");
            //读取配置文件的数据到_rootNodeManager中
            ConfigFile configFile = new ConfigFile();
            bool ret = configFile.Initialize(_baseDir);
            if (!ret)
            {
                return _isInitialized;
            }
            configFile.GetAllData(_rootNodeManager);

            _isInitialized = true;
            return _isInitialized;
        }

        /// <summary>
        /// 根据参数配置，更新有效数据
        /// </summary>
        public void UpdateValidateData()
        {
            if(!_isInitialized)
            {
                return;
            }
            ValidateDataNodes.Clear();
            //SettingManager对应-- -》RootNodeManager
            if (!SettingManager.IsEnable)
            {
                return;
            }

            foreach (SettingCollection rootSetting in SettingManager.Items)
            {
                //  SettingCollection对应 ---》RootNode
                bool rootEnable = rootSetting.IsEnable;
                string rootName = rootSetting.Name;
                if (!rootEnable)
                {
                    continue;
                }
                if (!_rootNodeManager.Children.Keys.Contains(rootName))
                {
                    continue;
                }
                RootNode rootNode = (RootNode)_rootNodeManager.Children[rootName];
                foreach (SettingItem item in rootSetting.Items)
                {
                    //  SettingItem对应 ---》CategoryNode
                    bool categoryEnable = item.IsEnable;
                    string categoryName = item.Name;
                    if (!categoryEnable)
                    {
                        continue;
                    }
                    if (rootNode.Children.Keys.Contains(categoryName))
                    {
                        CategoryNode categoryNode = (CategoryNode)rootNode.Children[categoryName];
                        ValidateDataNodes.AddRange(categoryNode.DataList);
                    }
                }
            }
        }
    }
}
