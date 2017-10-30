using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XLY.SF.Project.CaseManagement
{
    internal class XmlEmbeddedResourceResolver : XmlResolver
    {
        #region Fields

        public static readonly XmlResolver Instance = new XmlEmbeddedResourceResolver();

        #endregion

        #region Methods

        #region Public

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            String fileName = Path.GetFileName(absoluteUri.ToString());
            return GetResourceStrearm(fileName);
        }

        public static XmlSchemaSet GetXmlSchemaSet(String name)
        {
            XmlSchemaSet ss = new XmlSchemaSet() { XmlResolver = XmlEmbeddedResourceResolver.Instance };
            using (Stream stream = GetResourceStrearm(name))
            {
                ss.Add("", XmlReader.Create(stream));
                return ss;
            }
        }

        public static XDocument GetXmlTemplate(String name)
        {
            using (Stream stream = GetResourceStrearm(name))
            {
                return XDocument.Load(stream);
            }
        }

        #endregion

        #region Private

        private static Stream GetResourceStrearm(String name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            String assemblyName = assembly.GetName().Name;
            return assembly.GetManifestResourceStream($"{assemblyName}.{name}");
        }

        #endregion

        #endregion
    }
}
