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
                string directoty = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoty))
                {
                    Directory.CreateDirectory(directoty);
                }
                //正常数据的文件
                string content = @"<FileMd5Collection>"
                                + @" <Category Name = ""涉黄"" >"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty1"" />"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty2"" />"
                                + @" </Category>"
                                + @" <Category Name = ""涉毒"" >"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty3"" />"
                                + @" </Category >"
                                + @"</FileMd5Collection>";      
                File.WriteAllText(path, content);
                //空文件
                path = @"TestFiles\Md5FileEmpty.xml";
                File.WriteAllText(path, "");
                //节点不正确的文件
                path = @"TestFiles\Md5FileNodeError1.xml";
                content = @"<FileMd5Collection1>"
                                + @" <Category Name = ""涉黄"" >"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty1"" />"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty2"" />"
                                + @" </Category>"
                                + @" <Category Name = ""涉毒"" >"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty3"" />"
                                + @" </Category >"
                                + @"</FileMd5Collection1>";
                File.WriteAllText(path, content);

                path = @"TestFiles\Md5FileNodeError2.xml";
                content = @"<FileMd5Collection>"
                                + @" <Category>"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty1"" />"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty2"" />"
                                + @" </Category>"
                                + @" <Category Name = ""涉毒"" >"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty3"" />"
                                + @" </Category >"
                                + @"</FileMd5Collection>";
                File.WriteAllText(path, content);

                path = @"TestFiles\Md5FileNodeError3.xml";
                content = @"<FileMd5Collection>"
                                + @" <Category Name = ""涉黄"" >"
                                + @"     <FileMd512 Value = ""acb23tyrt7823rty1"" />"
                                + @"     <FileMd51 Value = ""acb23tyrt7823rty2"" />"
                                + @" </Category>"
                                + @" <Category Name = ""涉毒"" >"
                                + @"     <FileMd5 Value = ""acb23tyrt7823rty3"" />"
                                + @" </Category >"
                                + @"</FileMd5Collection>";
                File.WriteAllText(path, content);
            }

            Md5ConfigFileDir dir = new Md5ConfigFileDir();
            dir.Initialize(@"TestFiles\");
            List<SensitiveData> ls = dir.GetAllData();
        }
    }
}
