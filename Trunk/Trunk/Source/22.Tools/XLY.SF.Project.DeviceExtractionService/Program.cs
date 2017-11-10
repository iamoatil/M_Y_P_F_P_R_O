using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.IsolatedTaskEngine;
using XLY.SF.Project.Plugin.Adapter;

namespace XLY.SF.Project.DeviceExtractionService
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineSetup setup = new EngineSetup("isolatedTask", null);
            TaskEngine engine = new TaskEngine(setup);
            engine.Start();
            Console.WriteLine("Task engin is running...:{0}", engine.IsRuning);
            Console.Read();
        }
    }
}
