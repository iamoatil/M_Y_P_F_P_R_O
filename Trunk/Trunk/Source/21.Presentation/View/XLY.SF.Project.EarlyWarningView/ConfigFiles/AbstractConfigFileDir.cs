using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/* ==============================================================================
* Description：AbstractConfigFileDir  
* Author     ：litao
* Create Date：2017/11/22 17:44:01
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    abstract class AbstractConfigFileDir
    {
        /// <summary>
        /// 配置数据的根节点名字。数据用xml配置在文件中，每个文件都只有一个根节点名字
        /// </summary>
        protected abstract string RootName { get; }

        protected string CategoryName = "Category";

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
        public List<SensitiveData> GetAllData()
        {
            if (IsInitialize)
            {
                List<SensitiveData> list = new List<SensitiveData>();
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
                    if (xmdDoc.DocumentElement.Name != RootName)
                    {
                        continue;
                    }
                    XmlNodeList categoryNodes = xmdDoc.DocumentElement.ChildNodes;
                    foreach (XmlElement categoryNode in categoryNodes)
                    {
                        if (categoryNode.Name == CategoryName
                            && categoryNode.HasAttribute("Name"))
                        {
                            string categoryName = categoryNode.Attributes["Name"].Value;
                            foreach (XmlElement item in categoryNode.ChildNodes)
                            {
                                if(item.HasAttribute("Value"))
                                {
                                    string value = item.Attributes["Value"].Value;
                                    list.Add(new SensitiveData(categoryName, value));
                                }                                
                            }
                        }
                    }
                }
                return list;
            }
            return null;
        }
    }
}
