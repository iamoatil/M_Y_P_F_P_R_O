/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 11:12:54 
 * explain :  
 *
*****************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XLY.SF.Framework.Core.Base.MefIoc;
using XLY.SF.Project.Domains;
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

            var pump = new Pump() { OSType = EnumOSType.Android, Type = EnumPump.USB };

            var allexts = PluginAdapter.Instance.GetAllExtractItems(pump);

            var plugs = PluginAdapter.Instance.MatchPluginByPump(pump, allexts);

        }
    }
}
