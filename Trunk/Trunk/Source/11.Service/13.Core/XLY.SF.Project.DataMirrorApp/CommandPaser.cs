/* ==============================================================================
* Description：CommandPaser  
*               1,CommandPaser使用主线程作为一个控制线程。
*                   控制线程用于从输入流中读取数据，并且解析，然后控制镜像行为；
*               2，CommandPaser在开始镜像后生成以新线程---镜像线程，用来执行镜像功能。镜像执行完毕后，镜像线程执行停止，关闭这个应用程序
* Author     ：litao
* Create Date：2017/11/8 17:11:20
* ==============================================================================*/

using System;
using System.Threading;

namespace XLY.SF.Project.DataMirrorApp
{
    /// <summary>
    /// 命令解析，执行器
    /// </summary>
    class CommandPaser
    {
        string deviceSerialnumber = "";
        int isHtc = 0;
        string path = "";
        string block = "";

        readonly Mirror _mirror = new Mirror();
        bool _isStop;
        Thread _thread;

        public void Run()
        {
            while (true)
            {
                var arg = Console.ReadLine();

                Console.WriteLine("收到arg:" + arg);
                string operateCmd = ParseArgs(arg);
                CmdExecute(operateCmd);

                if(_isStop)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="args"></param>
        private string ParseArgs(string arg)
        {
            string[] cmds = arg.Split('|');

            string operateCmd = "";
            if (cmds.Length == 5)
            {
                operateCmd = cmds[0];
                deviceSerialnumber = cmds[1];
                isHtc = int.Parse(cmds[2]);
                path = cmds[3];
                block = cmds[4];
            }
            else if (cmds.Length == 1)
            {
                operateCmd = cmds[0];
            }
            else
            {
                Console.WriteLine("输入的参数不正确");
            }
            return operateCmd;
        }

        private void CmdExecute(string operateCmd)
        {        
            if (operateCmd == "StartMirror")
            {
                _mirror.Initialize(deviceSerialnumber, isHtc, path);
                _thread = new Thread(
                    o =>
                    {
                        _mirror.Start(block);
                    });
                _thread.IsBackground = true;
                _thread.Start();
            }
            else if (operateCmd == "StopMirror")
            {
                _thread.Abort();
                _mirror.Stop("UserStoped");

                _isStop = true;

            }
        }
        
    }
}
