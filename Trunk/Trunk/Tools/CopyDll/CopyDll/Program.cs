using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * 编写者：胡佐霖
 * 编写时间：2017/3/30
 * 作用：
 * 1. 拷贝Lib目录下所有DLL文件【不能直接引用的DLL】
 * 2. 发布之前运行即可
 */

namespace CopyDll
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> _filePaths = Directory.GetFiles(@"D:\SourceFiles\", "*.*", SearchOption.AllDirectories).ToList();
            //参数正确性验证
            bool isValidate = true;
            if (args.Length != 0 
                && args.Length != 3)
            {
                isValidate = false;
            }
            string sd =null;
            string td =null;
            if (args.Length == 3)
            {
                sd = args[1].Trim();
                td = args[2].Trim();
                if (!Directory.Exists(sd))
                {
                    isValidate = false;
                    Console.WriteLine(sd + "源目录不存在");
                }
                try
                {
                    Path.GetDirectoryName(td);
                }
                catch (Exception)
                {
                    isValidate = false;
                    Console.WriteLine(td + "目的目录不是有效的目录");
                }
            }

            if(isValidate == false)
            {
                Console.WriteLine("                       格式错误                            \n" +
                                  "此程序只支持以下两种格式运行：                              \n" +
                                  "1，CopyDll.exe                        ----无参数形式       \n" +
                                  "2，CopyDll.exe “源目录” “目的目录”     ---- 有参数形式      \n" +
                                  "     且要保证源目录存在，目的目录字符串是目录                \n");
                return;
            }

            //初始化FileDirectory列表
            List<DirectoryPair> _directoryPairList = new List<DirectoryPair>();
            if(args.Length == 0)
            {
                string exePath=Process.GetCurrentProcess().MainModule.FileName;
                string exeDir = Path.GetDirectoryName(exePath);
                string[] rows=File.ReadAllLines(exeDir+@"\config.cfg");
                foreach (var row in rows)
                {
                    string[] items = row.Split('|');
                    if(items.Length != 2)
                    {
                        continue;
                    }
                    sd = items[0].Trim();
                    td = items[1].Trim();
                    if (!Directory.Exists(sd))
                    {
                        Console.WriteLine(sd + "源目录不存在");
                        continue;
                    }
                    try
                    {
                        Path.GetDirectoryName(td);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(td + "目的目录不是有效的目录");
                        continue;
                    }

                    _directoryPairList.Add(new DirectoryPair(sd, td));
                }
            }
            else
            {
                _directoryPairList.Add(new DirectoryPair(sd, td));
            }
            //执行拷贝
            foreach (var directoryPair in _directoryPairList)
            {
                FileDirectory.CopyIfNewest(directoryPair.SourceDir,directoryPair.TargetDir);
            }

            return;
            //待拷贝DLL位置
            string sourceDllDir = Path.GetFullPath(@"..\..\..\..\Lib\vcdll");
            string sourceX64DllDir = Path.GetFullPath(@"..\..\..\..\Lib\vcdllX64");
            //待拷贝工具位置
            string sourceToolDir = Path.GetFullPath(@"..\..\..\..\Tools");
            //目标位置
            string copyTargetDir = Path.GetFullPath(@"..\..\..\..\Source\31-Build\");
            //中途等待时间
            int waitSeconds = 3;

            Console.WriteLine(@"请选择拷贝模式
1：Release拷贝模式（既拷贝Release版本DLL）
2：Debug拷贝模式（既拷贝Debug版本DLL）
   默认拷贝Debug");

            string _dir = "";
            string userInput = Console.ReadLine();
            switch (userInput)
            {
                case "1":
                    _dir = "Release";
                    break;
                case "2":
                    _dir = "Debug";
                    break;
                default:
                    _dir = "Debug";
                    break;
            }
            //路径验证
            var copyServerHostTargetDir = Path.GetFullPath(copyTargetDir + _dir + "\\ServerHost");
            var copyX64TargetDir = Path.GetFullPath(copyTargetDir + _dir);
            if (!Directory.Exists(sourceDllDir) ||
                !Directory.Exists(sourceX64DllDir) ||
                !Directory.Exists(sourceToolDir) ||
                !Directory.Exists(copyTargetDir) ||
                !Directory.Exists(copyServerHostTargetDir) ||
                !Directory.Exists(copyX64TargetDir))
            {
                Console.WriteLine("【{0}】路径不存在", sourceDllDir);
                return;
            }

            //拷贝DLL
            int copyCount = CopyDll(sourceDllDir, copyServerHostTargetDir, _dir);
            Console.WriteLine("成功拷贝文件【{0}】个", copyCount);
            WaitTime(waitSeconds);

            //拷贝X64DLL
            copyCount = CopyDll(sourceX64DllDir, copyX64TargetDir, _dir);
            Console.WriteLine("成功拷贝文件【{0}】个", copyCount);
            WaitTime(waitSeconds);

            //拷贝必要工具
            //copyCount = CopyTools(sourceToolDir, sourceToolDir, copyTargetDir);
            //Console.WriteLine("成功拷贝文件【{0}】个，{1}秒后程序关闭", copyCount, waitSeconds);
            //WaitTime(waitSeconds);
        }

        #region 拷贝DLL（只针对vcdll文件夹下）

        /// <summary>
        /// 拷贝DLL（只针对vcdll文件夹下）
        /// </summary>
        /// <param name="sourceDllDir"></param>
        private static int CopyDll(string sourceDllDir, string targetDir, string dirState)
        {
            Console.WriteLine("开始拷贝必要DLL到目标位置");
            int result = 0;

            if (Directory.Exists(targetDir) && Directory.Exists(sourceDllDir))
            {
                foreach (var item in Directory.GetDirectories(sourceDllDir))
                {
                    string sourceDllDirTmp = Path.Combine(item, dirState);
                    if (Directory.Exists(sourceDllDirTmp))
                        result += ExecuteCopyDll(sourceDllDirTmp, targetDir);
                }
            }
            else
            {
                Console.WriteLine("【{0}】或【{1}】路径不存在", sourceDllDir, targetDir);
            }
            return result;
        }

        private static int ExecuteCopyDll(string sourceDllDir, string targetDir)
        {
            if(!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            int result = 0;
            var dirs = Directory.GetDirectories(sourceDllDir);
            foreach (var item in dirs)
            {
                ExecuteCopyDll(item, Path.Combine(targetDir, new DirectoryInfo(item).Name));
            }

            var dllfiles = Directory.GetFiles(sourceDllDir, "*.*");
            foreach (var item in dllfiles)
            {
                string fileName = item.Split('\\').Last();
                fileName = Path.Combine(targetDir, fileName);
                //拷贝DLL
                if (!CopyFile(item, fileName))
                    break;
                Console.WriteLine("【{0}】拷贝成功", fileName);
                result++;
            }
            return result;
        }

        #endregion

        #region 拷贝工具

        private static int CopyTools(string baseDir, string dir, string targetBaseDir)
        {
            int copyFileCount = 0;          //拷贝文件数
            if (Directory.Exists(baseDir))
            {
                var dirs = Directory.GetDirectories(dir);
                foreach (var item in dirs)
                {
                    var newDir = item.Replace(baseDir, targetBaseDir);
                    if (!CreateDir(newDir))
                        return copyFileCount;
                    copyFileCount += CopyTools(baseDir, item, targetBaseDir);
                }

                var files = Directory.GetFiles(dir);
                foreach (var item in files)
                {
                    var newFile = item.Replace(baseDir, targetBaseDir);
                    if (!CopyFile(item, newFile))
                        return copyFileCount;
                    copyFileCount++;
                }
            }
            return copyFileCount;
        }

        #endregion

        #region 功能方法

        /// <summary>
        /// 等待执行时间
        /// </summary>
        /// <param name="stopSeconds"></param>
        private static void WaitTime(int stopSeconds)
        {
            int waitTmp = stopSeconds;
            while (waitTmp >= 0)
            {
                Console.WriteLine("等待【{0}】秒后执行下一步操作", waitTmp);
                Thread.Sleep(1000);
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                waitTmp--;
            }
            Console.Clear();
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourFileFullName"></param>
        /// <param name="targetFileFullName"></param>
        /// <returns></returns>
        private static bool CopyFile(string sourFileFullName, string targetFileFullName)
        {
            try
            {
                //取消文件的只读在拷贝
                if (File.Exists(targetFileFullName))
                    System.IO.File.SetAttributes(targetFileFullName, System.IO.FileAttributes.Normal);
                File.Copy(sourFileFullName, targetFileFullName, true);
                System.IO.File.SetAttributes(targetFileFullName, System.IO.FileAttributes.Normal);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"【{0}】---失败【{1}】", sourFileFullName, ex);
            }
            return false;
        }

        /// <summary>
        /// 创建对应文件夹
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static bool CreateDir(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("创建文件夹失败：{0}", ex);
            }
            return false;
        }

        /*
        /// <summary>
        /// 拷贝文件
        /// </summary>
        private static int ExecuteCopyFile(List<string> copyFiles, string copyTargetDir)
        {
            int failSum = 0;
            foreach (var item in copyFiles)
            {
                try
                {
                    if (File.Exists(Path.Combine(copyTargetDir, item.Split('\\').Last())))
                        System.IO.File.SetAttributes(Path.Combine(copyTargetDir, item.Split('\\').Last()), System.IO.FileAttributes.Normal);

                    File.Copy(item, Path.Combine(copyTargetDir, item.Split('\\').Last()), true);
                    System.IO.File.SetAttributes(Path.Combine(copyTargetDir, item.Split('\\').Last()), System.IO.FileAttributes.Normal);
                    Console.WriteLine("【{0}】---成功", item);
                }
                catch (Exception ex)
                {
                    failSum++;
                    Console.WriteLine(@"【{0}】---失败
【{1}】", item, ex);
                }
            }
            return failSum;
        }

        /// <summary>
        /// 获取要拷贝的文件
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="dirState"></param>
        /// <param name="targetFiles"></param>
        private static void GetCopyFiles(string targetDir, string dirState, ref List<string> targetFiles)
        {
            if ((targetDir.Split('\\').Last().ToUpper() == "RELEASE" ||
                targetDir.Split('\\').Last().ToUpper() == "DEBUG") &&
                targetDir.Split('\\').Last().ToUpper() != dirState.ToUpper())
                return;

            if (Directory.Exists(targetDir))
            {
                foreach (var item in Directory.GetFiles(targetDir, "*.*"))
                {
                    targetFiles.Add(item);
                }
                foreach (var item in Directory.GetDirectories(targetDir))
                {
                    GetCopyFiles(item, dirState, ref targetFiles);
                }
            }
        }*/

        #endregion
    }
}
