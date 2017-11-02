/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 19:22:58 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 文件节点
    /// </summary>
    public abstract class FileBrowingNode
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public FileBrowingNode Parent { get; internal set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 是否是文件 如果不是文件，则是文件夹
        /// </summary>
        public bool IsFile
        {
            get { return NodeType == FileBrowingNodeType.File; }
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        internal FileBrowingNodeType NodeType { get; set; }

        /// <summary>
        /// 是否是删除文件、文件夹
        /// </summary>
        public bool IsDelete
        {
            get { return NodeState == FileBrowingNodeState.Delete; }
        }

        /// <summary>
        /// 节点状态
        /// </summary>
        public FileBrowingNodeState NodeState { get; internal set; } = FileBrowingNodeState.Normal;

        /// <summary>
        /// 文件大小
        /// </summary>
        public UInt64 FileSize { get; internal set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastWriteTime { get; internal set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; internal set; }

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime? LastAccessTime { get; internal set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<FileBrowingNode> ChildNodes { get; internal set; }

    }

    /// <summary>
    /// 文件节点状态
    /// </summary>
    public enum FileBrowingNodeState
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
    }

    /// <summary>
    /// 文件节点类型
    /// </summary>
    public enum FileBrowingNodeType
    {
        /// <summary>
        /// 文件
        /// </summary>
        File,
        /// <summary>
        /// 文件夹
        /// </summary>
        Directory,
        /// <summary>
        /// 根节点，虚拟的，没有实际意义
        /// </summary>
        Root,
        /// <summary>
        /// 分区，安卓镜像文件节点专用
        /// </summary>
        Partition,
    }
}
