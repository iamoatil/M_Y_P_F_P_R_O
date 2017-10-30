/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 18:00:03 
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

        protected override FileBrowingNode DoGetRootNode(IAsyncProgress async)
        {
            if (null == FileServiceX)
            {
                FileServiceX = new MirrorDeviceService(CreateFileSystemDevice(), async);
                FileServiceX.OpenDevice();
                FileServiceX.LoadDevicePartitions();
            }

            return new AndroidMirrorFileBrowingNode()
            {
                Name = "Root",
                NodeType = FileBrowingNodeType.Root
            };
        }

        protected override List<FileBrowingNode> DoGetChildNodes(FileBrowingNode parentNode, IAsyncProgress async)
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

        protected override void DoDownload(FileBrowingNode node, string savePath, bool persistRelativePath, IAsyncProgress async)
        {
            var mPnode = node as AndroidMirrorFileBrowingNode;

            FileServiceX.ExportFileX(mPnode.FNode, savePath, persistRelativePath);

            if (mPnode.NodeType != FileBrowingNodeType.File)
            {
                if (null == mPnode.ChildNodes)
                {
                    DoGetChildNodes(mPnode, async);
                }

                var cSavePath = savePath;
                if (!persistRelativePath)
                {
                    cSavePath = Path.Combine(savePath, mPnode.Name);
                }

                foreach (var cnode in mPnode.ChildNodes)
                {
                    DoDownload(cnode, cSavePath, persistRelativePath, async);
                }
            }
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

            public List<FileBrowingNode> ChildNodes { get; set; }

            public override string ToString()
            {
                return Name;
            }

        }

    }
}
