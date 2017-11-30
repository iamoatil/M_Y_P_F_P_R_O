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
using System.Threading;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Contract;

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
                Name = device.Name,
                NodeType = FileBrowingNodeType.Directory,
                SourcePath = "/"
            };
        }

        protected override FileBrowingNode DoGetRootNode()
        {
            return RootNode;
        }

        protected override List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode)
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
            {//如果要搜索删除状态的文件，直接返回。因为IOS手机文件浏览不会有删除状态的文件。
                //TODO:通知搜索结束
                return;
            }

            base.BeginSearch(node, args, cancellationTokenSource, async);
        }

        protected override void DownLoadFile(FileBrowingNode fileNode, string savePath, bool persistRelativePath, CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async)
        {
            FileHelper.CreateDirectory(savePath);

            var ifileNode = fileNode as IOSDeviceFileBrowingNode;

            try
            {
                var tSavePath = string.Empty;
                if (persistRelativePath)
                {
                    tSavePath = Path.Combine(savePath, ifileNode.SourcePath).Replace('/', '\\');
                }
                else
                {
                    tSavePath = Path.Combine(savePath, ifileNode.Name).Replace('/', '\\');
                }

                FileHelper.CreateDirectory(FileHelper.GetFilePath(tSavePath));

                // 1，设置服务
                uint result = IOSDeviceCoreDll.SetIphoneFileService(IPhone.ID, IPhone.IsRoot);

                // 2，下载
                result = IOSDeviceCoreDll.CopyOneIosFile(IPhone.ID, ifileNode.SourcePath, tSavePath);
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
