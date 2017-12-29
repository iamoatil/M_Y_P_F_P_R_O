using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DeviceExtractionService
{
    internal static class ExtractItemConfigurationReader
    {
        #region Public

        public static ExtractItem[] GetDowngradingExtractItems()
        {
            String xmlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downgrading", "ExtractItems.xml");
            XElement root = XElement.Load(xmlFile);
            return GetDowngradingExtractItems(root);
        }

        #endregion

        #region Private

        private static ExtractItem[] GetDowngradingExtractItems(XElement root)
        {
            IEnumerable<XElement> categories = root.Elements("Category");
            List<ExtractItem> items = new List<ExtractItem>();
            ExtractItem[] temp;
            foreach (XElement category in categories)
            {
                temp = GetDowngradingExtractItemsByCategory(category);
                items.AddRange(temp);
            }
            return items.ToArray();
        }

        private static ExtractItem[] GetDowngradingExtractItemsByCategory(XElement categoryElement)
        {
            String groupName = categoryElement.Attribute("name").Value;
            IEnumerable<XElement> childs = categoryElement.Elements("App");
            return childs.Select(x => new ExtractItem
            {
                Name = x.Attribute("name").Value,
                GroupName = groupName,
                AppName = x.Attribute("appName").Value,
            }).ToArray();
        }

        #endregion
    }
}
