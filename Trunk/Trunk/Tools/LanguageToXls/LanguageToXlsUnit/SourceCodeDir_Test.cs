using System;
using System.Collections.Generic;
using System.IO;
using LanguageToXls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LanguageToXlsUnit
{
    [TestClass]
    public class SourceCodeDir_Test
    {
        [TestMethod]
        public void GetOriginalLanguageFilesTest()
        {
            SourceCodeDir sourceCodeDir = new SourceCodeDir();
            List<string> dirs = sourceCodeDir.GetLanguageDirs(@"..\..\..\..\Source\");

            string file = Path.GetFullPath(@"..\..\..\..\Source\11.Service\11.Domains\XLY.SF.Project.Domains\Language\");
            if (!dirs.Contains(file))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void InitializeTest()
        {
            SourceCodeDir sourceCodeDir = new SourceCodeDir();
           sourceCodeDir.Initialize(@"..\..\..\..\Source\");
        }
    }
}