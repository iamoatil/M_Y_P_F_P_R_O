using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XLY.SF.Framework.Language;
using XLY.SF.Project.IsolatedTaskEngine;
using XLY.SF.Project.IsolatedTaskEngine.Common;
using XLY.SF.Project.Plugin.Adapter;

namespace XLY.SF.Project.DeviceExtractionService
{
    class Program
    {
        static void Main(string[] args)
        {
            Mutex mutex = new Mutex(true, "DeviceExtractionService", out Boolean createdNew);
            if (createdNew)
            {
                LanguageManager.SwitchAll(LanguageType.Cn);
                PluginAdapter.Instance.Initialization(null);
                EngineSetup setup = new EngineSetup("isolatedTask", null);
                TaskEngine engine = new TaskEngine(setup);
                engine.Start();
                Console.WriteLine("Task engin is running...:{0}", engine.IsRuning);
                Console.Read();
                mutex.ReleaseMutex();
            }
        }
    }
}
