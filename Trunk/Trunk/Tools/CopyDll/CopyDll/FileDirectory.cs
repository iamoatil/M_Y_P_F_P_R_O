using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：DllDirectory  
* Author     ：litao
* Create Date：2017/11/1 17:16:44
* ==============================================================================*/

namespace CopyDll
{
    /// <summary>
    /// FileDirectory
    /// </summary>
    public class FileDirectory
    {
        public FileDirectory(string dirPath, string searchPattern, SearchOption searchOption)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            _filePaths = Directory.GetFiles(dirPath, searchPattern, searchOption).ToList();
            _dirPath = dirPath;
        }

        private readonly string _dirPath;
        private readonly List<string> _filePaths;

        /// <summary>
        /// 目录拷贝
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="targetDir"></param>
        public static void Copy(FileDirectory sourceDir, FileDirectory targetDir)
        {
            foreach (string path in sourceDir._filePaths)
            {
                try
                {
                    string fakeTargetPath = path.Replace(sourceDir._dirPath, targetDir._dirPath);
                    bool isExisted = targetDir._filePaths.Contains(fakeTargetPath);
                    if (!isExisted)
                    {
                        string dirName = Path.GetDirectoryName(fakeTargetPath);
                        if (!Directory.Exists(dirName))
                        {
                            Directory.CreateDirectory(dirName);
                        }
                        File.Copy(path, fakeTargetPath);
                    }
                }
                catch { }
            }
        }
        
        /// <summary>
        /// 如果最新则拷贝
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="targetDir"></param>
        public static void CopyIfNewest(FileDirectory sourceDir, FileDirectory targetDir)
        {
            Console.Write("正在拷贝目录：" + sourceDir._dirPath);
            foreach (string path in sourceDir._filePaths)
            {
                try
                {
                    string fakeTargetPath = path.Replace(sourceDir._dirPath, targetDir._dirPath);
                    bool isExisted = targetDir._filePaths.Contains(fakeTargetPath);
                    if (!isExisted)
                    {
                        string dirName = Path.GetDirectoryName(fakeTargetPath);
                        if (!Directory.Exists(dirName))
                        {
                            Directory.CreateDirectory(dirName);
                        }
                        Console.Write(".");
                        File.Copy(path, fakeTargetPath);
                    }
                    else
                    {
                        DateTime targetFileTime = File.GetLastWriteTime(fakeTargetPath);
                        DateTime sourceFileTime = File.GetLastWriteTime(path);

                        if (sourceFileTime > targetFileTime)
                        {
                            Console.Write(".");
                            File.Copy(path, fakeTargetPath, true);
                        }
                    }
                }
                catch { }
            }
            Console.WriteLine("\n拷贝结束");
        }
    }
}
