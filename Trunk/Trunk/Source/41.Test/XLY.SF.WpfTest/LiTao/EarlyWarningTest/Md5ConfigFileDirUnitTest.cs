using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using XLY.SF.Project.EarlyWarningView;

namespace EarlyWarningTest
{
    [TestClass]
    public class Md5ConfigFileDirTest
    {
        [TestMethod]
        public void GetAllData()
        {
            string path = @"TestFiles\Md5File.xml";
            if (!File.Exists(path))
            {
                string content = @"<FileMd5Collection >"
                                + @" <Category Name = ""涉黄"" >"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty1"" />"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty2"" />"
                                + @" </Category>"
                                + @" <Category Name = ""涉毒"" >"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty3"" />"
                                + @" </Category >"
                                + @"</FileMd5Collection>";
                string directoty = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoty))
                {
                    Directory.CreateDirectory(directoty);
                }
                File.WriteAllText(path, content);
            }

            Md5ConfigFileDir dir = new Md5ConfigFileDir();
            dir.Initialize(@"TestFiles\");
            List<SensitiveData> ls = dir.GetAllData();
        }
    }
}
