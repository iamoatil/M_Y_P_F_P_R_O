﻿/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 18:00:03 
 * explain :  
 *
*****************************************************************************/

using System.Collections.Generic;
using System.Threading;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 安卓镜像文件浏览服务
    /// </summary>
    internal class AndroidMirrorFileBrowsingService : AbsFileBrowsingService
    {
        /// <summary>
        /// 安卓镜像文件路径
        /// </summary>
        private string MirrorFilePath { get; set; }

        private FileServiceAbstractX FileServiceX { get; set; }

        public AndroidMirrorFileBrowsingService(string mirrorFile)
        {
            MirrorFilePath = mirrorFile;
        }

        private IFileSystemDevice CreateFileSystemDevice()
        {
            return new MirrorDevice
            {
                Source = MirrorFilePath,
                ScanModel = 0x87
            };
        }

        protected override FileBrowingNode DoGetRootNode()
        {
            if (null == FileServiceX)
            {
                FileServiceX = new MirrorDeviceService(CreateFileSystemDevice(), null);
                FileServiceX.OpenDevice();
                FileServiceX.LoadDevicePartitions();
            }

            return new AndroidMirrorFileBrowingNode()
            {
                Name = "Root",
                NodeType = FileBrowingNodeType.Root
            };
        }

        protected override List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode)
        {
            if (parentNode.NodeType == FileBrowingNodeType.Root)
            {//根节点，获取分区列表
                List<FileBrowingNode> list = new List<FileBrowingNode>();

                foreach (var part in FileServiceX.Device.Parts)
                {
                    list.Add(new AndroidMirrorFileBrowingNode()
                    {
                        Name = part.VolName.IsValid() ? string.Format("{0}-{1}", part.Name, part.VolName) : part.Name,
                        NodeType = FileBrowingNodeType.Partition,
                        Part = part,
                        Parent = parentNode,
                    });
                }

                return list;
            }
            else if (parentNode.NodeType == FileBrowingNodeType.Partition)
            {//分区，扫描分区
                var mPnode = parentNode as AndroidMirrorFileBrowingNode;
                if (null == mPnode.ChildNodes)
                {
                    mPnode.ChildNodes = new List<FileBrowingNode>();

                    foreach (var node in FileServiceX.GetFileSystemByDir(mPnode.Part))
                    {
                        mPnode.ChildNodes.Add(new AndroidMirrorFileBrowingNode()
                        {
                            Name = node.FileName,
                            FileSize = node.Size,
                            NodeType = node.IsFolder ? FileBrowingNodeType.Directory : FileBrowingNodeType.File,
                            NodeState = node.IsDelete ? EnumDataState.Deleted : EnumDataState.Normal,
                            CreateTime = BaseTypeExtension.ToSafeDateTime(node.Source.CreateTime),
                            LastWriteTime = BaseTypeExtension.ToSafeDateTime(node.Source.ModifyTime),
                            LastAccessTime = BaseTypeExtension.ToSafeDateTime(node.Source.LastAccessTime),
                            Parent = parentNode,
                            Part = mPnode.Part,
                            FNode = node,
                        });
                    }
                }

                return mPnode.ChildNodes;
            }
            else if (parentNode.NodeType == FileBrowingNodeType.Directory)
            {//文件夹 扫描文件节点
                var mPnode = parentNode as AndroidMirrorFileBrowingNode;
                if (null == mPnode.ChildNodes)
                {
                    mPnode.ChildNodes = new List<FileBrowingNode>();

                    foreach (var node in FileServiceX.GetFileSystemByDir(mPnode.FNode))
                    {
                        mPnode.ChildNodes.Add(new AndroidMirrorFileBrowingNode()
                        {
                            Name = node.FileName,
                            FileSize = node.Size,
                            NodeType = node.IsFolder ? FileBrowingNodeType.Directory : FileBrowingNodeType.File,
                            CreateTime = BaseTypeExtension.ToSafeDateTime(node.Source.CreateTime),
                            LastWriteTime = BaseTypeExtension.ToSafeDateTime(node.Source.ModifyTime),
                            LastAccessTime = BaseTypeExtension.ToSafeDateTime(node.Source.LastAccessTime),
                            Parent = parentNode,
                            Part = mPnode.Part,
                            FNode = node,
                        });
                    }
                }

                return mPnode.ChildNodes;
            }
            else
            {
                return null;
            }
        }

        protected override string DownLoadFile(FileBrowingNode fileNode, string savePath, bool persistRelativePath, CancellationTokenSource cancellationTokenSource, FileBrowingIAsyncTaskProgress async)
        {
            var mPnode = fileNode as AndroidMirrorFileBrowingNode;

            return FileServiceX.ExportFileX(mPnode.FNode, savePath, persistRelativePath, isThrowEx: true);
        }

        /// <summary>
        /// 安卓镜像文件节点
        /// </summary>
        private class AndroidMirrorFileBrowingNode : FileBrowingNode
        {
            /// <summary>
            /// 镜像分区信息 只有当NodeType为Partition时，该值才不为null
            /// </summary>
            public FileSystemPartition Part { get; set; }

            public FNodeX FNode { get; set; }

            public override string ToString()
            {
                return Name;
            }

        }

    }
}
