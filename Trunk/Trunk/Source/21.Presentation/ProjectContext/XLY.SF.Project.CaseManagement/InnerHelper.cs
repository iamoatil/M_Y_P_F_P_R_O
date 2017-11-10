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
    internal static class InnerHelper
    {
        public static XDocument Open(String file)
        {
            if (String.IsNullOrWhiteSpace(file)) return null;
            FileStream stream = null;
            try
            {
                if (!File.Exists(file)) return null;
                stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return XDocument.Load(stream);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                stream?.Close();
            }
        }

        public static Boolean Save(this XDocument doc, XmlSchemaSet schemaSet,String path)
        {
            try
            {
                doc.Validate(schemaSet);
                String directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                FileStream stream = File.Create(path);
                doc.Save(stream);
                stream.Close();
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void Validate(this XDocument doc, XmlSchemaSet schemaSet)
        {
            doc.Validate(schemaSet, (o, m) =>
            {
                throw new FormatException("Invalid value in config file.", m.Exception);
            });
        }
    }
}
