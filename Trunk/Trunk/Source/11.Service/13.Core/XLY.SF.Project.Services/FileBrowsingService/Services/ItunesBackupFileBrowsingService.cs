/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 19:57:44 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// ituns备份文件浏览服务
    /// </summary>
    internal class ItunesBackupFileBrowsingService : AbsFileBrowsingService
    {
        /// <summary>
        /// iTuns备份文件根路径
        /// </summary>
        private string SourceRootPath { get; set; }

        public ItunesBackupFileBrowsingService(string sourcePath)
        {
            SourceRootPath = sourcePath;
        }

        /// <summary>
        /// 解析文件保存根路径
        /// </summary>
        private string DataSourcePath { get; set; }

        protected override FileBrowingNode DoGetRootNode()
        {
            var di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Root.Name;

            string target = Path.Combine(di, "temp", Guid.NewGuid().ToString("N"), "ItunsBackup");
            FileHelper.CreateDirectorySafe(target);

            var result = IOSDeviceCoreDll.AnalyzeItunesBackupDATAPWD(SourceRootPath, target, BackupAnalysizeCallbackDelegate, (b) =>
                                  {
                                      var password = "";

                                      var pS = Marshal.StringToHGlobalAnsi(password);

                                      Marshal.WriteIntPtr(b, pS);

                                      return 0;
                                  });

            ItunesBackupFileBrowingNode node = new ItunesBackupFileBrowingNode();
            node.NodeType = FileBrowingNodeType.Directory;
            node.Name = "ItunsBackup";
            node.SourcePath = target.TrimStart("\\");

            DataSourcePath = target.TrimStart("\\");

            return node;
        }

        private int BackupAnalysizeCallbackDelegate(string pFilename, int curfileno, int allnums)
        {
            return 0;
        }

        protected override List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode)
        {
            List<FileBrowingNode> list = new List<FileBrowingNode>();

            if (null == parentNode || parentNode.NodeType == FileBrowingNodeType.File)
            {
                return list;
            }

            var path = (parentNode as ItunesBackupFileBrowingNode).SourcePath;
            var parentPathInfo = new DirectoryInfo(path);
            if (parentPathInfo.Exists)
            {
                foreach (var di in parentPathInfo.GetDirectories())
                {
                    list.Add(new ItunesBackupFileBrowingNode()
                    {
                        Name = di.Name,
                        NodeType = FileBrowingNodeType.Directory,
                        SourcePath = di.FullName,
                        Parent = parentNode,
                    });
                }

                foreach (var fi in parentPathInfo.GetFiles())
                {
                    list.Add(new ItunesBackupFileBrowingNode()
                    {
                        Name = fi.Name,
                        NodeType = FileBrowingNodeType.File,
                        SourcePath = fi.FullName,
                        FileSize = (UInt64)fi.Length,
                        Parent = parentNode,
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 开始搜索
        /// </summary>
        /// <param name="node">搜索根节点，必须是文件夹类型 即IsFile为false</param>
        /// <param name="args">搜索条件</param>
        /// <param name="async">异步通知</param>
        protected override void BeginSearch(FileBrowingNode node, IEnumerable<FilterArgs> args, CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async)
        {
            var stateArg = args.FirstOrDefault(a => a is FilterByEnumStateArgs);
            if (null != stateArg && (stateArg as FilterByEnumStateArgs).State != EnumDataState.Normal)
            {//如果要搜索删除状态的文件，直接返回。因为iTunes备份文件浏览不会有删除状态的文件。
                //TODO:通知搜索结束
                return;
            }

            var dateArg = args.FirstOrDefault(a => a is FilterByDateRangeArgs);
            if (null != stateArg)
            {//如果要搜索指定创建时间范围的文件，直接返回。因为iTunes备份文件浏览无法获取文件创建时间。
                //TODO:通知搜索结束
                return;
            }

            base.BeginSearch(node, args, cancellationTokenSource, async);
        }

        protected override void DownLoadFile(FileBrowingNode fileNode, string savePath, bool persistRelativePath, CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async)
        {
            var ifileNode = fileNode as ItunesBackupFileBrowingNode;

            try
            {
                var tSavePath = string.Empty;
                if (persistRelativePath)
                {
                    tSavePath = Path.Combine(savePath, ifileNode.SourcePath.Replace('/', '\\').TrimStart("\\").TrimStart(DataSourcePath).TrimStart("\\"));
                }
                else
                {
                    tSavePath = Path.Combine(savePath, ifileNode.Name).Replace('/', '\\');
                }

                FileHelper.CreateDirectory(FileHelper.GetFilePath(tSavePath));

                File.Copy(ifileNode.SourcePath, tSavePath);
            }
            catch
            {

            }
        }

        /// <summary>
        /// iTunes备份文件节点
        /// </summary>
        private class ItunesBackupFileBrowingNode : FileBrowingNode
        {
            /// <summary>
            /// 源路径
            /// </summary>
            public string SourcePath { get; internal set; }

            public override string ToString()
            {
                return SourcePath;
            }
        }
    }
}
