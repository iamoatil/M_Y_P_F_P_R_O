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
        private static string _fileMd5Name = "FileMd5";
        private static string _netAddressName = "NetAddress";
        private static string _keyWordName = "KeyWord";
        private static string _appName = "App";
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
            md5ConfigFileDir.Initialize(_baseDir+@"\FilMd5Config\");
            Md5Detection md5Detection = new Md5Detection();
            md5Detection.Initialize(md5ConfigFileDir.GetAllData());
            _detectionDic.Add(_fileMd5Name, md5Detection);
            //网址敏感初始化
            //应用敏感初始化
            //关键字敏感初始化
            
            _isInitialized = true;
            return _isInitialized;
        }
    }
}
