using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 提取设备配置。
    /// </summary>
    internal sealed class DPConfiguration
    {
        #region Fields

        private static readonly WeakReference<XmlSchemaSet> SchemaSet = new WeakReference<XmlSchemaSet>(null);

        private readonly XDocument _doc;

        private readonly XElement _propertyGroup;

        private readonly XElement _itemGroup;

        #endregion

        #region Constructors

        private DPConfiguration(XDocument doc)
        {
            doc.Validate(XmlSchemaSet);
            Id = doc.Root.Attribute("Id").Value;
            Type = doc.Root.Attribute("Type").Value;
            _propertyGroup = doc.Root.Element("PropertyGroup");
            if (_propertyGroup == null)
            {
                _propertyGroup = new XElement("PropertyGroup");
                doc.Root.Add(_propertyGroup);
            }
            _itemGroup = doc.Root.Element("ItemGroup");
            if (_itemGroup == null)
            {
                _itemGroup = new XElement("ItemGroup");
                doc.Root.Add(_itemGroup);
            }
            _doc = doc;
        }

        private DPConfiguration(String type, XDocument doc)
        {
            Id = Guid.NewGuid().ToString();
            Type = type;
            doc.Root.Attribute("Id").Value = Id;
            doc.Root.Attribute("Type").Value = type;
            _propertyGroup = doc.Root.Element("PropertyGroup");
            _itemGroup = doc.Root.Element("ItemGroup");
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
                    ss = XmlEmbeddedResourceResolver.GetXmlSchemaSet("DeviceExtractionProjectTemplate.xsd");
                    SchemaSet.SetTarget(ss);
                }
                return ss;
            }
        }

        /// <summary>
        /// 唯一标识。
        /// </summary>
        public String Id { get; }

        /// <summary>
        /// 设备类型。
        /// </summary>
        public String Type { get; }

        /// <summary>
        /// 属性组。
        /// </summary>
        public XElement PropertyGroup => _propertyGroup;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 打开已有的提取配置文件。
        /// </summary>
        /// <param name="file">提取配置文件路径。</param>
        /// <returns>DPConfiguration 类型实例。</returns>
        public static DPConfiguration Open(String file)
        {
            try
            {
                XDocument doc = InnerHelper.Open(file);
                if (doc != null)
                {
                    return new DPConfiguration(doc);
                }
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// 创建新的提取配置文件。
        /// </summary>
        /// <param name="type">设备类型。</param>
        /// <returns>DPConfiguration 类型实例。</returns>
        public static DPConfiguration Create(String type)
        {
            XDocument doc = XmlEmbeddedResourceResolver.GetXmlTemplate("DeviceExtractionProjectTemplate.dp");
            return new DPConfiguration(type, doc);
        }

        /// <summary>
        /// 添加一个 ExtractItem 项。
        /// </summary>
        /// <param name="mode">提取模式。</param>
        /// <param name="path">提取所在路径。</param>
        /// <param name="owner">拥有该提取项的设备。</param>
        /// <returns>ExtractItem 类型实例。</returns>
        public ExtractItem AddExtract(String mode, String path, DeviceExtraction owner)
        {
            ExtractItem item = ExtractItem.New(mode, path, owner);
            AddExtract(item, owner);
            return item;
        }

        /// <summary>
        /// 添加一个 ExtractItem 项。
        /// </summary>
        /// <param name="item">提取项。</param>
        /// <param name="owner">拥有该提取项的设备。</param>
        /// <returns>如果已存在相同的提取项，则忽略并返回false；否则添加并返回true。</returns>
        public Boolean AddExtract(ExtractItem item, DeviceExtraction owner)
        {
            if (item == null) return false;
            if (!item.IsValid) return false;
            IEnumerable<ExtractItem> extractItems = GetAllExtractItems(owner);
            ExtractItem found = extractItems.SingleOrDefault(x => x == item);
            if (found != null) return false;
            _itemGroup.Add(item.Element);
            return true;
        }

        /// <summary>
        /// 移除一个 ExtractItem 项。
        /// </summary>
        /// <param name="item">提取项。</param>
        /// <param name="owner">拥有该提取项的设备。</param>
        /// <returns>如果该提取项并不存在，则忽略并返回false；否则移除并返回true。</returns>
        public Boolean RemoveExtract(ExtractItem item, DeviceExtraction owner)
        {
            if (item == null) return false;
            IEnumerable<ExtractItem> extractItems = GetAllExtractItems(owner);
            ExtractItem found = extractItems.SingleOrDefault(x => x == item);
            if (found == null) return false;
            item.Element.Remove();
            return true;
        }

        /// <summary>
        /// 获取属性组中的所有属性名称。
        /// </summary>
        /// <returns>属性名称。</returns>
        public IEnumerable<String> GetPropertyNames()
        {
            return PropertyGroup.Elements().Select(x => x.Name.LocalName);
        }

        /// <summary>
        /// 获取指定属性的值。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>属性值。</returns>
        public String GetProperty(String propertyName)
        {
            return PropertyGroup.Element(propertyName)?.Value;
        }

        /// <summary>
        /// 设置指定属性的值。如果属性不存在，则创建新的属性。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <param name="value">属性值。</param>
        public void SetProperty(String propertyName,String value)
        {
            XElement element = PropertyGroup.Element(propertyName);
            if (element != null)
            {
                element.Value = value;
            }
            else
            {
                PropertyGroup.Add(new XElement(propertyName, value));
            }
        }

        /// <summary>
        /// 保存配置。
        /// </summary>
        /// <param name="path">保存路径。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Save(String path)
        {
            return _doc.Save(XmlSchemaSet, path);
        }

        /// <summary>
        /// 获取配置中ItemGroup的所有Extract节点。
        /// </summary>
        /// <param name="owner">拥有该提取项的设备。</param>
        /// <returns>提取项配置。</returns>
        public IEnumerable<ExtractItem> GetAllExtractItems(DeviceExtraction owner)
        {
            if (_itemGroup == null) return new ExtractItem[0];
            return _itemGroup.Elements().Select(x => new ExtractItem(x, owner));
        }

        #endregion

        #endregion
    }
}
