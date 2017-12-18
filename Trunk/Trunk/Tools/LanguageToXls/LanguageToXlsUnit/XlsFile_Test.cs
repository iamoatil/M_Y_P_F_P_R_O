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
    public class XlsFile_Test
    {
        [TestMethod]
        public void GenerateXlsFile()
        {
            List<string> ls = new List<string>();
            ls.Add("我是李涛\tEnglish1\tVirtualPath1");
            ls.Add("Chinese2\tEnglish2\tVirtualPath2");
            ls.Add("我是李涛3\tEnglish3\tVirtualPath3");
            File.WriteAllLines(@"d:\1.xls",ls,Encoding.Default);
        }
    }
}
