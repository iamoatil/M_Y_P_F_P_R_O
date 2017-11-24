/* ==============================================================================
* Description：EarlyWarning  
* Author     ：litao
* Create Date：2017/11/23 10:16:23
* ==============================================================================*/

using System.Collections.Generic;
using System.IO;

namespace XLY.SF.Project.EarlyWarningView
{
    class EarlyWarning
    {
        private readonly static string _fileMd5Name = "FileMd5";
        private readonly static string _netAddressName = "NetAddress";
        private readonly static string _keyWordName = "KeyWord";
        private readonly static string _appName = "App";

        private Dictionary<string, IDetection> _detectionDic = new Dictionary<string, IDetection>();

        /// <summary>
        /// 预警配置文件的所在顶层目录
        /// </summary>
        private string _baseDir;

        /// <summary>
        /// 是否已经初始化。对象要初始化后才能使用
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 对象要初始化后才能使用
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            _isInitialized = false;

            _baseDir = Path.GetFullPath(@"EarlyWarning\");
            //Md5敏感初始化
            Md5ConfigFileDir md5ConfigFileDir = new Md5ConfigFileDir();
            md5ConfigFileDir.Initialize(string.Format(@"{0}{1}Config\", _baseDir, _fileMd5Name));
            Md5Detection md5Detection = new Md5Detection() { Name = _fileMd5Name };
            md5Detection.Initialize(md5ConfigFileDir.GetAllData());
            _detectionDic.Add(_fileMd5Name, md5Detection);
            //应用敏感初始化
            AppNameConfigFileDir appNameConfigFileDir = new AppNameConfigFileDir();
            appNameConfigFileDir.Initialize(string.Format(@"{0}{1}Config\", _baseDir, _appName));
            AppNameDetection appNameDetection = new AppNameDetection() { Name = _appName };
            appNameDetection.Initialize(appNameConfigFileDir.GetAllData());
            _detectionDic.Add(_appName, appNameDetection);
            //关键字敏感初始化
            KeyWordConfigFileDir keyWordConfigFileDir = new KeyWordConfigFileDir();
            keyWordConfigFileDir.Initialize(string.Format(@"{0}{1}Config\", _baseDir, _keyWordName));
            KeyWordDetection keyWordDetection = new KeyWordDetection() { Name = _keyWordName };
            keyWordDetection.Initialize(keyWordConfigFileDir.GetAllData());
            _detectionDic.Add(_keyWordName, keyWordDetection);
            //网址敏感初始化
            NetAddressConfigFileDir netAddressConfigFileDir = new NetAddressConfigFileDir();
            netAddressConfigFileDir.Initialize(string.Format(@"{0}{1}Config\", _baseDir, _netAddressName));
            NetAddressDetection netAddressDetection = new NetAddressDetection() { Name = _netAddressName };
            netAddressDetection.Initialize(netAddressConfigFileDir.GetAllData());
            _detectionDic.Add(_netAddressName, netAddressDetection);

            _isInitialized = true;
            return _isInitialized;
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

            return true;
        }
    }
}
