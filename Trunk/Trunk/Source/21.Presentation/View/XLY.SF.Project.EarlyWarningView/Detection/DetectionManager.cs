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

        }
        private static DetectionManager _instance = new DetectionManager();
        public static DetectionManager Instance { get { return _instance; } }
        
        #endregion

        private Dictionary<string, IDetection> _detectionDic = new Dictionary<string, IDetection>();
        private List<RootNode> _allData = new List<RootNode>();
        private List<RootNode> _validateData = new List<RootNode>();
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
            configFile.GetAllData(_allData);

            _isInitialized = true;
            return _isInitialized;
        }

        public void SetParameter(SettingManager settingManager)
        {
            _validateData.Clear();
            if (!SettingManager.IsEnable)
            {               
                return;
            }
            List<string> rootNames= _allData.Select(it=>it.NodeName).ToList();
            foreach (SettingCollection rootSetting in settingManager.Items)
            {
                bool rootEnable = rootSetting.IsEnable;
                string rootName = rootSetting.Name;
                if(rootEnable
                   && rootNames.Contains(rootName))
                {
                    var rootNodes = _allData.Where(it => it.NodeName == rootName);                    
                    if(rootNodes.Count() < 1)
                    {
                        continue;
                    }
                    RootNode rootNode = rootNodes.ElementAt(0);
                    RootNode newRootNode = new RootNode(rootNode);
                    _validateData.Add(newRootNode);

                    List<string> nodeNames = rootNode.Children.Select(it => it.NodeName).ToList();
                    foreach (SettingItem item in rootSetting.Items)
                    {
                        bool nodeEnable = item.IsEnable;
                        string nodeName = item.Name;
                        if (nodeEnable
                           && nodeNames.Contains(rootName))
                        {
                            var nodes = rootNode.Children.Where(it => it.NodeName == rootName);
                            if (nodes.Count() < 1)
                            {
                                continue;
                            }

                            //CategoryNode node = nodes.ElementAt(0);
                            //CategoryNode newNode = new RootNode(rootNode);
                            _validateData.Add(newRootNode);
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
            foreach (var item in _detectionDic)
            {
                SensitiveData data= item.Value.Detect(content);
                if(data == null)
                {
                    continue;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
