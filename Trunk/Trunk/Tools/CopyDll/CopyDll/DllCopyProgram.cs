using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

/* ==============================================================================
* Description：DllCopyProgram  
* Author     ：litao
* Create Date：2017/11/1 17:14:24
* ==============================================================================*/

namespace CopyDll
{
    class DllCopyProgram
    {
        public void Execute(string[] args)
        {
            try
            {
                bool isSuc=Exec(args);
                if(isSuc)
                {
                    Console.WriteLine("全部拷贝完成！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("发生错误！");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                Console.WriteLine("按任意键继续");
                Console.Read();
            }            
        }

        private bool Exec(string[] args)
        {
            //参数正确性验证
            bool isValidate = true;
            if (args.Length != 0
                && args.Length != 2)
            {
                isValidate = false;
            }
            string arg1 = null;
            string arg2 = null;
            ArgType argType = ArgType.Arg;
            if (args.Length == 2)
            {
                arg1 = args[0].Trim();
                arg2 = args[1].Trim();
                if(arg1 == "/f")
                {
                    argType = ArgType.File;
                }                               
            }
            else if(args.Length == 0)
            {
                argType = ArgType.File;
                arg2 = "config.cfg";
            }

            if (argType == ArgType.Arg)
            {
                if (!Directory.Exists(arg1))
                {
                    isValidate = false;
                    Console.WriteLine(arg1 + "源目录不存在");
                }
                try
                {
                    Path.GetDirectoryName(arg2);
                }
                catch (Exception)
                {
                    isValidate = false;
                    Console.WriteLine(arg2 + "目的目录不是有效的目录");
                }
            }
            else if (argType == ArgType.File)
            {
                if (!File.Exists(arg2))
                {
                    isValidate = false;
                    Console.WriteLine(arg2 + "配置文件不存在！");
                }
            }

            if (isValidate == false)
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("                       格式错误                              \n" +
                                  "此程序只支持以下3种格式运行：                                  \n" +
                                  "1，CopyDll.exe                                                \n" +
                                  "             ----无参数形式,在调用CopyDll.exe的进程目录下找config.cfg文件，然后以其为文件参数进行拷贝\n" +
                                  "2，CopyDll.exe “源目录” “目的目录”                                                                 \n" +
                                  "             ---- 有参数形式,要保证源目录存在，目的目录字符串是目录                                  \n" +
                                  "3，CopyDll.exe /f filePath                                                                         \n" +
                                  "             ----一个文件参数形式,filePath表示一个配置文件，其中需要以形式 “源目录 | 目的目录” 为行标记一条要拷贝的记录  \n"
                                  );
                Console.ForegroundColor = color;
                return false;
            }

            //初始化FileDirectory列表
            List<DirectoryPair> _directoryPairList = new List<DirectoryPair>();
            if (argType == ArgType.File)
            {
                string exePath = Process.GetCurrentProcess().MainModule.FileName;
                string exeDir = Path.GetDirectoryName(exePath);
                string[] rows = File.ReadAllLines(exeDir +@"\"+ arg2);
                foreach (var row in rows)
                {
                    string[] items = row.Split('|');
                    if (items.Length != 2)
                    {
                        continue;
                    }
                    arg1 = items[0].Trim();
                    arg2 = items[1].Trim();
                    if (!Directory.Exists(arg1))
                    {
                        Console.WriteLine(Path.GetFullPath(arg1) + "源目录不存在");
                        continue;
                    }
                    try
                    {
                        Path.GetDirectoryName(arg2);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(Path.GetFullPath(arg2) + "目的目录不是有效的目录");
                        continue;
                    }

                    _directoryPairList.Add(new DirectoryPair(arg1, arg2));
                }
            }
            else
            {
                _directoryPairList.Add(new DirectoryPair(arg1, arg2));
            }
            //执行拷贝
            foreach (var directoryPair in _directoryPairList)
            {
                FileDirectory.CopyIfNewest(directoryPair.SourceDir, directoryPair.TargetDir);
            }

            return true;
        }
    }

    public enum ArgType
    {
        File,
        Arg
    }
}
