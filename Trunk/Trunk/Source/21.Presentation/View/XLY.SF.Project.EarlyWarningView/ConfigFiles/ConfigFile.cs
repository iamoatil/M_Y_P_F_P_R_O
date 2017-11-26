using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace XLY.SF.Project.EarlyWarningView
{
    class ConfigFile
    {
        /// <summary>
        /// 配置文件所在目录
        /// </summary>
        protected string Dir { get; set; }

        /// <summary>
        ///  是否已经初始化。没有初始化的话GetAllData返回为null
        /// </summary>
        protected bool IsInitialize;

        public virtual bool Initialize(string dir)
        {
            IsInitialize = false;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            Dir = dir;

            IsInitialize = true;
            return IsInitialize;
        }

        /// <summary>
        /// 获取全部数据。搜索Dir目录下的Xml文件，并且读取文件中RootName节点的数据；返回一个List<SensitiveData>数据
        /// </summary>
        /// <returns></returns>
        public void GetAllData(List<RootNode> list)
        {
            if (!IsInitialize)
            {
                return;
            }
            //从目录Dir中读取数据
            string[] files = Directory.GetFiles(Dir, "*.xml");
            foreach (var file in files)
            {
                XmlDocument xmdDoc = new XmlDocument();
                try
                {
                    xmdDoc.Load(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
                if (xmdDoc.DocumentElement.Name != NodeDefinition.RootName
                    || !xmdDoc.DocumentElement.HasAttribute("Name"))
                {
                    continue;
                }

                //新建RootNode节点
                string rootNodeName = xmdDoc.DocumentElement.GetAttribute("Name");
                RootNode curRootNode = new RootNode() { NodeName = rootNodeName };
                list.Add(curRootNode);

                XmlNodeList categoryNodes = xmdDoc.DocumentElement.ChildNodes;
                Dictionary<string, CategoryNode> categoryNames = new Dictionary<string, CategoryNode>();
                foreach (XmlElement categoryNode in categoryNodes)
                {
                    string categoryName = categoryNode.Name;

                    if (!categoryNames.Keys.Contains(categoryName))
                    {
                        //新建CategoryNode节点
                        CategoryNode node = new CategoryNode() { NodeName = categoryName };
                        curRootNode.Children.Add(node);
                        categoryNames.Add(categoryName, node);
                    }
                    CategoryNode curNode = categoryNames[categoryName];
                    foreach (XmlElement item in categoryNode.ChildNodes)
                    {
                        //新建数据
                        if (item.HasAttribute("Value"))
                        {
                            string value = item.Attributes["Value"].Value;
                            curNode.Children.Add(new DataNode() { Data = new SensitiveData(value) });
                        }
                    }
                }
            }
        }
    }
}
