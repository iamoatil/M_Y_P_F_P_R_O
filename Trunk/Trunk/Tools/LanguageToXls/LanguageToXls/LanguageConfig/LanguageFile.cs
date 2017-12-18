using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;

namespace LanguageToXls
{
    class LanguageFile
    {
        public const string CnName = "Language_Cn.xml";
        public const string EnName = "Language_En.xml";

        private const string RootNodeName = "LanguageResource";
        private const char VirtualPathSepartor = '/';

        public LanguageFile(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 文件的名字
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 是否已经初始化。只有初始化成功之后才执行其他逻辑
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 源代码主目录
        /// </summary>
        public string AbsolutePath { get; private set; }

        /// <summary>
        /// 相对路径在绝对路径中的起始索引
        /// </summary>
        private int _relativePathStartedIndex = 0;

        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelativePath
        {
            get
            {
                if (!IsInitialized)
                {
                    return string.Empty;
                }
                else
                {
                    return AbsolutePath.Substring(_relativePathStartedIndex);
                }
            }
        }

        /// <summary>
        /// 源代码中所有Word
        /// </summary>
        public Dictionary<string,LanguageWord> LanguageWordDic
        {
            get { return _languageWordDic; }
        }
        private Dictionary<string, LanguageWord> _languageWordDic = new Dictionary<string, LanguageWord>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize(string file, int startedIndex)
        {
            _relativePathStartedIndex = startedIndex;
            IsInitialized = InnerInitialize(file);
            return IsInitialized;
        }

        private bool InnerInitialize(string file)
        {
            //验证源代码目录的有效性
            file = Path.GetFullPath(file);
            if (!File.Exists(file))
            {
                return false;
            }
            AbsolutePath = file;
            
            return true;
        }

        /// <summary>
        /// 读取所有字词
        /// </summary>
        public void ReadAllWord()
        {
            XmlDocument xmdDoc = new XmlDocument();
            try
            {
                xmdDoc.Load(AbsolutePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            if (xmdDoc.DocumentElement == null
                || xmdDoc.DocumentElement.Name != RootNodeName)
            {
                return;
            }
            //获取子节点。
            ReadXmlNode(xmdDoc.DocumentElement,"");
        }

        private void ReadXmlNode(XmlNode xmlNode,string virtualPath)
        {
            if(xmlNode == null)
            {
                return;
            }

            if(!(xmlNode is XmlElement))
            {
                string str=xmlNode.InnerText;
                if(LanguageWordDic.Keys.Contains(virtualPath))
                {
                    return;
                }
                LanguageWord word = new LanguageWord();
                word.Initialize(virtualPath, str);
                LanguageWordDic.Add(word.VirtualPath,word);
            }
            else
            {
                XmlNodeList childrenNodes  =xmlNode.ChildNodes;
                virtualPath += VirtualPathSepartor+xmlNode.Name;
                foreach (XmlNode node in childrenNodes)
                {
                    ReadXmlNode(node, virtualPath);
                }
            }
        }

        /// <summary>
        /// 把所有字词写入文件中
        /// </summary>
        public void WriteAllWord(string path=null)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建Xml声明部分，即<?xml version="1.0" encoding="utf-8" ?>
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
            //创建根节点
            XmlElement rootNode = xmlDoc.CreateElement(RootNodeName);
            xmlDoc.AppendChild(rootNode);

            foreach (var word in LanguageWordDic.Values)
            {
                string virtualPath = word.VirtualPath.TrimStart(VirtualPathSepartor);
                int index = virtualPath.IndexOf(VirtualPathSepartor);
                virtualPath = virtualPath.Substring(index+1);

                XmlNode node = CreateElement(rootNode, virtualPath);
                string content = word.Content;
                if(string.IsNullOrWhiteSpace(content))
                {
                    content = " ";
                }
                node.InnerText = content;
            }

            if (path==null)
            {
                path = AbsolutePath;
            }

            if(File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                fileInfo.IsReadOnly = false;
            }
            xmlDoc.Save(path);
        }

        /// <summary>
        /// 根据xpath创建xmlDoc节点
        /// </summary>
        /// <param name="xpath"></param>
        private XmlNode CreateElement(XmlNode parentNode, string xpath)
        {
            xpath = xpath.TrimEnd(VirtualPathSepartor);

            XmlNode childNode = parentNode.SelectSingleNode(xpath);
            if(childNode != null)
            {
                return childNode;
            }
            
            while(true)
            {
                xpath = xpath.TrimStart(VirtualPathSepartor);
                int index = xpath.IndexOf(VirtualPathSepartor);
                if (index > 0)
                {
                    string name = xpath.Substring(0, index);

                    childNode = null;
                    foreach (XmlNode node in parentNode.ChildNodes)
                    {
                        if(node.Name == name)
                        {
                            childNode = node;
                            break;
                        }
                    }

                    if (childNode == null)
                    {
                        childNode = parentNode.OwnerDocument.CreateElement(name);
                        parentNode.AppendChild(childNode);
                    }                    

                    parentNode = childNode;
                    xpath = xpath.Substring(index + 1);
                }
                else
                {
                    childNode = null;
                    foreach (XmlNode node in parentNode.ChildNodes)
                    {
                        if (node.Name == xpath)
                        {
                            childNode = node;
                            break;
                        }
                    }

                    if (childNode == null)
                    {
                        childNode = parentNode.OwnerDocument.CreateElement(xpath);
                        parentNode.AppendChild(childNode);
                    }
                    return childNode;
                }
            }    
        }

        /// <summary>
        /// 同步语言文件的节点
        /// 使用langFile同步本文件的节点，使之和langFile的节点一致
        /// </summary>
        public void Update(LanguageFile langFile)
        {
            Dictionary<string, LanguageWord> tmpWords = new Dictionary<string, LanguageWord>();
            foreach (var word in langFile.LanguageWordDic)
            {
                if (this.LanguageWordDic.ContainsKey(word.Key))
                {
                    tmpWords.Add(word.Key, LanguageWordDic[word.Key]);
                }
                else
                {
                    LanguageWord newWord = new LanguageWord();
                    newWord.Initialize(word.Key, "");
                    tmpWords.Add(word.Key, newWord);
                }
            }

            LanguageWordDic.Clear();
            foreach (var item in tmpWords)
            {
                LanguageWordDic.Add(item.Key, item.Value);
            }
        }

    }
}
