using LanguageToXls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageToXlsUnit
{
    [TestClass]
    public class LanguageFile_Test
    {
        [TestMethod]
        public void InitializeTest()
        {
            LanguageFile lanFile = new LanguageFile("sdfsf");
            bool ret=lanFile.Initialize("sdfs",0);
           
            if (lanFile.IsInitialized)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ReadAllWordTest()
        {
            LanguageFile lanFile = new LanguageFile(LanguageFile.CnName);
            string file = @"..\..\..\..\Source\21.Presentation\View\XLY.SF.Project.FileBrowingView\Language\Language_Cn.xml";
            file = Path.GetFullPath(file);
            bool ret = lanFile.Initialize(file, file.IndexOf(@"\Source\"));
            if (!lanFile.IsInitialized)
            {
                Assert.Fail();
            }
            lanFile.ReadAllWord();
        }

        [TestMethod]
        public void WriteAllWordTest()
        {
            LanguageFile lanFile = new LanguageFile(LanguageFile.CnName);
            string file = @"..\..\..\..\Source\21.Presentation\View\XLY.SF.Project.FileBrowingView\Language\Language_Cn.xml";
            file = Path.GetFullPath(file);
            bool ret = lanFile.Initialize(file, file.IndexOf(@"\Source\"));
            if (!lanFile.IsInitialized)
            {
                Assert.Fail();
            }
            lanFile.ReadAllWord();           
            lanFile.WriteAllWord("1.txt");
        }
    }
}
