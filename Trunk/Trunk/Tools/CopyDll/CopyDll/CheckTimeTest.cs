using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/* ==============================================================================
* Description：CheckTimeTest  
* Author     ：litao
* Create Date：2017/11/1 16:18:18
* ==============================================================================*/

namespace CopyDll
{
    /// <summary>
    /// CheckTimeTest
    /// </summary>
    class CheckTimeTest
    {
        public void ExistCheck()
        {
            string sourceDir = @"d:\SourceFiles\";
            string targetDir = @"d:\TargetFiles\";
            Stopwatch sw=new Stopwatch();
            long lastTime = 0;
            long curTime = 0;
            long elapsedTime = 0;
            sw.Start();

            FileDirectory sourceDirectory = new FileDirectory(sourceDir,"*.*",SearchOption.AllDirectories);
            
            curTime = sw.ElapsedMilliseconds;
            elapsedTime = curTime - lastTime;
            lastTime = curTime;
            Console.WriteLine("elapsedTime:" + elapsedTime);

            FileDirectory targetDirectory = new FileDirectory(targetDir, "*.*", SearchOption.AllDirectories);
            curTime = sw.ElapsedMilliseconds;
            elapsedTime = curTime - lastTime;
            lastTime = curTime;
            Console.WriteLine("elapsedTime:" + elapsedTime);

            FileDirectory.CopyIfNewest(sourceDirectory,targetDirectory);

            curTime = sw.ElapsedMilliseconds;
            elapsedTime = curTime - lastTime;
            lastTime = curTime;
            Console.WriteLine("elapsedTime:" + elapsedTime);
        }
    }
}
