using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Plugin.Adapter;

namespace XLY.SF.Project.PluginMonitor
{
    /// <summary>
    /// 实现插件的监听服务
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start PluginMonitor ... ");
            Console.WriteLine("load plugin adapter...");
            PluginAdapter.Instance.Initialization(null);
            Console.WriteLine("load PluginInitService...");
            PluginInitService.Instance.Refresh();
            Console.WriteLine("OK...");
        }
    }
}
