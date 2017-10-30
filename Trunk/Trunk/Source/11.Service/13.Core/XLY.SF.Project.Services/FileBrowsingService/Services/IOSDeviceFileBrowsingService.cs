/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 17:58:56 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Devices.AdbSocketManagement;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// iPhone手机文件浏览服务
    /// </summary>
    internal class IOSDeviceFileBrowsingService : AbsFileBrowsingService
    {
        /// <summary>
        /// 手机
        /// </summary>
        private Device IPhone { get; set; }

        /// <summary>
        /// 根节点
        /// </summary>
        private FileBrowingNode RootNode { get; set; }

        public IOSDeviceFileBrowsingService(Device device)
        {
            IPhone = device;

            RootNode = new IOSDeviceFileBrowingNode()
            {
                Name = "根目录",
                NodeType = FileBrowingNodeType.Directory,
                SourcePath = "/"
            };
        }

        protected override FileBrowingNode DoGetRootNode(IAsyncProgress async)
        {
            return RootNode;
        }

        protected override List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode, IAsyncProgress async)
        {
            return DoGetChildNodes(parentNode as IOSDeviceFileBrowingNode);
        }

        private List<FileBrowingNode> DoGetChildNodes(IOSDeviceFileBrowingNode parentNode)
        {
            if (null == parentNode)
            {
                return null;
            }

            if (parentNode.NodeType == FileBrowingNodeType.Directory)
            {
                return GetIOSFileSystem(parentNode.SourcePath, parentNode);
            }

            return null;
        }

        private List<FileBrowingNode> GetIOSFileSystem(string folderPath, FileBrowingNode parentNode)
        {
            List<FileBrowingNode> list = new List<FileBrowingNode>();

            try
            {
                // 1，设置服务
                uint result = IOSDeviceCoreDll.SetIphoneFileService(IPhone.ID, IPhone.IsRoot);
                if (0 != result)
                {
                    return list;
                }

                IntPtr allFileInfosInDir = IntPtr.Zero;
                result = IOSDeviceCoreDll.GetIphoneALLFileInfosInDir(IPhone.ID, folderPath, ref allFileInfosInDir);
                if (result != 0 || allFileInfosInDir == IntPtr.Zero)
                {
                    return list;
                }

                //它返回的当前文件下的第一级文件系统，其中可能包括文件和文件夹。
                IosFileSystemList iosFileSystemlist = allFileInfosInDir.ToStruct<IosFileSystemList>();
                IntPtr fileSysNodes = iosFileSystemlist.FileSystemNodes;

                IosFileSystem iosFileSystem;
                //读取每一个文件系统实体。根据FileSystemList大小，一次解析一个文件系统，根据Type判断是文件还是文件夹。
                for (int i = 0; i < iosFileSystemlist.Length && fileSysNodes != IntPtr.Zero; i++)
                {
                    iosFileSystem = fileSysNodes.ToStruct<IosFileSystem>();

                    string name = iosFileSystem.Name.ToAnsiString();
                    //掉过掉Unix特有的隐藏文件“.”和"..",他们两个的作用是前进和后退。
                    if (name != "." && name != "..")
                    {
                        list.Add(new IOSDeviceFileBrowingNode()
                        {
                            Name = name,
                            SourcePath = string.Format("{0}/{1}", folderPath.TrimStart('/'), name),
                            FileSize = iosFileSystem.Size,
                            CreateTime = DynamicConvert.ToSafeDateTime(iosFileSystem.CreateTime),
                            NodeType = iosFileSystem.Type == 1 ? FileBrowingNodeType.Directory : FileBrowingNodeType.File,
                            Parent = parentNode,
                        });
                    }

                    fileSysNodes = fileSysNodes.Increment<IosFileSystem>();
                }
            }
            catch
            {
            }
            finally
            {
                // 3，关闭服务
                IOSDeviceCoreDll.CloseIphoneFileService(IPhone.ID);
            }

            return list;
        }

        protected override void DoDownload(FileBrowingNode node, string savePath, bool persistRelativePath, IAsyncProgress async)
        {
            FileHelper.CreateDirectory(savePath);

            DownLoadNode(node as IOSDeviceFileBrowingNode, savePath, persistRelativePath, async);
        }

        private void DownLoadNode(IOSDeviceFileBrowingNode node, string savePath, bool persistRelativePath, IAsyncProgress async)
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

                foreach (var cnode in GetIOSFileSystem(node.SourcePath, node))
                {
                    DownLoadNode(cnode as IOSDeviceFileBrowingNode, cSavePath, persistRelativePath, async);
                }
            }
        }

        private void DownLoadFile(IOSDeviceFileBrowingNode fileNode, string savePath, bool persistRelativePath, IAsyncProgress async)
        {
            try
            {
                var tSavePath = string.Empty;
                if (persistRelativePath)
                {
                    tSavePath = Path.Combine(savePath, fileNode.SourcePath).Replace('/', '\\');
                }
                else
                {
                    tSavePath = Path.Combine(savePath, fileNode.Name).Replace('/', '\\');
                }

                FileHelper.CreateDirectory(FileHelper.GetFilePath(tSavePath));

                // 1，设置服务
                uint result = IOSDeviceCoreDll.SetIphoneFileService(IPhone.ID, IPhone.IsRoot);

                // 2，下载
                result = IOSDeviceCoreDll.CopyOneIosFile(IPhone.ID, fileNode.SourcePath, tSavePath);
            }
            catch
            {
            }
            finally
            {
                // 3，关闭服务
                IOSDeviceCoreDll.CloseIphoneFileService(IPhone.ID);
            }
        }

        /// <summary>
        /// iPhone文件节点
        /// </summary>
        private class IOSDeviceFileBrowingNode : FileBrowingNode
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
