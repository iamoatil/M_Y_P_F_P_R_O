/* ==============================================================================
* Description：DetectionManager  
* Author     ：litao
* Create Date：2017/11/23 10:16:23
* ==============================================================================*/

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XLY.SF.Project.EarlyWarningView
{
    class DetectionManager
    {
        #region 单例
        private DetectionManager()
        {
            _rootNodeManager.Children.Add(ConstDefinition.CountrySafety,new RootNode(ConstDefinition.CountrySafety));
            _rootNodeManager.Children.Add(ConstDefinition.PublicSafety, new RootNode(ConstDefinition.PublicSafety));
            _rootNodeManager.Children.Add(ConstDefinition.EconomySafety, new RootNode(ConstDefinition.EconomySafety));
            _rootNodeManager.Children.Add(ConstDefinition.Livehood, new RootNode(ConstDefinition.Livehood) );
            _rootNodeManager.Children.Add(ConstDefinition.Custom, new RootNode(ConstDefinition.Custom) );
        }
        private static DetectionManager _instance = new DetectionManager();
        public static DetectionManager Instance { get { return _instance; } }
        
        #endregion

        private RootNodeManager _rootNodeManager = new RootNodeManager();


        private readonly List<DataNode> _validateDataNodes=new List<DataNode>();

        /// <summary>
        /// 预警配置文件的所在顶层目录
        /// </summary>
        private string _baseDir;

        /// <summary>
        /// 是否已经初始化。对象要初始化后才能使用
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 设置管理
        /// </summary>
        public SettingManager SettingManager { get { return _settingManager; } }
        private readonly SettingManager _settingManager=new SettingManager();

        /// <summary>
        /// 对象要初始化后才能使用
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            _isInitialized = false;          

            _baseDir = Path.GetFullPath(@"EarlyWarningConfig\");
            //读取配置文件
            ConfigFile configFile = new ConfigFile();
            bool ret=configFile.Initialize(_baseDir);
            if(!ret)
            {
                return _isInitialized;
            }
            configFile.GetAllData(_rootNodeManager);
            SetParameter(SettingManager);
            _isInitialized = true;
            return _isInitialized;
        }

        /// <summary>
        /// SettingManager对应 ---》RootNodeManager
        ///  SettingCollection对应 ---》RootNode
        ///  SettingItem对应 ---》CategoryNode
        /// </summary>
        public void SetParameter(SettingManager settingManager)
        {
            //SettingManager对应-- -》RootNodeManager
            if (!SettingManager.IsEnable)
            {
                return;
            }
            
            foreach (SettingCollection rootSetting in settingManager.Items)
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
                        _validateDataNodes.AddRange(categoryNode.DataList);
                    }
                }
            }
        }

        /// <summary>
        /// 检测
        /// </summary>
        public void Detect()
        {
            ExtactionItemParser parser = new ExtactionItemParser();
            parser.DetectAction += Parser_DetectAction;
            parser.Detect();
        }

        private bool Parser_DetectAction(string content)
        {
            foreach (var item in _validateDataNodes)
            {
                if(item.Data.Value == content)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
