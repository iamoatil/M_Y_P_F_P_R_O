using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace XLY.SF.Framework.BaseUtility
{
    /// <summary>
    /// winRAR辅助类
    /// 提供文件夹压缩和文件解压方法
    /// </summary>
    public static class WinRARCSharp
    {
        /// <summary>
        /// WinRAR.exe 的完整路径
        /// </summary>
        private static readonly string rarexe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"lib\WinRAR\WinRAR.exe");

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern int ShowWindow(IntPtr hWnd, uint nCmdShow);

        /// <summary>
        /// 利用 WinRAR 进行压缩
        /// </summary>
        /// <param name="path">将要被压缩的文件夹（绝对路径）</param>
        /// <param name="rarPath">压缩后的 .rar 的存放目录（绝对路径）</param>
        /// <param name="rarName">压缩文件的名称（包括后缀）</param>
        /// <returns>true 或 false。压缩成功返回 true，反之，false。</returns>
        public static bool RAR(string path, string rarPath, string rarName)
        {
            bool flag = false;
            string cmd;          //WinRAR 命令参数
            ProcessStartInfo startinfo;
            Process process;
            try
            {
                Directory.CreateDirectory(path);
                //压缩命令，相当于在要压缩的文件夹(path)上点右键->WinRAR->添加到压缩文件->输入压缩文件名(rarName)
                cmd = string.Format(@"a {0} {1}\*.* -r -ep1", rarName, path);
                // -r 包含子文件夹   -ep1 排除基本路径 
                //\*.*是必备的 代表压缩指定文件夹下面的所有子文件夹和子文件
                //例如压缩C:\11\22\33文件夹
                //使用a {0} {1} -r  ,则压缩后的文件都是11\22\33\*.*
                //使用a {0} {1} -r -ep1  ,则压缩后的文件都是33\*.*
                //使用a {0} {1}\*.* -r -ep1  ,则压缩后的文件都是*.*

                startinfo = new ProcessStartInfo();
                startinfo.FileName = rarexe;
                startinfo.Arguments = cmd;                          //设置命令参数
                startinfo.WindowStyle = ProcessWindowStyle.Hidden;  //隐藏 WinRAR 窗口
                startinfo.UseShellExecute = false;
                startinfo.CreateNoWindow = true;

                startinfo.WorkingDirectory = rarPath;
                process = new Process();
                process.StartInfo = startinfo;
                process.Start();

                try
                {
                    while (process.MainWindowHandle == IntPtr.Zero && !process.HasExited)
                    {//这儿必须等待，不然主界面还没Show出来MainWindowHandle是空的
                        System.Threading.Thread.Sleep(1);
                    }
                    ShowWindow(process.MainWindowHandle, 0);//隐藏窗口
                }
                catch { }

                process.WaitForExit(); //无限期等待进程 winrar.exe 退出
                if (process.HasExited)
                {
                    flag = true;
                }
                process.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return flag;
        }

        /// <summary>
        /// 利用 WinRAR 进行解压缩
        /// 支持长路径
        /// </summary>
        /// <param name="path">文件解压路径（绝对）</param>
        /// <param name="rarPath">将要解压缩的 .rar 文件的存放目录（绝对路径）</param>
        /// <param name="rarName">将要解压缩的 .rar 文件名（包括后缀）</param>
        /// <returns>true 或 false。解压缩成功返回 true，反之，false。</returns>
        public static bool UnRAR(string path, string rarPath, string rarName)
        {
            bool flag = false;
            string cmd;
            ProcessStartInfo startinfo;
            Process process;
            try
            {
                Directory.CreateDirectory(path);
                //解压缩命令，相当于在要压缩文件(rarName)上点右键->WinRAR->解压到当前文件夹
                cmd = string.Format("x -ibck {0} {1} -y",
                                    rarName,
                                    path);
                startinfo = new ProcessStartInfo();
                startinfo.FileName = rarexe;
                startinfo.Arguments = cmd;
                startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                startinfo.UseShellExecute = false;

                startinfo.WorkingDirectory = rarPath;
                process = new Process();
                process.StartInfo = startinfo;
                process.Start();

                try
                {
                    while (process.MainWindowHandle == IntPtr.Zero && !process.HasExited)
                    {//这儿必须等待，不然主界面还没Show出来MainWindowHandle是空的
                        System.Threading.Thread.Sleep(1);
                    }
                    ShowWindow(process.MainWindowHandle, 0);//隐藏窗口
                }
                catch { }

                process.WaitForExit();
                if (process.HasExited)
                {
                    flag = true;
                }
                process.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return flag;
        }

    }
}
