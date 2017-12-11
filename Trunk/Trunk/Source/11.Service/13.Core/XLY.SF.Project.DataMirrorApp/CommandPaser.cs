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
        long startedPos = 0;

        readonly Mirror _mirror = new Mirror();
        bool _isStop;
        Thread _thread;

        public void Run()
        {
            while (true)
            {
                var arg = Console.ReadLine();
                if (arg == null)//当正在镜像时，关闭客户端arg就会为空
                {
                    break;
                }

                Console.WriteLine("收到arg:" + arg);
                string operateStr = ParseArgs(arg);
                CmdString operate = new CmdString(operateStr);
                try
                {
                    CmdExecute(operate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}|{1}", CmdStrings.Exception, ex.Message);
                    throw ex;
                }

                if (_isStop)
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
            if (cmds.Length == 6)
            {
                operateCmd = cmds[0];
                deviceSerialnumber = cmds[1];
                isHtc = int.Parse(cmds[2]);
                path = cmds[3];
                block = cmds[4];
                bool ret=long.TryParse(cmds[5],out startedPos);
                if(ret == false)
                {
                    Console.WriteLine("startedPos输入的参数不正确");
                }
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

        private void CmdExecute(CmdString operateCmd)
        {
            if (operateCmd.Match(CmdStrings.StartMirror)
                || operateCmd.Match(CmdStrings.ContinueMirror))
            {
                //只有开始状态时，才初始化_mirror
                if (operateCmd.Match(CmdStrings.StartMirror))
                {
                    _mirror.Initialize(deviceSerialnumber, isHtc, path);
                }
                
                _thread = new Thread(
                    o =>
                    {
                        _mirror.Start(block, startedPos);
                    });
                _thread.IsBackground = true;
                _thread.Start();
            }
            else if (operateCmd.Match(CmdStrings.StopMirror)
                || operateCmd.Match(CmdStrings.PauseMirror))
            {
                _thread.Abort();
                _isStop = true;
            }

            //返回当前状态
            if(_mirror.IsInitialized)
            {
                SendSate(operateCmd);
            }            
        }

        /// <summary>
        /// 发送状态到调用端
        /// </summary>
        public void SendSate(CmdString cmd)
        {
            Console.WriteLine(string.Format("{0}", cmd));
        }
    }
}
