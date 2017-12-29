using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XLY.SF.Project.CaseManagement;

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class CaseManagementTest
    {
        String caseTestPath = @"G:\XLY\SpfData\hj_20171023[025005]\CaseProject.cp";

        [TestMethod]
        public void TestXmlSchema()
        {
            XDocument doc = XDocument.Load(@"CaseProjectTemplate.cp");
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("", "CaseProjectTemplate.xsd");
            doc.Validate(schemaSet, ValidationEventHandler);

        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
        }

        [TestMethod]
        public void TestCreateCase()
        {
            CaseInfo ci = new CaseInfo();
            ci.Name = "hj";
            ci.Number = "123";
            ci.Type = "1";
            ci.Author = "hj";
            Case @case = Case.New(ci, @"G:\XLY\SpfData");
            Console.ReadKey();
            Assert.IsNotNull(@case);
        }

        [TestMethod]
        public void TestOpenCase()
        {
            Case @case = Case.Open(caseTestPath);
            Assert.IsNotNull(@case);
        }

        [TestMethod]
        public void TestChangeCaseName()
        {
            Case @case = Case.Open(caseTestPath);
            @case.CaseInfo.Name = "newName123";
            @case.Update();
            Assert.AreEqual("newName123", @case.Name);
        }

        [TestMethod]
        public void TestDeleteCase()
        {
            Case @case = Case.Open(caseTestPath);
            @case.Delete();
            Assert.IsFalse(@case.Existed);
        }

        [TestMethod]
        public void TestCreateDeviceExtraction()
        {
            CaseInfo ci = new CaseInfo();
            ci.Name = "hj";
            ci.Number = "123";
            ci.Type = "1";
            ci.Author = "hj";
            Case @case = Case.New(ci, @"G:\XLY\SpfData");
            DeviceExtraction de = @case.CreateDeviceExtraction("设备1", "andrion");
            Assert.IsNotNull(de);
            Assert.AreNotEqual(@case.DeviceExtractions.Count(), 0);
        }

        [TestMethod]
        public void TestDeviceExtractionProperties()
        {
            CaseInfo ci = new CaseInfo();
            ci.Name = "hj";
            ci.Number = "123";
            ci.Type = "1";
            ci.Author = "hj";
            Case @case = Case.New(ci, @"G:\XLY\SpfData");
            DeviceExtraction de = @case.CreateDeviceExtraction("设备1","andrion");
            Assert.IsNotNull(de);
            de["SN"] = "1234565";
            Assert.IsTrue(de.Save());
        }

        [TestMethod]
        public void TestDeviceExtractionExtractItem()
        {
            CaseInfo ci = new CaseInfo();
            ci.Name = "hj";
            ci.Number = "123";
            ci.Type = "1";
            ci.Author = "hj";
            Case @case = Case.New(ci, @"G:\XLY\SpfData");

            //相对路径
            DeviceExtraction de = @case.CreateDeviceExtraction("设备1", "andrion相对路径");
            de["Name"] = "hujing";
            de.Updated += De_Updated;
            de.Save();

            Assert.IsNotNull(de);
            ExtractItem ei = de.CreateExtract("minnor", "镜像1相对路径");
            ei.Deleted += Ei_Deleted;
            Assert.IsNotNull(ei);
            ei.Delete();
            //Assert.AreEqual(de.ExtractItems.Count(), 0);
            de.Deleted += De_Deleted;
            de.Delete();
            Console.ReadKey();
            //绝对路径
            //de = @case.CreateDeviceExtraction("andrion绝对路径",directory: @"G:\XLY\SpfData");
            //Assert.IsNotNull(de);
            //ei = de.CreateExtract("minnor", @"G:\XLY\SpfData\镜像1绝对路径");
            //Assert.IsNotNull(ei);
            //ei.Delete();
            //Assert.AreEqual(de.ExtractItems.Count(), 0);
            //de.Delete();

            //Assert.IsFalse(de.Existed);
            @case.Delete();
        }

        private void De_Updated(object sender, EventArgs e)
        {
        }

        private void De_Deleted(object sender, EventArgs e)
        {
        }

        private void Ei_Deleted(object sender, EventArgs e)
        {
        }
    }
}
