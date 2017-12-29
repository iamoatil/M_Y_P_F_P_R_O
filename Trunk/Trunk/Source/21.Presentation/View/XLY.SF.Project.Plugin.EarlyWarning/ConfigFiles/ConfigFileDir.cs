using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace XLY.SF.Project.EarlyWarningView
{
    class ConfigFileDir
    {
        /// <summary>
        /// 配置文件所在目录
        /// </summary>
        protected string Dir { get; set; }

        /// <summary>
        ///  是否已经初始化。没有初始化的话GetAllData返回为null
        /// </summary>
        public  bool IsInitialize { get; private set; }    

        public virtual bool Initialize(string dir)
        {
            IsInitialize = false;

            if (!Directory.Exists(dir))
            {
                return false;
            }
            Dir = dir;

            IsInitialize = true;
            return IsInitialize;
        }

        /// <summary>
        /// 获取全部数据。搜索Dir目录下的Xml文件，并且读取文件中RootName节点的数据；返回一个List<SensitiveData>数据
        /// </summary>
        /// <returns></returns>
        public void GetAllData(RootNodeManager rootNodeManager)
        {
            if (!IsInitialize)
            {
                return;
            }
            //从目录Dir中读取数据
            string[] files = Directory.GetFiles(Dir, "*.xml");
            foreach (var file in files)
            {
                ConfigFile configFile = new ConfigFile();
                configFile.Initialize(file);                
                configFile.GetAllData(rootNodeManager);
            }
        }

        /// <summary>
        /// 获取全部配置文件
        /// </summary>
        /// <returns></returns>
        public List<ConfigFile> GetAllFiles()
        {
            List<ConfigFile> ls = new List<ConfigFile>();
            if (!IsInitialize)
            {
                return ls;
            }
            //从目录Dir中读取数据
            string[] files = Directory.GetFiles(Dir, "*.xml");
            foreach (var file in files)
            {
                ConfigFile configFile = new ConfigFile();
                configFile.Initialize(file);
                if(configFile.IsInitialized)
                {
                    ls.Add(configFile);
                }
            }
            return ls;
        }
    }
}
