using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLY.SF.Project.Domains;
using XLY.SF.Project.EarlyWarningView;

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class FileMd5DbWriter_Test
    {
        [TestMethod]
        public void GenerateMd5Test()
        {
            //先获取D:\Test\下文件的md5（当前已经存在了）
            FileMd5Generator fileMd5Generator = new FileMd5Generator();
            fileMd5Generator.GenerateMd5(@"\splash2",@"D:\Test");
            List<FileMd5DataItem> fileMd5List=fileMd5Generator.Md5List;
            File.WriteAllLines(@"d:\Test\fileMd5.txt",fileMd5List.Select(i=>i.ToString()));

            //测试生成Ds的功能
            FileMd5DbWriter writer = new FileMd5DbWriter();
            writer.Initialize(@"D:\XLY\SpfData\手里全部提取_20171204[111147]\H60-L01_20171204[111149]\自动提取\");
            writer.GenerateDsFile();
            writer.TestDeserialize();
            //测试写入db的功能
            writer.WriteDb(fileMd5List);
        }
    }
}
