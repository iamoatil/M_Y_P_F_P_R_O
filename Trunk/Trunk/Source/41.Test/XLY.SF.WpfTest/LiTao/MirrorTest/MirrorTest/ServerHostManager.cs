using System;
using System.Diagnostics;
using System.IO;

/* ==============================================================================
* Description：ServerHostManager  
* Author     ：litao
* Create Date：2017/11/7 19:21:35
* ==============================================================================*/

namespace MirrorTest
{
    class ServerHostManager
    {

        const string ServerHostPath = @"ServerHost\DllServerHost.exe";

        /// <summary>
        /// 启动DllServerHost.exe的原因是，安卓镜像采用的dll（研究部提供）是x86的，而现在我们的SPPro是x64的，所以不能直接调用。
        /// 当前采取的策略是镜像功能作为一个独立的程序运行，其数据通过wcf传递给SPPro，这样就能解决X64调用X86的问题了。
        /// 
        /// 所以，镜像功能的测试需要先运行x86的镜像独立程序。
        /// </summary>
        public void StartServerHost()
        {
            StopServerHost();
            if (!File.Exists(ServerHostPath))
            {
                throw new Exception("程序不能运行，请确保文件" + ServerHostPath + "存在");
            }
            Process.Start(ServerHostPath);
        }

        /// <summary>
        /// 停止ServerHost。在启动前和程序关闭时执行
        /// </summary>
        public void StopServerHost()
        {
            Process[] processes = Process.GetProcessesByName(Path.GetFileName(ServerHostPath));
            if (processes != null
                && processes.Length > 0)
            {
                foreach (var item in processes)
                {
                    item.Kill();
                }
            }
        }
    }
}
