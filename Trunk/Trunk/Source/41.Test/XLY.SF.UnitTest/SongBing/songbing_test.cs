/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 11:12:54 
 * explain :  
 *
*****************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Plugin.Adapter;

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class Songbing_Test
    {
        [TestMethod]
        public void TestPlugin()
        {
            Console.WriteLine("开始启动服务");
            PluginAdapter.Instance.Initialization(null);

            var pump = new Pump(@"F:\Temp", "a.db") { OSType = EnumOSType.Android, Type = EnumPump.USB };

            var allexts = PluginAdapter.Instance.GetAllExtractItems(pump);

            var plugs = PluginAdapter.Instance.MatchPluginByPump(pump, allexts);

        }

        [TestMethod]
        public void TestDataRecovery()
        {
            var MainDbPath = @"D:\test\contacts2.db";

            var t1 = Task.Run(() =>
                  {
                      SqliteRecoveryHelper.DataRecovery(MainDbPath, @"chalib\com.android.providers.contacts\contacts2.db.charactor", "calls", true);
                  });

            var t2 = Task.Run(() =>
              {
                  SqliteRecoveryHelper.DataRecovery(MainDbPath, @"chalib\com.android.providers.contacts\contacts2.db.charactor", "calls", true);
              });

            Task.WaitAll(t1, t2);

        }
    }
}
