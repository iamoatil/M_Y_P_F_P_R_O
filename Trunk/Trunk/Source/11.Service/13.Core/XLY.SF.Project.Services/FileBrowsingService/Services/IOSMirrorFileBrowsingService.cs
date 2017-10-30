/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 18:00:24 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// iPhone镜像文件浏览服务
    /// </summary>
    internal class IOSMirrorFileBrowsingService : AbsFileBrowsingService
    {
        /// <summary>
        /// iTuns备份文件根路径
        /// </summary>
        private string MirrorFilePath { get; set; }

        public IOSMirrorFileBrowsingService(string mirrorFile)
        {
            MirrorFilePath = mirrorFile;
        }

        /// <summary>
        /// 解析文件保存根路径
        /// </summary>
        private string DataSourcePath { get; set; }

        protected override FileBrowingNode DoGetRootNode(IAsyncProgress async)
        {
            var di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Root.Name;

            string target = Path.Combine(di, "temp", Guid.NewGuid().ToString("N"), "IOSMirror");
            FileHelper.CreateDirectorySafe(target);

            ZipFile.ExtractToDirectory(MirrorFilePath, target);

            IOSMirrorFileBrowingNode node = new IOSMirrorFileBrowingNode();
            node.NodeType = FileBrowingNodeType.Directory;
            node.Name = "ItunsBackup";
            node.SourcePath = target.TrimStart("\\");

            DataSourcePath = target.TrimStart("\\");

            return node;
        }

        protected override List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode, IAsyncProgress async)
        {
            List<FileBrowingNode> list = new List<FileBrowingNode>();

            if (null == parentNode || parentNode.NodeType == FileBrowingNodeType.File)
            {
                return list;
            }

            var path = (parentNode as IOSMirrorFileBrowingNode).SourcePath;
            var parentPathInfo = new DirectoryInfo(path);
            if (parentPathInfo.Exists)
            {
                foreach (var di in parentPathInfo.GetDirectories())
                {
                    list.Add(new IOSMirrorFileBrowingNode()
                    {
                        Name = di.Name,
                        NodeType = FileBrowingNodeType.Directory,
                        SourcePath = di.FullName,
                        Parent = parentNode,
                    });
                }

                foreach (var fi in parentPathInfo.GetFiles())
                {
                    list.Add(new IOSMirrorFileBrowingNode()
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

        protected override void DoDownload(FileBrowingNode node, string savePath, bool persistRelativePath, IAsyncProgress async)
        {
            FileHelper.CreateDirectory(savePath);

            DownLoadNode(node as IOSMirrorFileBrowingNode, savePath, persistRelativePath, async);
        }

        private void DownLoadNode(IOSMirrorFileBrowingNode node, string savePath, bool persistRelativePath, IAsyncProgress async)
        {
            if (null == node || node.SourcePath.IsInvalid())
            {
                return;
            }

            if (node.NodeType == FileBrowingNodeType.File)
            {
                DownLoadFile(node, savePath, persistRelativePath, async);
            }
            else
            {
                var cSavePath = Path.Combine(savePath, node.Name).Replace('/', '\\');
                if (persistRelativePath)
                {
                    cSavePath = savePath.Replace('/', '\\');
                }

                FileHelper.CreateDirectory(cSavePath);

                foreach (var cnode in DoGetChildNodes(node, async))
                {
                    DownLoadNode(cnode as IOSMirrorFileBrowingNode, cSavePath, persistRelativePath, async);
                }
            }
        }

        private void DownLoadFile(IOSMirrorFileBrowingNode fileNode, string savePath, bool persistRelativePath, IAsyncProgress async)
        {
            try
            {
                var tSavePath = string.Empty;
                if (persistRelativePath)
                {
                    tSavePath = Path.Combine(savePath, fileNode.SourcePath.Replace('/', '\\').TrimStart("\\").TrimStart(DataSourcePath).TrimStart("\\"));
                }
                else
                {
                    tSavePath = Path.Combine(savePath, fileNode.Name).Replace('/', '\\');
                }

                FileHelper.CreateDirectory(FileHelper.GetFilePath(tSavePath));

                File.Copy(fileNode.SourcePath, tSavePath);
            }
            catch
            {

            }
        }

        /// <summary>
        /// iPhone镜像文件节点
        /// </summary>
        private class IOSMirrorFileBrowingNode : FileBrowingNode
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
