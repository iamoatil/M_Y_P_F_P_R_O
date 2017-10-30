using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 案例项目配置。
    /// </summary>
    internal sealed class CPConfiguration
    {
        #region Fields

        private static readonly WeakReference<XmlSchemaSet> SchemaSet = new WeakReference<XmlSchemaSet>(null);

        private readonly XDocument _doc;

        private readonly XElement _itemGroup;

        #endregion

        #region Constructors

        private CPConfiguration(XDocument doc, Boolean isCreate)
        {
            doc.Validate(XmlSchemaSet);
            if (isCreate)
            {
                _itemGroup = doc.Root.Element("ItemGroup");
            }
            else
            {
                _itemGroup = doc.Root.Element("ItemGroup");
                if (_itemGroup == null)
                {
                    _itemGroup = new XElement("ItemGroup");
                    doc.Root.Add(_itemGroup);
                }
            }
            _doc = doc;
        }

        #endregion

        #region Properties

        /// <summary>
        /// XSD架构信息。
        /// </summary>
        private static XmlSchemaSet XmlSchemaSet
        {
            get
            {
                if (!SchemaSet.TryGetTarget(out XmlSchemaSet ss))
                {
                    ss = XmlEmbeddedResourceResolver.GetXmlSchemaSet("CaseProjectTemplate.xsd");
                    SchemaSet.SetTarget(ss);
                }
                return ss;
            }
        }

        /// <summary>
        /// 获取配置中ItemGroup的所有Reference节点。
        /// </summary>
        public IEnumerable<ReferenceItem> ReferenceItems
        {
            get
            {
                if (_itemGroup == null) return new ReferenceItem[0];
                return _itemGroup.Elements().Select(x => new ReferenceItem(x));
            }
        }

        #endregion

        #region Methods

        #region Internal

        /// <summary>
        /// 打开已有的案例项目文件。
        /// </summary>
        /// <param name="file">案例项目文件路径。</param>
        /// <returns>CPConfiguration 类型实例。</returns>
        public static CPConfiguration Open(String file)
        {
            try
            {
                XDocument doc = InnerHelper.Open(file);
                if (doc != null)
                {
                    return new CPConfiguration(doc, false);
                }
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// 创建新的案例项目文件。
        /// </summary>
        /// <param name="caseInfo">案例信息。</param>
        /// <returns>CPConfiguration 类型实例。</returns>
        public static CPConfiguration Create(CaseInfo caseInfo)
        {
            XDocument doc = XmlEmbeddedResourceResolver.GetXmlTemplate("CaseProjectTemplate.cp");
            XElement propertyGroup = doc.Root.Element("PropertyGroup");
            propertyGroup.Element("Id").Value = caseInfo.Id;
            propertyGroup.Element("Name").Value = caseInfo.Name ?? String.Empty;
            propertyGroup.Element("Number").Value = caseInfo.Number ?? String.Empty;
            propertyGroup.Element("Type").Value = caseInfo.Type ?? String.Empty;
            propertyGroup.Element("Author").Value = caseInfo.Author ?? String.Empty;
            caseInfo.Timestamp = DateTime.Now;
            propertyGroup.Element("Timestamp").Value = caseInfo.Timestamp.ToString("s");

            try
            {
                return new CPConfiguration(doc, true);
            }
            catch(FormatException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取案例信息。
        /// </summary>
        /// <returns>案例信息。</returns>
        public RestrictedCaseInfo GetCaseInfo(String path)
        {
            XElement propertyGroup = _doc.Root.Element("PropertyGroup");
            CaseInfo caseInfo = new CaseInfo();
            caseInfo.Id = propertyGroup.Element("Id").Value;
            caseInfo.Name = propertyGroup.Element("Name").Value;
            caseInfo.Number = propertyGroup.Element("Number").Value;
            caseInfo.Type = propertyGroup.Element("Type").Value;
            caseInfo.Author = propertyGroup.Element("Author").Value;
            caseInfo.Timestamp = DateTime.Parse(propertyGroup.Element("Timestamp").Value);
            caseInfo.Path = path;
            return new RestrictedCaseInfo(caseInfo);
        }

        /// <summary>
        /// 设置案例信息。
        /// </summary>
        /// <param name="caseInfo">案例信息。</param>
        public void SetCaseInfo(RestrictedCaseInfo caseInfo)
        {
            XElement propertyGroup = _doc.Root.Element("PropertyGroup");
            propertyGroup.Element("Name").Value = caseInfo.Name;
            propertyGroup.Element("Type").Value = caseInfo.Type;
            propertyGroup.Element("Author").Value = caseInfo.Author;
        }

        /// <summary>
        /// 添加一个 ReferenceItem 项。
        /// </summary>
        /// <param name="reference">ReferenceItem 类型实例。</param>
        /// <returns>如果已存在相同的引用项，则忽略并返回false；否则添加并返回true。</returns>
        public Boolean AddReference(ReferenceItem reference)
        {
            if (reference == null) return false;
            if (!reference.IsValid) return false;
            ReferenceItem found = ReferenceItems.SingleOrDefault(x => x == reference);
            if (found != null) return false;
            _itemGroup.Add(reference.Element);
            return true;
        }

        /// <summary>
        /// 移除一个ReferenceItem 项。
        /// </summary>
        /// <param name="reference">ReferenceItem 类型实例。</param>
        /// <returns>如果该引用项并不存在，则忽略并返回false；否则移除并返回true。</returns>
        public Boolean RemoveReference(ReferenceItem reference)
        {
            if (reference == null) return false;
            ReferenceItem found = ReferenceItems.SingleOrDefault(x => x == reference);
            if (found == null) return false;
            reference.Element.Remove();
            return true;
        }

        /// <summary>
        /// 保存案例项目文件。
        /// </summary>
        /// <param name="path">保存路径。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Save(String path)
        {
            return _doc.Save(XmlSchemaSet, path);
        }

        #endregion

        #endregion
    }
}
