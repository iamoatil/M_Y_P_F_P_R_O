/* ==============================================================================
* Description：Consoler  
*               1,控制台使用主线程作为一个控制线程。
*                   控制线程用于从输入流中读取数据，并且解析，然后控制镜像行为；
*               2，控制台在开始镜像后生成以新线程---镜像线程，用来执行镜像功能。镜像执行完毕后，镜像线程执行停止，关闭这个应用程序
* Author     ：litao
* Create Date：2017/11/8 17:11:20
* ==============================================================================*/

using System;
using System.Threading;

namespace XLY.SF.Project.DataMirrorApp
{
    class Consoler
    {
        string deviceSerialnumber = "";
        int isHtc = 0;
        string path = "";
        string block = "";

        readonly Mirror _mirror = new Mirror();
        bool _isStop;
        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="args"></param>
        public string ParseArgs(string[] args)
        {
            string cmd = "";
            if (args.Length == 5)
            {
                cmd= args[0];
                deviceSerialnumber = args[1];
                isHtc = int.Parse(args[2]);
                path = args[3];
                block = args[4];
            }           
            else
            {
                Console.WriteLine("输入的参数不正确");
                
            }
            return cmd;
        }

        public void Run()
        {
            _mirror.Initialize(deviceSerialnumber, isHtc, path);
            _mirror.Start(block);
        }


        private void TextChangedWriter_TextChanged(string originalCmd)
        {
            string[] cmds = originalCmd.Split('|');
            string cmd = ParseArgs(cmds);

            if (cmd == "StartMirror")
            {
                _mirror.Initialize(deviceSerialnumber, isHtc, path);
                Thread thread = new Thread(
                    o =>
                    {
                        _mirror.Start(block);
                    });
                thread.IsBackground = true;
                thread.Start();
            }
            else if (cmd == "StopMirror")
            {
                _isStop = true;
            }
        }
        
    }
}
