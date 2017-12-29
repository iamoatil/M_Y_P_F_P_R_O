using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static Boolean Save(this XDocument doc, XmlSchemaSet schemaSet, String path)
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

        public static String GetValidDirectory(String directory)
        {
            String parentDirectory = Path.GetDirectoryName(directory);
            String directoryName = Path.GetFileName(directory);
            String fullPath = directory;
            IEnumerable<String> items = null;
            try
            {
                items = Directory.EnumerateDirectories(parentDirectory, $"{directoryName}*");
            }
            catch (DirectoryNotFoundException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (IOException ex)
            {
                throw ex;
            }
            Int32 i = 1;
            while (true)
            {
                if (items.All(x => x != fullPath))
                {
                    break;
                }
                fullPath = Path.Combine(parentDirectory, $"{directoryName}_{i++}");
            }
            return fullPath;
        }
    }
}
