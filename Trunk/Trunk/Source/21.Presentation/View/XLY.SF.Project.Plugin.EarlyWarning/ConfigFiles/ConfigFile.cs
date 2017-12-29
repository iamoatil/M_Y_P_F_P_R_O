using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace XLY.SF.Project.EarlyWarningView
{
    class ConfigFile
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        string _filePath;

        /// <summary>
        ///  文件对应的XmlDocument
        /// </summary>
        XmlDocument _xmlDoc = new XmlDocument();

        /// <summary>
        ///  是否已经初始化。没有初始化的话GetAllData返回为null
        /// </summary>
        public bool IsInitialized { get; private set; }       
       
        /// <summary>
        /// 此文件中定义的类型名字
        /// </summary>
        public string RootNodeName { get; private set; }

        /// <summary>
        /// 此文件中定义的类型的ID
        /// </summary>
        public string SensitiveId { get; private set; }

        public virtual bool Initialize(string filePath)
        {
            IsInitialized = false;

            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                _xmlDoc.Load(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            if (_xmlDoc.DocumentElement == null
                || _xmlDoc.DocumentElement.Name != ConstDefinition.RootName
                || !_xmlDoc.DocumentElement.HasAttribute("Name")
                || !_xmlDoc.DocumentElement.HasAttribute("SensitiveId"))
            {
                return false;
            }
            RootNodeName = _xmlDoc.DocumentElement.GetAttribute("Name");
            SensitiveId = _xmlDoc.DocumentElement.GetAttribute("SensitiveId");

            _filePath = filePath;
            IsInitialized = true;
            return IsInitialized;
        }

        public void GetAllData(RootNodeManager rootNodeManager)
        {
            if(!IsInitialized)
            {
                return;
            }
            
            //新建、获取RootNode节点curRootNode
            string rootNodeName = RootNodeName;
            string sensitiveId = SensitiveId;
            if (!rootNodeManager.Children.Keys.Contains(rootNodeName))
            {
                rootNodeManager.Children.Add(rootNodeName, new RootNode(rootNodeName, Path.GetFullPath(_filePath)));
            }
            RootNode curRootNode = (RootNode)rootNodeManager.Children[rootNodeName];

            XmlNodeList categoryNodes = _xmlDoc.DocumentElement.ChildNodes;
            foreach (XmlElement categoryNode in categoryNodes)
            {
                string categoryName = categoryNode.Name;
                if (!curRootNode.Children.Keys.Contains(categoryName))
                {
                    CategoryNode node = new CategoryNode() { NodeName = categoryName };
                    curRootNode.Children.Add(categoryName, node);
                }
                CategoryNode curCategoryNode = (CategoryNode)curRootNode.Children[categoryName];

                foreach (XmlElement item in categoryNode.ChildNodes)
                {
                    //新建、获取DataNode节点
                    if (item.HasAttribute("Value"))
                    {
                        string value = item.Attributes["Value"].Value;
                        DataNode dataNode = new DataNode() { SensitiveData = new SensitiveData(rootNodeName, categoryName, sensitiveId, value) };
                        curCategoryNode.DataList.Add(dataNode);
                        rootNodeManager.ValidateDataNodes.Add(dataNode);
                    }
                }
            }
        }

        /// <summary>
        /// 设置Warning的个数
        /// </summary>
        /// <param name="count"></param>
        public void SetWarningCount(int count)
        {
            if (!IsInitialized)
            {
                return;
            }
            string attributeName = "WarningCount";
            XmlElement rootElement = _xmlDoc.DocumentElement;
            if(!rootElement.HasAttribute(attributeName))
            {
                XmlAttribute attribute = _xmlDoc.CreateAttribute(attributeName);
                attribute.Value = count.ToString();
                rootElement.Attributes.Append(attribute);
            }
            else
            {
                rootElement.Attributes[attributeName].Value= count.ToString();
            }
            
            _xmlDoc.Save(_filePath);
        }
    }
}
