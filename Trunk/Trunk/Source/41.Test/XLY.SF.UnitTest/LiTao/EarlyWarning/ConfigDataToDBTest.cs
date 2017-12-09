using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using XLY.SF.Project.EarlyWarningView;

/* ==============================================================================
* Description：EarlyWarningTest  
* Author     ：litao
* Create Date：2017/12/5 17:46:34
* ==============================================================================*/

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class ConfigDataToDBTest
    {
        [TestMethod]
        public void RunConfigDataToDB()
        {
            //建立模拟数据
            List<SensitiveData> list = new List<SensitiveData>()
            {
                new SensitiveData("涉及国安","URL","1","www.baidu.com"),
                new SensitiveData("涉及国安","URL","1","www.google.com"),
                new SensitiveData("涉及治安","URL","2","www.治安.com"),
                new SensitiveData("涉及民生","App","3","www.民生.com"),
            };
            for (int i = 0; i < 10000; i++)
            {
                list.Add(new SensitiveData("涉及大数据", "关键字", "4", "关键字" + i));
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            //生成数据库文件
            string curDir = Path.GetFullPath("EarlyWarning");
            DbFromConfigData dataToDb = new DbFromConfigData();
            dataToDb.Initialize(curDir);
            //向数据库中添加数据     
            dataToDb.GenerateDbFile(list);

            Console.WriteLine("dataToDb.GenerateDbFile的时间：{0} ms",sw.ElapsedMilliseconds);
        }
    }
}
